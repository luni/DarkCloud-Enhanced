#!/bin/bash
set -e

SMOKE_DIR="$(cd "$(dirname "$0")" && pwd)"
REPO_ROOT="$(cd "$SMOKE_DIR/../.." && pwd)"
BUILD_DIR="$REPO_ROOT/Dark Cloud Improved Version/bin/Release"
ASM="$BUILD_DIR/Dark Cloud Enhanced Mod.exe"

echo "Building fake PIE PCSX2 process..."
gcc -fPIC -fPIE -pie -Wl,-E -o "$SMOKE_DIR/fake_pcsx2" "$SMOKE_DIR/fake_pcsx2.c"

echo "Building smoke test..."
mcs /out:"$SMOKE_DIR/SmokeTest.exe" /r:"$ASM" "$SMOKE_DIR/SmokeTest.cs"

echo "Running smoke test..."
cd "$SMOKE_DIR"
mono "$SMOKE_DIR/SmokeTest.exe" "$ASM"
