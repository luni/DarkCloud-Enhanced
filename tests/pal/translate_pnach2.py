#!/usr/bin/env python3
import os
import re
from collections import defaultdict

# Default: this script lives in <repo>/tests/pal.
# Set DCEX env var to the directory containing ntsc_syms.txt and pal_syms.txt.
SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
REPO = os.environ.get('REPO', os.path.dirname(os.path.dirname(SCRIPT_DIR)))
DCEX = os.environ.get('DCEX', SCRIPT_DIR)

# ---------- symbol mapping (cached physical addresses) ----------
def parse_readelf_syms(path):
    syms=[]
    with open(path) as f:
        for line in f:
            m=re.match(r'\s*\d+:\s+([0-9a-fA-F]+)\s+(\d+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(\S+)\s+(.+)', line)
            if m:
                value=int(m.group(1),16)
                size=int(m.group(2))
                typ=m.group(3)
                bind=m.group(4)
                ndx=m.group(6)
                name=m.group(7).strip()
                if ndx.isdigit() and ndx!='0':
                    syms.append({'value':value,'size':size,'type':typ,'bind':bind,'ndx':int(ndx),'name':name})
    syms.sort(key=lambda s:s['value'])
    return syms

def pair_symbols(ntsc, pal):
    ntsc_by=defaultdict(list)
    for s in ntsc:
        ntsc_by[(s['name'],s['type'],s['ndx'])].append(s)
    pal_by=defaultdict(list)
    for s in pal:
        pal_by[(s['name'],s['type'],s['ndx'])].append(s)
    mapping={}
    for k, nlist in ntsc_by.items():
        plist=pal_by.get(k,[])
        for i in range(min(len(nlist),len(plist))):
            mapping[id(nlist[i])]=plist[i]
    pairs=[(n,mapping[id(n)]) for n in ntsc if id(n) in mapping]
    pairs.sort(key=lambda x:x[0]['value'])
    return pairs

ntsc_syms=parse_readelf_syms(os.path.join(DCEX, 'ntsc_syms.txt'))
pal_syms=parse_readelf_syms(os.path.join(DCEX, 'pal_syms.txt'))
pairs=pair_symbols(ntsc_syms, pal_syms)
ntsc_sorted=[n for n,_ in pairs]
ntsc_to_pal={id(n):p for n,p in pairs}

def find_pal_vaddr(vaddr):
    if vaddr<0 or vaddr>0xFFFFFFFF:
        return None
    lo=0; hi=len(ntsc_sorted)
    while lo<hi:
        mid=(lo+hi)//2
        if ntsc_sorted[mid]['value']<=vaddr:
            lo=mid+1
        else:
            hi=mid
    candidates=[]
    for idx in [lo-1, lo]:
        if 0<=idx<len(ntsc_sorted):
            n=ntsc_sorted[idx]
            p=ntsc_to_pal[id(n)]
            candidates.append((abs(n['value']-vaddr), idx, n, p))
    if not candidates:
        return None
    candidates.sort(key=lambda x:(x[0], x[1]))
    _,_,n,p=candidates[0]
    return p['value']+(vaddr-n['value'])

func_ranges=[]
for s in ntsc_syms:
    if s['type']=='FUNC' and s['size']>0:
        func_ranges.append((s['value'], s['value']+s['size']))
func_ranges.sort()

def in_func(vaddr):
    for st,en in func_ranges:
        if st<=vaddr<en:
            return True
        if st>vaddr: break
    return False

def sign_ext16(x):
    if x & 0x8000:
        return x - 0x10000
    return x

def fmt8(v):
    return '{:08X}'.format(v & 0xFFFFFFFF)

def translate_addr_field(original):
    a=int(original,16)
    cached = a & 0x0FFFFFFF
    pal_cached = find_pal_vaddr(cached)
    if pal_cached is None:
        return original
    return fmt8((a & 0xF0000000) | (pal_cached & 0x0FFFFFFF))

# ---------- main translation ----------
input_path=os.path.join(REPO, 'Dark Cloud Improved Version/Resources/PNACH/A5C05C78.pnach')
output_path=os.path.join(REPO, 'Dark Cloud Improved Version/Resources/PNACH/SCES-50295_0BAA8DD8.pnach')

out_lines=['gametitle=Dark Cloud (PAL-M5) (SCES-50295) 0BAA8DD8\n']

last_lui={}  # reg -> (pc_cached, imm, word)

def process_value(pc_cached, value_str):
    word=int(value_str,16)
    op=word>>26
    new_word=word

    # jal / j
    if op==0x03 or op==0x02:
        target=(word & 0x3FFFFFF)<<2
        pal_target=find_pal_vaddr(target)
        if pal_target is not None and (pal_target & 3)==0:
            new_word=(word & 0xFC000000) | ((pal_target>>2) & 0x3FFFFFF)
        return fmt8(new_word), None, None

    # branches
    if op in (0x04,0x05,0x06,0x07):
        offset=sign_ext16(word & 0xFFFF)<<2
        target=pc_cached+4+offset
        pal_target=find_pal_vaddr(target)
        pal_pc=find_pal_vaddr(pc_cached)
        if pal_target is not None and pal_pc is not None:
            new_offset=(pal_target-(pal_pc+4))//4
            if -0x8000 <= new_offset < 0x8000:
                new_word=(word & 0xFFFF0000) | (new_offset & 0xFFFF)
        return fmt8(new_word), None, None

    # cop1 branch
    if op==0x11 and ((word>>21)&0x1F)==0x08:
        offset=sign_ext16(word & 0xFFFF)<<2
        target=pc_cached+4+offset
        pal_target=find_pal_vaddr(target)
        pal_pc=find_pal_vaddr(pc_cached)
        if pal_target is not None and pal_pc is not None:
            new_offset=(pal_target-(pal_pc+4))//4
            if -0x8000 <= new_offset < 0x8000:
                new_word=(word & 0xFFFF0000) | (new_offset & 0xFFFF)
        return fmt8(new_word), None, None

    # lui
    if op==0x0F:
        rt=(word>>16)&0x1F
        imm=word&0xFFFF
        return fmt8(new_word), 'lui', (pc_cached, imm, word)

    # I-type mem/arith using base register
    if op in (0x23,0x2B,0x09,0x08,0x0D):
        base=(word>>21)&0x1F
        if base in last_lui:
            lpc, limm, lword = last_lui[base]
            if pc_cached==lpc+4:
                full=(limm<<16)+sign_ext16(word & 0xFFFF)
                pal_full=find_pal_vaddr(full)
                if pal_full is not None:
                    new_lui_imm=(pal_full>>16)&0xFFFF
                    new_lui_word=(lword & 0xFFFF0000) | new_lui_imm
                    new_off=pal_full&0xFFFF
                    new_word=(word & 0xFFFF0000) | new_off
                    return fmt8(new_word), 'update_lui', fmt8(new_lui_word)

    return fmt8(new_word), None, None

with open(input_path,'r',encoding='latin1') as f:
    lines=f.readlines()

for line in lines:
    s=line.strip()
    comment=''
    if '//' in s:
        code_part, comment = s.split('//',1)
        code_part=code_part.strip()
        comment=' //'+comment
    else:
        code_part=s
    if not code_part:
        continue
    if code_part.startswith('gametitle'):
        continue

    m=re.match(r'patch=1,EE,([0-9a-fA-F]+),extended,([0-9a-fA-F]+)', code_part)
    if not m:
        out_lines.append(line)
        continue

    f1=m.group(1)
    f2=m.group(2)

    if f1.startswith('E'):
        new_f2=translate_addr_field(f2)
        out_lines.append('patch=1,EE,{},extended,{}{}\n'.format(f1,new_f2,comment))
        last_lui.clear()
        continue

    new_f1=translate_addr_field(f1)
    a=int(f1,16)
    cached_addr=a & 0x0FFFFFFF

    if in_func(cached_addr):
        new_f2, action, payload = process_value(cached_addr, f2)
        if action=='update_lui':
            # replace previous output line's value (same address)
            prev=out_lines[-1]
            pm=re.match(r'(patch=1,EE,[0-9a-fA-F]+,extended,)([0-9a-fA-F]+)(.*)', prev)
            if pm:
                out_lines[-1]='{}{}{}\n'.format(pm.group(1), payload, pm.group(3) or '')
            # clear last_lui because this pair is consumed
            last_lui.clear()
        elif action=='lui':
            pc, imm, word = payload
            rt=(word>>16)&0x1F
            last_lui[rt]=(pc, imm, word)
        else:
            last_lui.clear()
    else:
        new_f2=f2
        last_lui.clear()

    out_lines.append('patch=1,EE,{},extended,{}{}\n'.format(new_f1,new_f2,comment))

with open(output_path,'w') as f:
    f.writelines(out_lines)
print('Wrote', output_path)
