# TensoMatter Coding Instructions

## Project Overview
**TensoMatter** is a .NET 4.8 Grasshopper plugin for Rhino 7/8 that provides material property selection and management for tensile membrane/fabric analysis. It's a lightweight library focused on materials with specific mechanical properties (E, thickness, Poisson ratio, anisotropic stiffness).

**Architecture**: Single-assembly plugin built as a `.gha` file (Grasshopper add-in) deployed to `%AppData%\Grasshopper\Libraries\`.

## Key Components

### Material System
- **`TmMaterial.cs`**: Core data container with mechanical properties (Young's modulus `E`, thickness, Poisson ratio, warp/weft stiffness, strain limits, minimum radius)
- **`TM_MaterialLibrary.cs`**: Grasshopper UI component exposing a dropdown selector to choose from 6 predefined materials (PVC, PTFE, ETFE, Bamboo, Rami-Ijuk, Steel Cable)
- Material data flows from component output to downstream Grasshopper nodes—always output as `TM_Material` instance

### Inheritance Pattern
All user-facing components inherit from `GH_Component` (Grasshopper.Kernel):
```csharp
public class TM_MaterialLibrary : GH_Component
{
    // Must override: ComponentGuid (unique ID), RegisterInputParams, RegisterOutputParams, SolveInstance
}
```
Dropdown menus are appended via `AppendAdditionalComponentMenuItems()` using `Menu_AppendItem()`.

## Build & Deployment Workflow

**Build command**: `dotnet build TensoMatter.csproj -c Debug`  
**Output**: `bin/Debug/net48/TensoMatter.dll` → auto-copied to `.gha` via MSBuild PostBuild target

**PostBuild target** (in `.csproj`):
- Reads `%AppData%\Grasshopper\Libraries\`
- Copies `.dll` as `TensoMatter.gha` with `OverwriteReadOnlyFiles=true`
- Rhino/Grasshopper auto-detects `.gha` files—restart Grasshopper to reload plugin

**Dependencies**: Hard-coded paths to Rhino 8 system DLLs:
- `C:\Program Files\Rhino 8\System\RhinoCommon.dll`
- `C:\Program Files\Rhino 8\Plug-ins\Grasshopper\Grasshopper.dll`

⚠️ If building on different machine, verify Rhino 8 installation path or update `.csproj` reference paths.

## Development Conventions

### Namespace & Naming
- Single namespace: `TensoMatter`
- Component classes use TM_ prefix: `TM_MaterialLibrary`
- Data classes use Tm prefix: `TmMaterial`
- Property names: English, CamelCase (e.g., `WarpStiff`, `MinRadius`)
- Comments in Indonesian—preserve existing style

### Component Pattern
1. Dropdown state stored in private field (e.g., `currentMaterial`)
2. `SolveInstance()` fetches data from enum and outputs via `DA.SetData()`
3. Component message (small text below icon) shows active selection: `this.Message = mat.Name`
4. Use `ExpireSolution(true)` when user changes dropdown to trigger recompute

### Material Property Convention
Materials define mechanical behavior via structure:
- `E`: Elastic modulus (Pa or MPa—unit consistency is developer responsibility)
- `WarpStiff`, `WeftStiff`: Directional stiffness factors (often 0.8–3.0 range for anisotropic fabrics)
- `MaxStrain`: Failure threshold (0.02–0.20 range depending on material)
- `MinRadius`: Geometric constraint for minimum curvature

## Testing & Validation
- No automated tests currently in repo
- Manual testing: Open Grasshopper, drag `TM_MaterialLibrary` component onto canvas, verify dropdown works and outputs expected material
- Validate output via Grasshopper's parameter inspector

## Key Files Reference
| File | Purpose |
|------|---------|
| `TensoMatter.csproj` | Build config, Rhino/Grasshopper references, PostBuild deployment |
| `TM_MaterialLibrary.cs` | Main UI component (dropdown menu selector) |
| `TmMaterial.cs` | Data model for material properties |

## Common Tasks

### Add a New Material
1. Add enum value in `TM_MaterialLibrary.MaterialType`
2. Add case in `GetMaterial()` method with properties
3. Add menu item in `AppendAdditionalComponentMenuItems()` using `AddMenu()`
4. Rebuild & restart Grasshopper

### Modify Component Output
- Edit `RegisterOutputParams()` to add/remove parameters
- Update `SolveInstance()` to set corresponding data
- Component GUID must remain unique (don't change unless creating new component variant)

### Debug Plugin
- Use VS Code debugger with `.vscode/launch.json` configured
- Attach to Rhino.exe process
- Set breakpoints in component code, trigger Grasshopper compute
