# Windows installer

## Steps

1. On a Windows PC with .NET 8 SDK:

```powershell
cd QuasarPad
dotnet publish src\QuasarPad\QuasarPad.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o publish\Small
```

2. Install [Inno Setup 6](https://jrsoftware.org/isinfo.php) (free).

3. Open `installer\QuasarPad.iss` in Inno Setup → **Compile**.

4. Output: `publish\Installer\QuasarPad-Setup-1.5.0.exe`

## Notes

- Install path defaults to `C:\QuasarPad` (user-level, no admin required).
- Users of the **Small** build still need [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0).
- For a fully offline installer with no runtime dependency, publish `--self-contained true` into a folder and point the `.iss` `[Files]` source at that folder (larger setup).
