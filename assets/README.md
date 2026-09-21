# Assets

- `logo.svg` — QuasarPad mark (used on GitHub README)

## Application / installer icon (`.ico`)

Windows needs a real `.ico` file for the `.exe` and Inno Setup.

1. Create folder: `src\QuasarPad\Assets\`
2. Create `QuasarPad.ico` (sizes 16, 32, 48, 256 recommended).
   - Easy: open `logo.svg` in a browser, screenshot/export PNG, then convert to ICO with any online “PNG to ICO” tool or [GIMP](https://www.gimp.org/) / [IcoFX](https://icofx.ro/).
   - Or design a **Q** on dark blue circle to match the SVG.
3. Rebuild:

```powershell
dotnet publish src\QuasarPad\QuasarPad.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o publish\Small
```

The `.csproj` picks up `Assets\QuasarPad.ico` automatically when the file exists.
Inno Setup uses the same path via `SetupIconFile`.
