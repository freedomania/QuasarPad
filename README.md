# QuasarPad

**Offline Text & Markup Toolkit** for Windows 10/11.

Notepad-style editor + converters + utilities — **no telemetry, offline-first, MIT License**.

> Looking for testers! Please download, try features, and [open an issue](https://github.com/freedomania/QuasarPad/issues) if something breaks.

## Features (v1.5)

### Editor
- Multi-tab (close with × on each tab)
- Clear button (empty current tab fast)
- Line numbers, word wrap, status bar
- Pure Mode / Dark Mode
- Find & Replace
- Right-click Cut / Copy / Paste

### Markup & text
- Markdown → HTML (live preview, clean full HTML document)
- HTML Test (edit + offline preview)
- Text Tools (Base64, URL, hash, case…)
- Regex Tester, Diff, Timestamp tools
- JSON format, YAML clean, slug, sort / unique lines

### Converters (offline constants only)
- Unit Converter (temp, length, weight, data, area, volume, speed, angle, time, pressure, energy, power)
- Number base (bin/oct/dec/hex)
- Color Tools (large offline palette → HEX / RGB + copy)
- Generators (UUID, password)

## Requirements

| Build | Needs |
|--------|--------|
| **Small** (recommended) | [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) |
| **PortableFull** | Nothing extra (larger exe) |

## Download / test builds

Until formal Releases are published, build from source:

```powershell
git clone https://github.com/freedomania/QuasarPad.git
cd QuasarPad
dotnet publish src\QuasarPad\QuasarPad.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o publish\Small
```

Run: `publish\Small\QuasarPad.exe`

### Installer (optional)

1. Build the Small publish folder (above).
2. Install [Inno Setup](https://jrsoftware.org/isinfo.php).
3. Open `installer\QuasarPad.iss` and compile.

The installer creates a Start Menu entry and optional desktop shortcut under `C:\QuasarPad\` (not System32).

## Support the project

QuasarPad is **free for everyone**. See [SUPPORT.md](SUPPORT.md) for:

- How to help (star, issues, PRs)
- Optional donations
- Supporter idea (no paywall on core tools)

## License

MIT — see [LICENSE](LICENSE)
