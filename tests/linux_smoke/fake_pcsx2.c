#define _GNU_SOURCE
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <sys/mman.h>
#include <stdint.h>

// Exported symbol the mod searches for in the ELF dynamic symbol table.
__attribute__((visibility("default"))) void* EEmem = NULL;

int main(void)
{
    // 32 MB, matching the PS2 EE RAM size the mod expects.
    size_t size = 32ULL * 1024 * 1024;
    EEmem = mmap(NULL, size, PROT_READ | PROT_WRITE, MAP_PRIVATE | MAP_ANONYMOUS, -1, 0);
    if (EEmem == MAP_FAILED) {
        perror("mmap");
        return 1;
    }

    // Marker at the start of EE memory.
    memcpy(EEmem, "DarkClou", 8);

    // Boot string marker at PS2 offset 0x20299540 (offset 0x299540).
    memcpy((char*)EEmem + 0x299540, "Dark", 4);

    // PAL flag marker at PS2 offset 0x21F22EA1 (offset 0x1F22EA0).
    *((uint8_t*)EEmem + 0x1F22EA0) = 1;

    printf("FAKE_PCSX2 pid=%d EEmem=%p\n", getpid(), EEmem);
    fflush(stdout);

    while (1) sleep(1);
    return 0;
}
