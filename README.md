# QuasarPad

**Offline Text & Markup Toolkit** for Windows.

Pure Mode Notepad + Markdown → HTML + HTML Test + Text Tools.  
No telemetry. MIT License.

## Features (v1.2)

- **Pure Mode** – clean like classic Notepad
- **Line numbers** (AvalonEdit)
- **Markdown → HTML** with Live Preview and full clean HTML (`<!DOCTYPE html>…`)
- **HTML Test** – edit HTML and preview offline
- **Text Tools** – Base64, URL encode/decode, case, MD5, SHA256, reverse, remove empty lines
- **JSON Format**
- **Format Text / Reflow Paragraphs**
- **Find / Replace**
- **Settings window** (theme, font, word wrap, pure mode)
- **Dark / Light theme**
- Auto-save settings to `%AppData%\QuasarPad\`
- **Small build** (Framework-dependent) or optional full portable

## Requirements

- Windows 10 / 11 (64-bit)
- **Small build:** [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)
- **PortableFull build:** no runtime install needed (larger file)

## Build

```powershell
git clone https://github.com/freedomania/QuasarPad.git
cd QuasarPad
.\build.ps1
```

- `publish\Small\QuasarPad.exe` – recommended (small)
- `publish\PortableFull\QuasarPad.exe` – self-contained

## Install location

Copy the Small (or PortableFull) output to e.g. `C:\QuasarPad\` — **not** System32.

## License

MIT — see [LICENSE](LICENSE)

## Support

Optional. Star the repo or open issues:  
https://github.com/freedomania/QuasarPad
