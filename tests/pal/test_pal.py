#!/usr/bin/env python3
"""Lightweight CI-friendly checks for the PAL port.

Does not require extracted ELF files or symbol dumps; it only uses files
committed in the repository.
"""
import json
import os
import re
import sys

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
REPO = os.path.dirname(os.path.dirname(SCRIPT_DIR))
PAL_MAP = os.path.join(SCRIPT_DIR, 'pal_address_map.json')
REGION_CS = os.path.join(REPO, 'src', 'DarkCloudEnhancedMod', 'RegionAddresses.cs')
NTSC_PNACH = os.path.join(REPO, 'src', 'DarkCloudEnhancedMod', 'Resources', 'PNACH', 'A5C05C78.pnach')
PAL_PNACH = os.path.join(REPO, 'src', 'DarkCloudEnhancedMod', 'Resources', 'PNACH', 'SCES-50295_0BAA8DD8.pnach')


def parse_region_addresses_arrays(path):
    with open(path) as f:
        text = f.read()

    def parse_array(name):
        m = re.search(rf'{name}\s*=\s*new\s+long\[\]\s*{{(.*?)}};', text, re.S)
        if not m:
            raise ValueError(f'could not find {name} array in {path}')
        return [int(x, 16) for x in re.findall(r'0x([0-9a-fA-F]+)L?', m.group(1))]

    return parse_array('NTSC'), parse_array('PAL')


def in_ee_ram(addr):
    return 0x20000000 <= addr <= 0x22000000


def test_region_addresses_match_map():
    ntsc_arr, pal_arr = parse_region_addresses_arrays(REGION_CS)
    assert len(ntsc_arr) == len(pal_arr), 'NTSC/PAL arrays have different lengths'

    with open(PAL_MAP) as f:
        pal_map = {int(k, 16): int(v, 16) for k, v in json.load(f).items()}

    mismatches = 0
    for ntsc_addr, pal_addr in zip(ntsc_arr, pal_arr):
        expected = pal_map.get(ntsc_addr)
        if expected is not None and expected != pal_addr:
            mismatches += 1
            if mismatches <= 5:
                print(f'  MISMATCH: 0x{ntsc_addr:08X} -> expected 0x{expected:08X}, found 0x{pal_addr:08X}')

    assert mismatches == 0, f'{mismatches} RegionAddresses entries mismatch pal_address_map.json'
    print(f'PASS: all {len(ntsc_arr)} mapped entries match pal_address_map.json')


def parse_pnach_entries(path):
    entries = []
    with open(path, encoding='latin1') as f:
        for line in f:
            s = line.strip()
            if not s or s.startswith('//') or s.startswith('gametitle'):
                continue
            m = re.match(r'patch=1,EE,([0-9a-fA-F]+),extended,([0-9a-fA-F]+)', s)
            if m:
                entries.append((m.group(1), m.group(2)))
    return entries


def test_pnach_addresses_in_ram():
    for label, path in [('NTSC', NTSC_PNACH), ('PAL', PAL_PNACH)]:
        entries = parse_pnach_entries(path)
        bad = 0
        for f1, f2 in entries:
            if f1.upper().startswith('E'):
                # conditional value; 8-digit hex, may include type nibble
                if re.fullmatch(r'[0-9a-fA-F]{8}', f2):
                    v = int(f2, 16)
                    if v != 0 and not in_ee_ram((v & 0x0FFFFFFF) | 0x20000000):
                        bad += 1
            else:
                v = int(f1, 16)
                if not in_ee_ram(v):
                    bad += 1
        assert bad == 0, f'{label} .pnach has {bad} address(es) outside EE RAM'
        print(f'PASS: {label} .pnach has {len(entries)} entries and all addresses are in EE RAM')


def main():
    test_region_addresses_match_map()
    test_pnach_addresses_in_ram()
    print('\nAll CI PAL tests passed.')
    return 0


if __name__ == '__main__':
    sys.exit(main())
