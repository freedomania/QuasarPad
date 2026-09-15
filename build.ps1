# QuasarPad Build Script (run on Windows with .NET 8 SDK)

Write-Host "=== Building QuasarPad ===" -ForegroundColor Cyan

# Restore
dotnet restore src/QuasarPad/QuasarPad.csproj

# Build
dotnet build src/QuasarPad/QuasarPad.csproj -c Release

# Publish Self-contained Portable (recommended)
Write-Host "`nPublishing Portable (Self-contained)..." -ForegroundColor Yellow
dotnet publish src/QuasarPad/QuasarPad.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o publish/Portable

# Publish Framework-dependent (smaller)
Write-Host "Publishing Framework-dependent..." -ForegroundColor Yellow
dotnet publish src/QuasarPad/QuasarPad.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o publish/FrameworkDependent

Write-Host "`n=== Build Complete ===" -ForegroundColor Green
Write-Host "Portable version: publish/Portable/QuasarPad.exe"
Write-Host "Framework version: publish/FrameworkDependent/QuasarPad.exe"
Write-Host "`nFor Installer: Use Inno Setup or MSIX packaging with the Portable output."
