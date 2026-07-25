#!/bin/bash
set -e

SMOKE_DIR="$(cd "$(dirname "$0")" && pwd)"
REPO_ROOT="$(cd "$SMOKE_DIR/../.." && pwd)"

echo "Building fake PIE PCSX2 process..."
gcc -fPIC -fPIE -pie -Wl,-E -o "$SMOKE_DIR/fake_pcsx2" "$SMOKE_DIR/fake_pcsx2.c"

echo "Running integration tests..."
cd "$REPO_ROOT"
dotnet test "tests/DarkCloudEnhancedMod.IntegrationTests/DarkCloudEnhancedMod.IntegrationTests.csproj" -c Release
