# IK - 3D Line Visualizer

A Blazor WebAssembly app that renders 3D lines with node markers in the browser using Three.js. All logic is written in C#.

## Prerequisites

- [.NET SDK 10.0+](https://dotnet.microsoft.com/download) (install via `brew install dotnet` on macOS)

## Run (development)

```bash
export DOTNET_ROOT="/opt/homebrew/opt/dotnet/libexec"
dotnet watch run
```

Opens at **http://localhost:5172** (port may vary — check terminal output).

## Build (release)

```bash
dotnet publish -c Release -o publish
```

Output goes to `publish/wwwroot/` — serve with any static file server.

## Controls

- **Left-click + drag**: Orbit camera
- **Right-click + drag**: Pan camera
- **Scroll wheel**: Zoom in/out

## What you'll see

Two green lines connected at a shared point, with small yellow cubes marking the three nodes (two endpoints + one connection point). A grid on the ground plane provides spatial reference.
