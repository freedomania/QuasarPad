# QuasarPad Build Script
# Requires: .NET 8 SDK on the build machine
# Runtime on user machine: .NET 8 Desktop Runtime (much smaller app)

Write-Host "=== Building QuasarPad (Framework-dependent, small size) ===" -ForegroundColor Cyan

dotnet restore src/QuasarPad/QuasarPad.csproj
dotnet build src/QuasarPad/QuasarPad.csproj -c Release

Write-Host "`nPublishing Framework-dependent (recommended, ~15-40MB)..." -ForegroundColor Yellow
dotnet publish src/QuasarPad/QuasarPad.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o publish/Small

Write-Host "`nOptional: Self-contained (large, no .NET install needed)..." -ForegroundColor DarkYellow
dotnet publish src/QuasarPad/QuasarPad.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o publish/PortableFull

Write-Host "`n=== Done ===" -ForegroundColor Green
Write-Host "Small build:  publish\Small\QuasarPad.exe"
Write-Host "Full build:   publish\PortableFull\QuasarPad.exe"
Write-Host "`nUsers of Small build need: https://dotnet.microsoft.com/download/dotnet/8.0 (Desktop Runtime)"
