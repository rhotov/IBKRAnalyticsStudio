# Offline App Plan

Goal: turn IBKR Analytics Studio into an installable, local-first offline app while keeping statement data in the user's browser/device.

## First pass

- Add a web app manifest with app name, icons, theme colors, and standalone display mode.
- Add a service worker that precaches `index.html`, `assets/`, `src/`, and sample data needed for demo mode.
- Register the service worker from the app entry point with a graceful fallback when unsupported.
- Add an offline-ready local preview check for first load, reload, sample load, and share image generation.
- Keep all parsing and exports client-side; do not introduce remote APIs.

## Packaging options

- PWA: simplest path for browser install and offline use.
- Tauri: good desktop wrapper if native file integration is needed later.
- Electron: broadest desktop compatibility, but heavier runtime.

## Notes

- Current copied base version: `2.1.6`.
- Original project folder remains unchanged except for existing working-tree edits already present there.
