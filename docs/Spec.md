# QuasarPad - Product Specification v1.0

## Overview
QuasarPad is a clean, lightweight Notepad alternative for Windows 10/11.
- Pure Mode (default): minimal like classic Windows 10 Notepad
- Smart features available when needed
- No auto-save / session restore by default
- Fully offline, no telemetry

## Name
**QuasarPad**

## Monetization
- Completely free to use
- Optional donations (GitHub Sponsors, Ko-fi, PromptPay)
- Optional "Supporter" tier (one-time or monthly) for extras

## Distribution
1. **Portable (Self-contained)** - Recommended, works immediately
2. **Installer** - Creates Start Menu + Desktop shortcut
3. Framework-dependent version (optional, smaller size)

## Core Features (Must Have)
- Pure Mode as default
- Open / Save any text-based file
- No auto-save by default
- Markdown → HTML Converter with Live Preview + clean full HTML output
- Line numbers + Status bar
- Encoding detection & display
- Dark / Light mode
- Find / Replace
- Word Wrap
- Portable support

## Technology
- C# + WPF + .NET 8
- Markdig for Markdown
- AvalonEdit for advanced editing (planned)

## Version
1.0.0 (Initial development)
