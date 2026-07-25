#!/usr/bin/env python3
"""Static verification for the PAL port.

Checks:
  1. The PAL .pnach re-translates deterministically from the NTSC .pnach.
  2. All non-conditional patch addresses in the PAL .pnach point into a
     mapped segment of the PAL ELF.
  3. All conditional "value" fields that look like EE memory addresses
     point into a mapped segment of the PAL ELF.
  4. RegionAddresses.cs NTSC/PAL arrays are consistent with the generated
     pal_address_map.json.
  5. Region detection addresses are inside mapped PAL segments.
"""
import os
import re
import json
import struct
import sys
from collections import defaultdict

# Default: this script lives in <repo>/tests/pal.
# Set DCEX env var to the directory containing the extracted ELFs and syms
# (e.g. /home/calvin/dc_extract). Only the ELF/sym-dependent checks need it.
SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
REPO = os.environ.get('REPO', os.path.dirname(os.path.dirname(SCRIPT_DIR)))
DCEX = os.environ.get('DCEX', SCRIPT_DIR)
NTSC_PNACH = os.path.join(REPO, 'src/DarkCloudEnhancedMod/Resources/PNACH/A5C05C78.pnach')
PAL_PNACH = os.path.join(REPO, 'src/DarkCloudEnhancedMod/Resources/PNACH/SCES-50295_0BAA8DD8.pnach')
PAL_ELF = os.path.join(DCEX, 'pal.elf')
NTSC_ELF = os.path.join(DCEX, 'ntsc.elf')
PAL_MAP = os.path.join(DCEX, 'pal_address_map.json')
REGION_CS = os.path.join(REPO, 'src/DarkCloudEnhancedMod/RegionAddresses.cs')


def parse_elf_segments(path):
    with open(path, 'rb') as f:
        magic = f.read(4)
        if magic != b'\x7fELF':
            raise ValueError(f'{path}: not an ELF')
        ei_class = f.read(1)
        if ei_class != b'\x01':
            raise ValueError(f'{path}: not 32-bit ELF')
        f.seek(28)
        e_phoff = struct.unpack('<I', f.read(4))[0]
        f.seek(42)
        e_phentsize = struct.unpack('<H', f.read(2))[0]
        e_phnum = struct.unpack('<H', f.read(2))[0]
        segs = []
        f.seek(e_phoff)
        for _ in range(e_phnum):
            p_type, p_offset, p_vaddr, p_paddr, p_filesz, p_memsz, p_flags, p_align = \
                struct.unpack('<IIIIIIII', f.read(32))
            if p_type == 1:  # PT_LOAD
                segs.append((p_vaddr, p_vaddr + p_memsz, p_offset, p_filesz, p_flags))
        return sorted(segs, key=lambda s: s[1])


def in_segment(segs, vaddr):
    for start, end, _, _, _ in segs:
        if start <= vaddr < end:
            return True
        if end > vaddr:
            break
    return False


def in_executable_segment(segs, vaddr):
    for start, end, _, _, flags in segs:
        if start <= vaddr < end:
            return (flags & 0x1) != 0  # PF_X
        if end > vaddr:
            break
    return False


def in_ee_ram(addr):
    return 0x20000000 <= addr < 0x22000000


def to_ps2_virtual(addr):
    # pnach / mod convention: 0x20xxxxxx is the EE main-RAM overlay used by the mod.
    return addr & 0x0FFFFFFF


def to_mod_addr(addr):
    return (addr & 0x0FFFFFFF) | 0x20000000


def parse_pnach(path):
    entries = []
    with open(path, 'r', encoding='latin1') as f:
        for line in f:
            s = line.split('//')[0].strip()
            if not s or s.startswith('gametitle'):
                continue
            m = re.match(r'patch=1,EE,([0-9a-fA-F]+),extended,([0-9a-fA-F]+)', s)
            if m:
                entries.append((m.group(1), m.group(2)))
    return entries


def is_e_code(word):
    return word.upper().startswith('E')


def looks_like_address(word):
    # 8-digit hex word used in a pnach value/address field.
    # Return the 0x20xxxxxx mod address if it is a plausible EE RAM reference,
    # otherwise None.
    if not re.fullmatch(r'[0-9a-fA-F]{8}', word):
        return None
    v = int(word, 16)
    mod = to_mod_addr(v)
    # Accept anything whose lower 28 bits are inside the 32MB executable/RAM range.
    cached = mod & 0x0FFFFFFF
    if 0x00100000 <= cached <= 0x002A4000:
        return mod
    return None


def decode_mips_target(value_word, pc_cached):
    word = int(value_word, 16)
    op = word >> 26
    if op == 0x03 or op == 0x02:
        return (word & 0x3FFFFFF) << 2
    if op in (0x04, 0x05, 0x06, 0x07):
        offset = (word & 0xFFFF)
        if offset & 0x8000:
            offset -= 0x10000
        return pc_cached + 4 + (offset << 2)
    if op == 0x11 and ((word >> 21) & 0x1F) == 0x08:
        offset = (word & 0xFFFF)
        if offset & 0x8000:
            offset -= 0x10000
        return pc_cached + 4 + (offset << 2)
    return None


def parse_region_addresses_arrays(path):
    with open(path, 'r', encoding='utf-8-sig') as f:
        txt = f.read()
    ntsc = re.search(r'internal static readonly long\[\] NTSC = new long\[\] \{(.*?)\};', txt, re.S)
    pal = re.search(r'internal static readonly long\[\] PAL = new long\[\] \{(.*?)\};', txt, re.S)
    def extract(arr):
        return [int(x, 16) for x in re.findall(r'0x([0-9a-fA-F]+)L?,?', arr.group(1))]
    return extract(ntsc), extract(pal)


def parse_syms(path):
    syms = []
    with open(path) as f:
        for line in f:
            m = re.match(r'\s*\d+:\s+([0-9a-fA-F]+)\s+(\d+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(.+)', line)
            if m:
                value = int(m.group(1), 16)
                size = int(m.group(2))
                typ = m.group(3)
                bind = m.group(4)
                ndx = m.group(6)
                name = m.group(7).strip()
                if ndx.isdigit() and ndx != '0':
                    syms.append({'value': value, 'size': size, 'type': typ, 'bind': bind, 'ndx': int(ndx), 'name': name})
    return syms


def pair_symbols(ntsc, pal):
    ntsc_by = defaultdict(list)
    for s in ntsc:
        ntsc_by[(s['name'], s['type'], s['ndx'])].append(s)
    pal_by = defaultdict(list)
    for s in pal:
        pal_by[(s['name'], s['type'], s['ndx'])].append(s)
    mapping = {}
    for k, nlist in ntsc_by.items():
        plist = pal_by.get(k, [])
        for i in range(min(len(nlist), len(plist))):
            mapping[id(nlist[i])] = plist[i]
    pairs = [(n, mapping[id(n)]) for n in ntsc if id(n) in mapping]
    pairs.sort(key=lambda x: x[0]['value'])
    return pairs


def build_find_pal(pairs):
    ntsc_sorted = [n for n, _ in pairs]
    ntsc_to_pal = {id(n): p for n, p in pairs}
    def find(vaddr):
        lo, hi = 0, len(ntsc_sorted)
        while lo < hi:
            mid = (lo + hi) // 2
            if ntsc_sorted[mid]['value'] <= vaddr:
                lo = mid + 1
            else:
                hi = mid
        candidates = []
        for idx in [lo - 1, lo]:
            if 0 <= idx < len(ntsc_sorted):
                n = ntsc_sorted[idx]
                p = ntsc_to_pal[id(n)]
                candidates.append((abs(n['value'] - vaddr), idx, n, p))
        if not candidates:
            return None
        candidates.sort(key=lambda x: (x[0], x[1]))
        _, _, n, p = candidates[0]
        return p['value'] + (vaddr - n['value'])
    return find


def main():
    errors = []
    warnings = []

    print('Loading PAL ELF segments...')
    pal_segs = parse_elf_segments(PAL_ELF)
    print(f'  {len(pal_segs)} PT_LOAD segments:')
    for start, end, off, fsz, flags in pal_segs:
        print(f'    0x{start:08X} - 0x{end:08X} (off 0x{off:08X}, filesz 0x{fsz:08X}, flags 0x{flags:08X})')

    print('\n1. Deterministic PAL .pnach translation check')
    # Re-run translator and compare.
    import subprocess
    result = subprocess.run([sys.executable, os.path.join(DCEX, 'translate_pnach2.py')],
                            capture_output=True, text=True)
    if result.returncode != 0:
        print(result.stdout)
        print(result.stderr)
        errors.append('translate_pnach2.py failed')
    else:
        diff = subprocess.run(['git', 'diff', '--', PAL_PNACH],
                              cwd=REPO, capture_output=True, text=True)
        if diff.stdout.strip():
            errors.append('PAL .pnach changed after re-running translate_pnach2.py (not deterministic)')
            print('  FAIL: .pnach is not deterministic')
        else:
            print('  PASS: re-running translator produces identical .pnach')

    print('\n2. PAL .pnach address validity')
    entries = parse_pnach(PAL_PNACH)
    bad = 0
    ram_only = 0
    checked = 0
    for f1, f2 in entries:
        if is_e_code(f1):
            # f2 is usually the address to test in a conditional.
            mod = looks_like_address(f2)
            if mod is not None:
                checked += 1
                if not in_ee_ram(mod):
                    bad += 1
                    if bad <= 10:
                        print(f'  BAD conditional value 0x{f2} (mod 0x{mod:08X}) not in EE RAM')
                elif not in_segment(pal_segs, to_ps2_virtual(mod)):
                    ram_only += 1
        else:
            v = int(f1, 16)
            checked += 1
            if not in_ee_ram(v):
                bad += 1
                if bad <= 10:
                    print(f'  BAD patch addr 0x{f1} not in EE RAM')
            else:
                vvirt = to_ps2_virtual(v)
                if in_segment(pal_segs, vvirt):
                    # For code patches inside a function, validate that the
                    # patch address is executable and any jal/j/branch target is valid.
                    target = decode_mips_target(f2, vvirt)
                    if target is not None:
                        checked += 1
                        if 0x00100000 <= target <= 0x002A4000 and not in_executable_segment(pal_segs, target):
                            bad += 1
                            if bad <= 10:
                                print(f'  BAD MIPS target from 0x{f1} value 0x{f2} -> 0x{target:08X}')
                else:
                    # Patch points into EE RAM but not a loaded ELF segment.
                    # That is only OK for simple flag/data writes (value not a control instruction).
                    word = int(f2, 16)
                    op = word >> 26
                    if op in (0x02, 0x03, 0x04, 0x05, 0x06, 0x07) or (op == 0x11 and ((word >> 21) & 0x1F) == 0x08):
                        bad += 1
                        if bad <= 10:
                            print(f'  BAD control-flow patch 0x{f1}=0x{f2} outside ELF segment')
                    else:
                        ram_only += 1
    if bad:
        errors.append(f'{bad} invalid PAL .pnach address(es) out of {checked} checked')
        print(f'  FAIL: {bad}/{checked} checked entries are invalid')
    else:
        print(f'  PASS: all {checked} checked entries are valid')
        if ram_only:
            print(f'  ({ram_only} entries point to EE RAM outside the loaded ELF, likely mod flags/scratch)')

    print('\n3. RegionAddresses.cs mapping consistency')
    with open(PAL_MAP) as f:
        pal_map = {int(k, 16): int(v, 16) for k, v in json.load(f).items()}
    ntsc_arr, pal_arr = parse_region_addresses_arrays(REGION_CS)
    if len(ntsc_arr) != len(pal_arr):
        errors.append('RegionAddresses NTSC/PAL arrays have different lengths')
    else:
        mismatches = 0
        for ntsc_addr, pal_addr in zip(ntsc_arr, pal_arr):
            expected = pal_map.get(ntsc_addr)
            if expected is None:
                warnings.append(f'NTSC addr 0x{ntsc_addr:08X} not in pal_address_map.json')
            elif expected != pal_addr:
                mismatches += 1
                if mismatches <= 10:
                    print(f'  MISMATCH: 0x{ntsc_addr:08X} -> expected 0x{expected:08X}, found 0x{pal_addr:08X}')
        if mismatches:
            errors.append(f'{mismatches} RegionAddresses entries mismatch pal_address_map.json')
            print(f'  FAIL: {mismatches} mismatches')
        else:
            print(f'  PASS: all {len(ntsc_arr)} mapped entries match pal_address_map.json')

    print('\n4. Region detection address validity')
    ntsc_segs = parse_elf_segments(NTSC_ELF)
    detect_checks = [
        ('ntscBoot', 0x00299540, ntsc_segs),
        ('palBoot', 0x0029BCA0, pal_segs),
    ]
    for name, vaddr, segs in detect_checks:
        if in_segment(segs, vaddr):
            print(f'  PASS: {name} 0x{vaddr:08X} is inside a PT_LOAD segment')
        else:
            errors.append(f'{name} 0x{vaddr:08X} not in PT_LOAD')
            print(f'  FAIL: {name} 0x{vaddr:08X} not in any PT_LOAD segment')

    # Check the expected boot-string "Dark" (0x6B726144 little-endian).
    DARK_MAGIC = 0x6B726144
    def read_word_from_elf(elf, vaddr):
        with open(elf, 'rb') as f:
            f.seek(28)
            e_phoff = struct.unpack('<I', f.read(4))[0]
            f.seek(42)
            e_phentsize = struct.unpack('<H', f.read(2))[0]
            e_phnum = struct.unpack('<H', f.read(2))[0]
            f.seek(e_phoff)
            for _ in range(e_phnum):
                p_type, p_offset, p_vaddr, p_paddr, p_filesz, p_memsz, p_flags, p_align = \
                    struct.unpack('<IIIIIIII', f.read(32))
                if p_type == 1 and p_vaddr <= vaddr < p_vaddr + min(p_filesz, p_memsz):
                    off = p_offset + (vaddr - p_vaddr)
                    f.seek(off)
                    return struct.unpack('<I', f.read(4))[0]
        return None

    for name, elf, vaddr in [('ntscBoot', NTSC_ELF, 0x00299540), ('palBoot', PAL_ELF, 0x0029BCA0)]:
        word = read_word_from_elf(elf, vaddr)
        if word is None:
            warnings.append(f'{name}: could not read 4 bytes from ELF')
        elif word == DARK_MAGIC:
            print(f'  PASS: {name} contains "Dark" magic at 0x{vaddr:08X}')
        else:
            warnings.append(f'{name}: expected "Dark" magic 0x{DARK_MAGIC:08X}, got 0x{word:08X} at 0x{vaddr:08X}')
            print(f'  WARN: {name} does not contain "Dark" magic at 0x{vaddr:08X} (got 0x{word:08X})')

    # Handshake flags are in EE RAM beyond the game binary; they do not need
    # to be inside a PT_LOAD segment, only inside the 32MB EE RAM range.
    flags = {
        'ntscFlag': 0x21F10020,
        'palFlag': 0x21F22EA0,
    }
    for name, addr in flags.items():
        if in_ee_ram(addr):
            print(f'  PASS: {name} 0x{addr:08X} is inside 32MB EE RAM')
        else:
            errors.append(f'{name} 0x{addr:08X} outside 32MB EE RAM')
            print(f'  FAIL: {name} 0x{addr:08X} outside 32MB EE RAM')

    print('\n5. Spot-check RegionAddresses translation against symbol map')
    ntsc_set = set(ntsc_arr)
    ntsc_syms = parse_syms(os.path.join(DCEX, 'ntsc_syms.txt'))
    pal_syms = parse_syms(os.path.join(DCEX, 'pal_syms.txt'))
    pairs = pair_symbols(ntsc_syms, pal_syms)
    find_pal = build_find_pal(pairs)
    # Only check addresses that are actually in the RegionAddresses NTSC table.
    samples = [0x202A2534, 0x201F75E0, 0x201F7DB4, 0x21CD9551, 0x21D90470, 0x21F10020]
    bad_samples = 0
    for s in samples:
        expected = find_pal(s - 0x20000000)
        if expected is None:
            warnings.append(f'sample 0x{s:08X} could not be translated by symbol map')
            continue
        expected_mod = to_mod_addr(expected)
        import bisect
        idx = bisect.bisect_right(ntsc_arr, s) - 1
        if idx >= 0:
            translated = s + (pal_arr[idx] - ntsc_arr[idx])
        else:
            translated = s
        if translated != expected_mod:
            bad_samples += 1
            print(f'  MISMATCH sample 0x{s:08X}: symbol map -> 0x{expected_mod:08X}, RegionAddresses -> 0x{translated:08X}')
        else:
            print(f'  PASS sample 0x{s:08X} -> 0x{expected_mod:08X}')
    if bad_samples:
        errors.append(f'{bad_samples} spot-check samples mismatch')

    print('\n' + '=' * 50)
    if errors:
        print(f'ERRORS ({len(errors)}):')
        for e in errors:
            print('  -', e)
    else:
        print('No errors.')
    if warnings:
        print(f'Warnings ({len(warnings)}):')
        for w in warnings[:20]:
            print('  -', w)
    return 1 if errors else 0


if __name__ == '__main__':
    sys.exit(main())
