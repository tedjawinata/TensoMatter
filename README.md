**Tenso**Matter

**Tensile Form-Finding Tools** which **Material-Aware**

For Young Lecturer Research - Parahyangan Catholic University - Year One

Current Build:
https://www.figma.com/board/HKpc0Y2dkBE8jLnaw9OKCq/TensoMatter?node-id=1-219&t=mAUbMRvoqPhwH4bw-1


🧵 TensoMatter
A Grasshopper Plugin for Tensile, Membrane, and Fiber-Winding Design

TensoMatter is a parametric plugin for Rhino/Grasshopper focused on the simulation, generation, and analysis of tensile structures, membrane systems, and fiber-winding patterns. The project aims to provide architects, engineers, and researchers with a set of tools that connect material-aware behaviour, computational form-finding, and fabrication-oriented pattern generation.

TensoMatter is developed entirely in C# and VS Code, built as a lightweight .gha plugin for Grasshopper.

🎯 Key Features (Current & Upcoming)
✔️ Material Library (TM_MaterialLibrary)


A curated list of tensile, membrane, and fiber-based materials presented through a simple dropdown interface.
Each material entry includes:

- Elastic Modulus (E)

- Thickness

- Poisson Ratio

- Warp Stiffness

- Weft Stiffness

- Maximum Strain

- Minimum Bending Radius

This ensures that downstream solvers or form-finding methods consider actual material constraints, not just geometric targets.

✔️ Fiber Winding Generator (In Development)

Automatic generation of helical or multi-strand fiber winding patterns on surfaces such as cylinders, shells, or freeform geometries.


Planned capabilities:

- UV-based pattern generation

- Multi-strand & phase-shift logic

- Pitch, angle, and density control

- Cross-winding and hoop-winding variations

- Ready for future optimization & material coupling



✔️ Material-Aware Tensile Solver (Upcoming)

A physically-informed solver designed for:

- Simple membrane form-finding

- Anisotropic behaviour (warp–weft response)

- Pretension-based stress distribution

- Minimum curvature radius enforcement (e.g., bamboo strip behaviour)

- Output of stress maps & critical regions

- This module integrates directly with the Material Library for true material-aware design workflows.


🧩 Vision & Motivation

Modern tensile and membrane systems rely on more than geometry—they depend on materials, mechanics, and fabrication constraints.

TensoMatter proposes a workflow where:

- Material selection

- Pattern generation

- Form-finding

- Analysis & optimization

…happen inside the same parametric environment.


The goal is to support:

- academic research,

- prototype fabrication,

- rapid form experimentation,

- and material-driven design education.


🛠️ Technology Stack

- C#

- Visual Studio Code

- .NET Framework 4.8

- RhinoCommon (Rhino 8)

- Grasshopper SDK


The plugin is built using a clean file structure, modern C# coding style, and an extendable architecture.

📁 Folder Structure
TensoMatter/
  ├─ src/
  │   ├─ TM_MaterialLibrary.cs
  │   ├─ TM_Material.cs
  │   ├─ TM_WindingComponent.cs   (planned)
  │   ├─ TM_TensileSolver.cs      (planned)
  │   └─ Utilities/               (planned)
  ├─ TensoMatter.csproj
  ├─ README.md
  └─ LICENSE (optional)


🚀 Installation

Build the project using:

dotnet build -c Release


The build will automatically copy TensoMatter.gha to:

%AppData%\Grasshopper\Libraries\


Open Rhino → Launch Grasshopper → Locate components under:
TensoMatter → Materials


🤝 Contributions

TensoMatter welcomes feedback, collaboration, and research integration—especially in:

- computational form-finding

- natural fiber materials

- bamboo/biodegradable composites

- robotic winding workflows

- tensile/membrane engineering


📜 License

MIT License
