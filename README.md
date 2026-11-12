# MVVMPhoto

A fast, native, free and open-source photo viewer for Windows, built with WinUI 3.

The goal: open photos instantly, browse folders of thousands of images without stutter, and stay out of
your way. No account, no cloud and no ads. The viewer and library come first; editing tools come later.

> **Status:** early groundwork. Nothing usable yet.

## Building

Requirements:

- Windows 10 1809 (build 17763) or later. Windows 11 is recommended.
- Visual Studio 2026 with the **WinUI application development** workload, or the .NET 10 SDK.

Open `MVVMPhoto.slnx` in Visual Studio, pick the `x64` platform, and press F5. The app runs as a packaged
(MSIX) app.

## How the code is organised

- **MVVM, strictly.** Views are XAML. View models hold state and commands and never touch UI types.
- **XAML-first UI.** Controls, layouts and templates are declared in `.xaml`. Code supplies data and
  behaviour, not elements.
- **Dependency injection** for every service, and **structured logging** throughout.
- Package versions are pinned in one place: `Directory.Packages.props`.

## Licence

MVVMPhoto is free software under the [GNU General Public License v3.0 or later](LICENSE.txt).
