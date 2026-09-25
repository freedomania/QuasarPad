# QuasarPad — User Guide

Offline text & markup toolkit for **Windows 10/11 (64-bit)**.
English UI. No telemetry. No account required.

**Download:** [Latest Release](https://github.com/freedomania/QuasarPad/releases/latest)

---

## 1. Install & first run

1. Install the [.NET 8 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/8.0) if needed.
2. Run **QuasarPad-Setup-….exe** from [Releases](https://github.com/freedomania/QuasarPad/releases).
3. Default install path: `C:\Program Files\QuasarPad`.
4. Start QuasarPad from the Start Menu or desktop shortcut.

Settings (theme, font, word wrap, window size) are saved under your user profile (`%AppData%\QuasarPad\`), not inside Program Files.

---

## 2. Editor basics

| Action | How |
|--------|-----|
| **New tab** | Toolbar **New Tab**, or File → New Tab (`Ctrl+N`) |
| **Close tab** | Click **×** on the tab, or File → Close Tab (`Ctrl+W`) |
| **Clear text** | Toolbar **Clear**, Edit → Clear, or right-click → Clear (empties the current tab only) |
| **Open file** | File → Open (`Ctrl+O`) — opens in a **new tab** |
| **Save / Save As** | `Ctrl+S` / `Ctrl+Shift+S` |
| **Cut / Copy / Paste** | Menu, shortcuts, or right-click on the editor |
| **Find / Replace** | Edit → Find (`Ctrl+F`) / Replace (`Ctrl+H`) |
| **Word wrap** | View → Word Wrap |
| **Line numbers** | View → Line Numbers |
| **Status bar** | View → Status Bar |
| **Pure Mode** | View → Pure Mode (simple notepad-style status) |
| **Dark Mode** | View → Dark Mode |

**Note:** Tool windows open **empty**. Paste with `Ctrl+V` or right-click if you need text from elsewhere. They do not auto-fill from the clipboard or the main editor.

---

## 3. Tools menu

### Markdown → HTML
**Tools → Markdown to HTML…**

- Paste or type Markdown on one side.
- See a live preview.
- Output is a **clean full HTML document** (`<!DOCTYPE html>`, `<html>`, `<body>`, …).
- Copy the HTML for use in blogs, docs, or email-friendly markup workflows.

### HTML Test
**Tools → HTML Test…**

- Paste HTML and preview it offline.
- Useful for quick checks without a browser project setup.

### Text Tools
**Tools → Text Tools…**

Common transforms, for example:

- Base64 encode / decode
- URL encode / decode
- Upper / lower / title case
- MD5 / SHA256 hashes
- Reverse text, remove empty lines

Work stays on your PC (offline).

### Regex Tester
**Tools → Regex Tester…**

- Test regular expressions against sample text.
- Handy when building search patterns before using them elsewhere.

### Diff
**Tools → Diff…**

- Compare two text blocks side by side.
- Paste left and right texts manually.

### Timestamp Tools
**Tools → Timestamp Tools…**

- Convert or inspect timestamps (offline helpers for date/time values).

### Unit Converter
**Tools → Unit Converter…**

- Convert using **fixed offline constants** (no internet).
- Categories such as temperature, length, weight, data size, area, volume, speed, angle, time, pressure, energy, power (and related units available in the app).

### Number Base
**Tools → Number Base…**

- Convert between binary, octal, decimal, and hexadecimal.

### Color Tools
**Tools → Color Tools…**

- Click a palette color to fill **HEX** and **RGB**.
- Use **Copy HEX** / **Copy RGB**.
- Values come from the offline palette (no network).

### Generators (UUID / Password)
**Tools → Generators (UUID/Password)…**

- Generate UUIDs and passwords locally.
- Copy results into your documents or configs.

---

## 4. Tools that change the current tab

These run on the **current editor tab** (they do not open a separate window):

| Menu item | Effect |
|-----------|--------|
| **JSON Format** | Pretty-print JSON (shows a warning if invalid) |
| **YAML Clean** | Light cleanup / formatting for YAML-like text |
| **Format Text** | Normalize plain text spacing / structure |
| **Reflow Paragraphs** | Wrap long paragraphs (e.g. ~80 columns) |
| **Make Slug** | Turn selection or whole text into a URL-friendly slug |
| **Sort Lines** | Sort lines alphabetically |
| **Unique Lines** | Remove duplicate lines |
| **Sort + Unique** | Sort and remove duplicates |

---

## 5. Settings

**Tools → Settings…**

Adjust options such as theme-related preferences, font, and editor behavior (depending on version). Changes are stored in your user AppData folder.

---

## 6. Help & support

| Item | Purpose |
|------|---------|
| **Help → About QuasarPad** | Version and license summary |
| **Help → Open on GitHub** | Project repository |
| **Help → Support the Project** | Ways to help (star, issues) |

- Report bugs: [Issues](https://github.com/freedomania/QuasarPad/issues) with label `bug` when possible  
- General feedback: label `feedback`  
- More on supporting the project: [SUPPORT.md](../SUPPORT.md)

---

## 7. Tips

1. **Offline-first** — converters and tools do not need the internet.
2. **One tab = one document** — unsaved tabs show `*` in the title.
3. **Clear ≠ New file** — Clear only empties the current tab; use Save if you need a file on disk.
4. **Installer vs portable** — official Release is the Setup installer; building from source can produce a folder under `publish\Small`.
5. **Icon / shortcuts** — if Windows shows an old icon, refresh Explorer or reinstall from the latest Setup.

---

## 8. License

MIT — see [LICENSE](../LICENSE).
