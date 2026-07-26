# Supported Environments

## Host

| Host | Project | Target framework | Run | Build |
|---|---|---|---|---|
| Modern WinForms host | `src/DarkCloud.App.WinForms/DarkCloud.App.WinForms.csproj` | .NET 8 (Windows) | Windows only | Windows and Linux (`<EnableWindowsTargeting>true</EnableWindowsTargeting>`) |

## Requirements

- **Windows**: .NET 8 SDK. Visual Studio 2022 or `msbuild` can build the full solution, including the native PCSX2 offset reader.
- **Linux**: .NET 8 SDK and Mono (for .NET Framework 4.8 tests). The full solution cannot be built with `dotnet` because it contains a native `vcxproj`, but the C# host and test projects can be built and tested individually.

## Build

```bash
# Full solution (Windows / MSBuild)
msbuild -restore "DarkCloud-Enhanced.sln" /p:Configuration=Release /p:Platform=x64 /p:RestoreLockedMode=true

# Modern host (Linux / dotnet; skips the native vcxproj)
dotnet restore "DarkCloud-Enhanced.sln" --locked-mode
dotnet build "src/DarkCloud.App.WinForms/DarkCloud.App.WinForms.csproj" -c Release --no-restore
```

## Test

```bash
# All C# test projects on Linux
dotnet test "tests/DarkCloud.Core.Tests/DarkCloud.Core.Tests.csproj" -c Release --no-restore
dotnet test "tests/DarkCloud.Memory.Abstractions.Tests/DarkCloud.Memory.Abstractions.Tests.csproj" -c Release --no-restore
dotnet test "tests/DarkCloudEnhancedMod.IntegrationTests/DarkCloudEnhancedMod.IntegrationTests.csproj" -c Release --no-restore
dotnet test "tests/DarkCloud.Memory.Windows.IntegrationTests/DarkCloud.Memory.Windows.IntegrationTests.csproj" -c Release --no-restore

# Full solution tests on Windows (after msbuild)
dotnet test "DarkCloud-Enhanced.sln" -c Release --no-build
```

## Known limitations

- The modern host (`DarkCloud.App.WinForms`) is Windows-only at runtime because it uses WinForms and Windows process APIs.
- Linux CI builds the modern host to verify compilation and runs the C# test suites; it does not run the WinForms executable.
- Native helper (`native/pcsx2_offsetreader/pcsx2_offsetreader.vcxproj`) builds only on Windows with the C++ workload installed.
