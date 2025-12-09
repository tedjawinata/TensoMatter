using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Grasshopper.Kernel;

namespace TensoMatter
{
    public class TM_MaterialLibrary : GH_Component
    {
        // ===== Dropdown List (enum) =====
        private enum MaterialType
        {
            PVC,
            PTFE,
            ETFE,
            BambooStrip,
            RamiIjuk,
            SteelCable
        }

        // Simpan material aktif saat ini
        private MaterialType currentMaterial = MaterialType.PVC;

        public TM_MaterialLibrary()
        : base("TM Material Library",
               "MatLib",
               "Material selector with dropdown | Material yang sudah di prasiapkan untuk dipakai langsung.",
               "TensoMatter",
               "Materials")
        { }

        public override Guid ComponentGuid =>
            new Guid("555B1E67-45E4-417B-AB55-1D998B834F11");

        protected override System.Drawing.Bitmap Icon => null;



        // ===== Tidak ada input =====
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        { }

        // ===== Output material =====
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Material", "Mat", "Selected material", GH_ParamAccess.item);
        }



        // ===== Material Properties =====
        private TmMaterial GetMaterial(MaterialType t)
        {
            switch (t)
            {
                case MaterialType.PVC:
                    return new TmMaterial {
                        Name="PVC Membrane",
                        E=1.0e9,
                        Thickness=0.001,
                        Poisson=0.30,
                        WarpStiff=1.0,
                        WeftStiff=0.8,
                        MaxStrain=0.10,
                        MinRadius=0.50
                    };

                case MaterialType.PTFE:
                    return new TmMaterial {
                        Name="PTFE Fabric",
                        E=2.5e9,
                        Thickness=0.0008,
                        Poisson=0.25,
                        WarpStiff=1.2,
                        WeftStiff=1.0,
                        MaxStrain=0.05,
                        MinRadius=1.00
                    };

                case MaterialType.ETFE:
                    return new TmMaterial {
                        Name="ETFE Foil",
                        E=1.5e9,
                        Thickness=0.00025,
                        Poisson=0.40,
                        WarpStiff=1.0,
                        WeftStiff=1.0,
                        MaxStrain=0.20,
                        MinRadius=0.30
                    };

                case MaterialType.BambooStrip:
                    return new TmMaterial {
                        Name="Bamboo Strip",
                        E=10e9,
                        Thickness=0.010,
                        Poisson=0.20,
                        WarpStiff=2.0,
                        WeftStiff=1.5,
                        MaxStrain=0.02,
                        MinRadius=1.50
                    };

                case MaterialType.RamiIjuk:
                    return new TmMaterial {
                        Name="Rami–Ijuk Fiber",
                        E=2e9,
                        Thickness=0.002,
                        Poisson=0.30,
                        WarpStiff=1.1,
                        WeftStiff=0.9,
                        MaxStrain=0.08,
                        MinRadius=0.40
                    };

                case MaterialType.SteelCable:
                    return new TmMaterial {
                        Name="Steel Cable",
                        E=200e9,
                        Thickness=0.010,
                        Poisson=0.30,
                        WarpStiff=3.0,
                        WeftStiff=3.0,
                        MaxStrain=0.03,
                        MinRadius=0.20
                    };

                default:
                    return new TmMaterial { Name="Generic" };
            }
        }



        // ===== OUTPUT =====
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            TmMaterial mat = GetMaterial(currentMaterial);
            this.Message = mat.Name;   // Teks kecil di bawah komponen
            DA.SetData(0, mat);
        }



        // ===== DROPDOWN MENU =====
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);

            AddMenu(menu, "PVC Membrane", MaterialType.PVC);
            AddMenu(menu, "PTFE Fabric", MaterialType.PTFE);
            AddMenu(menu, "ETFE Foil", MaterialType.ETFE);
            AddMenu(menu, "Bamboo Strip", MaterialType.BambooStrip);
            AddMenu(menu, "Rami–Ijuk", MaterialType.RamiIjuk);
            AddMenu(menu, "Steel Cable", MaterialType.SteelCable);
        }

        private void AddMenu(ToolStripDropDown menu, string text, MaterialType type)
        {
            var item = Menu_AppendItem(menu, text, OnMaterialSelected, true, type == currentMaterial);
            item.Tag = type;
        }

        private void OnMaterialSelected(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem mi && mi.Tag is MaterialType mt)
            {
                currentMaterial = mt;
                ExpireSolution(true);
            }
        }
    }
}
