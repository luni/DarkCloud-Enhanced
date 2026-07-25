# Project Notes

## Build

```bash
dotnet build "DarkCloud-Enhanced.sln"
```

## Test

```bash
dotnet test "DarkCloud-Enhanced.sln" --no-build
```

## Verification

- Build with `dotnet build`.
- Run `dotnet test` for Core, Memory Abstractions, and integration test projects.
- The WinForms project targets .NET Framework and builds on Linux with Mono/.NET reference assemblies.
