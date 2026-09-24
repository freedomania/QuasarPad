# QuasarPad

**Offline Text & Markup Toolkit** for Windows 10/11 (64-bit).

A lightweight editor with built-in converters and utilities — **no telemetry**, offline-first, **MIT License**.

<p align="center">
  <img src="assets/logo.svg" width="120" alt="QuasarPad logo"/>
</p>

> **Download the installer:** [Latest Release](https://github.com/freedomania/QuasarPad/releases/latest)  
> Testers welcome — open an [Issue](https://github.com/freedomania/QuasarPad/issues) if something breaks.

## Why QuasarPad?

| | Windows Notepad | QuasarPad |
|--|-----------------|-----------|
| Tabs | Limited / different UX | Multi-tab with close (×) |
| Clear text | Manual select-delete | One-click **Clear** |
| Markdown → HTML | No | Live preview + clean full HTML |
| Units / color / bases | No | Offline converters |
| Telemetry | OS-integrated | None |

Focused on **offline utilities** next to a clean editor — not a Notepad++ plugin clone.

## Features (v1.5)

- Multi-tab editor, Clear, Find/Replace, Dark / Pure mode
- Markdown → HTML, HTML Test, Text Tools, Regex, Diff, Timestamps
- Unit converter, number bases, color palette, UUID & password generators
- JSON / YAML helpers, sort & unique lines

## Requirements

- Windows 10/11 **64-bit**
- [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) (x64)

## Download

1. Open **[Releases](https://github.com/freedomania/QuasarPad/releases)**
2. Download **QuasarPad-Setup-….exe**
3. Install (default path: **`C:\Program Files\QuasarPad`**)
4. Optional: create a desktop shortcut during setup

## Build from source

```powershell
git clone https://github.com/freedomania/QuasarPad.git
cd QuasarPad
dotnet publish src\QuasarPad\QuasarPad.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o publish\Small
```

## Installer (developers)

Inno Setup is only needed to *create* the setup package.

1. Publish to `publish\Small` (command above).
2. Place `QuasarPad.ico` in `src\QuasarPad\Assets\`.
3. Open `installer\QuasarPad.iss` in [Inno Setup 6](https://jrsoftware.org/isinfo.php) → **Build → Compile**.
4. Output: `publish\Installer\QuasarPad-Setup-1.5.0.exe`

Default install directory: **`C:\Program Files\QuasarPad`** (admin required).

## Support

Free for everyone. How to help / optional donations: [SUPPORT.md](SUPPORT.md)

## License

MIT — see [LICENSE](LICENSE)
