# IK Project — AI Agent Reference

## Project Overview

Blazor WebAssembly app for 3D line/node visualization in the browser. C# logic runs via WASM, Three.js handles WebGL rendering through JS interop.

## Tech Stack

- **Runtime**: Blazor WebAssembly (.NET 10, `net10.0`)
- **3D Rendering**: Three.js r128 (loaded via CDN in `index.html`)
- **Camera Controls**: Three.js OrbitControls (CDN)
- **JS Interop**: `IJSRuntime` from Blazor calls into `window.scene3d` object

## Directory Structure

```
ik/
├── ik.csproj              # Project file — Blazor WASM SDK
├── Program.cs             # App entry point (no changes needed usually)
├── App.razor              # Router setup
├── _Imports.razor         # Global using directives for Razor
├── IK/
│   ├── Vec3.cs            # 3D vector struct with math operators
│   └── TwoBoneIk.cs       # Two-bone IK solver (static class + result struct)
├── Layout/
│   └── MainLayout.razor   # Minimal layout — just renders @Body
├── Pages/
│   ├── Home.razor         # Main page — IK leg sim, calls JS interop
│   └── NotFound.razor     # 404 page
├── wwwroot/
│   ├── index.html         # HTML shell — loads Three.js, OrbitControls, scene.js
│   └── js/
│       └── scene.js       # JS bridge — scene3d object with init/addLine/addNode/clearScene
└── CLAUDE.md              # This file
```

## Architecture: How C# Talks to Three.js

1. `Home.razor` defines joints as `Vec3`, solves IK, and calls `JS.InvokeVoidAsync(...)` in `OnAfterRenderAsync`
2. These calls invoke methods on the `window.scene3d` object defined in `wwwroot/js/scene.js`
3. `scene.js` manages the Three.js scene, camera, renderer, and OrbitControls

## JS Interop API (`window.scene3d`)

| Method | Params | Description |
|--------|--------|-------------|
| `init(canvasId)` | string | Creates renderer, scene, camera, controls, grid, lights. Starts render loop. |
| `addLine(x1,y1,z1, x2,y2,z2, colorHex)` | 6 floats + int | Adds a line segment between two 3D points |
| `addNode(x,y,z, colorHex, size)` | 3 floats + int + float | Adds a cube (box mesh) at a 3D point |
| `clearScene()` | none | Removes all lines and meshes (keeps grid/lights) |

## How to Add New Geometry

In `Home.razor`, add more `JS.InvokeVoidAsync` calls in `OnAfterRenderAsync`:

```csharp
// Add a line
await JS.InvokeVoidAsync("scene3d.addLine", x1, y1, z1, x2, y2, z2, 0xff0000);

// Add a node marker
await JS.InvokeVoidAsync("scene3d.addNode", x, y, z, 0xffdd00, 0.15f);
```

To add new JS-side capabilities, add methods to the `scene3d` object in `wwwroot/js/scene.js`.

## Build & Run Commands

```bash
export DOTNET_ROOT="/opt/homebrew/opt/dotnet/libexec"
dotnet watch run          # Dev server with hot reload
dotnet build              # Build only
dotnet publish -c Release # Publish static output
```

## IK Solver

- **Two-bone IK** in `IK/TwoBoneIk.cs` — solves Hip -> Knee -> Foot chain
- **Vec3** in `IK/Vec3.cs` — lightweight 3D vector struct with operators
- Bone lengths: thigh = 1.8, shin = 2.0
- Pole vector `(0,0,1)` controls knee bend direction (+Z = forward)
- Red debug line (0.5 length) drawn from knee in pole vector direction

## Key Bindings

| Key | Action |
|-----|--------|
| Arrow Up / Down | Move hip in -Z / +Z |
| Arrow Left / Right | Move hip in -X / +X |
| U | Move hip up (+Y) |
| D | Move hip down (-Y) |

## Key Conventions

- All 3D math/logic stays in C# (Razor pages)
- JS layer is a thin rendering bridge only — no logic in JS
- Colors are hex integers (e.g., `0x00cc66` for green)
- Coordinate system: Y-up (Three.js default)
