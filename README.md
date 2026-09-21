# QuasarPad

**Offline Text & Markup Toolkit** for Windows 10/11 (64-bit).

A lightweight editor with built-in converters and utilities — **no telemetry**, offline-first, **MIT License**.

<p align="center">
  <img src="assets/logo.svg" width="120" alt="QuasarPad logo"/>
</p>

> **Testers welcome.** Clone or download a Release, try the tools, and open an Issue if something breaks.

## Why QuasarPad?

| | Windows Notepad | QuasarPad |
|--|-----------------|-----------|
| Tabs | Limited / different UX | Multi-tab with close (×) |
| Clear text | Manual select-delete | One-click **Clear** |
| Markdown → HTML | No | Live preview + clean full HTML |
| Units / color / bases | No | Offline converters |
| Telemetry | OS-integrated | None |

Not competing with Notepad++ plugin ecosystem — focused on **offline utilities** next to a clean editor.

## Features (v1.5)

- Multi-tab editor, Clear, Find/Replace, Dark / Pure mode
- Markdown → HTML, HTML Test, Text Tools, Regex, Diff, Timestamps
- Unit converter, number bases, color palette, UUID & password generators
- JSON / YAML helpers, sort & unique lines

## Requirements

- Windows 10/11 **64-bit**
- [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) (for the recommended Small build)

## Build

```powershell
git clone https://github.com/freedomania/QuasarPad.git
cd QuasarPad
dotnet publish src\QuasarPad\QuasarPad.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o publish\Small
```

## Installer (Inno Setup)

You only need Inno Setup on the **developer** PC to *create* the setup.exe. End users just run the setup.

1. Publish to `publish\Small` (command above).
2. Install [Inno Setup 6](https://jrsoftware.org/isinfo.php).
3. Open `installer\QuasarPad.iss` → **Build → Compile**.
4. Output: `publish\Installer\QuasarPad-Setup-1.5.0.exe`

Default install path: `C:\QuasarPad` (desktop shortcut optional).

## Support

Free for everyone. Optional donations and how to help: [SUPPORT.md](SUPPORT.md)

## License

MIT — see [LICENSE](LICENSE)
