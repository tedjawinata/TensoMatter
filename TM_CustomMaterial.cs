using System;
using Grasshopper.Kernel;

namespace TensoMatter
{
    public class TM_CustomMaterial : GH_Component
    {
        public TM_CustomMaterial()
          : base("TM Custom Material",
                 "CustomMat",
                 "Bikin material versi kamu sendiri, nggak cuma dari library.",
                 "TensoMatter",
                 "Materials")
        {
        }

        // ID ini bebas, yang penting unik di project kamu
        public override Guid ComponentGuid =>
            new Guid("C6E0C0E1-9D7D-44D8-B1F6-4C7F1E0AC0F9");

        // icon bisa diisi nanti, sekarang kosong dulu
        protected override System.Drawing.Bitmap Icon => null;

        // ===== INPUT =====
        protected override void RegisterInputParams(GH_InputParamManager p)
        {
            // nama material, kalau males bisa dibiarkan default
            p.AddTextParameter("Name", "Name",
                "Nama material custom (misal: \"Membran Studio\", \"Bambu Eksperimen\").",
                GH_ParamAccess.item, "Custom Material");

            // beberapa angka dasar; satuan fleksibel, yang penting kamu konsisten di seluruh sistem
            p.AddNumberParameter("E", "E",
                "Modulus elastisitas. Pakai satuan yang kamu sepakati (MPa / Pa).",
                GH_ParamAccess.item, 1.0e9);

            p.AddNumberParameter("Thickness", "t",
                "Tebal material. Biasanya meter (misal 0.001 = 1 mm).",
                GH_ParamAccess.item, 0.001);

            p.AddNumberParameter("Poisson", "ν",
                "Poisson ratio. Biasanya di kisaran 0.2–0.45.",
                GH_ParamAccess.item, 0.3);

            p.AddNumberParameter("Warp Stiff", "Warp",
                "Relatif stiffness arah warp. Angka bebas, yang penting konsisten antar material.",
                GH_ParamAccess.item, 1.0);

            p.AddNumberParameter("Weft Stiff", "Weft",
                "Relatif stiffness arah weft.",
                GH_ParamAccess.item, 1.0);

            p.AddNumberParameter("Max Strain", "εmax",
                "Strain maksimum sebelum dianggap bahaya. Misal 0.05 = 5%.",
                GH_ParamAccess.item, 0.05);

            p.AddNumberParameter("Min Radius", "Rmin",
                "Radius lengkung minimum yang masih nyaman buat material ini.",
                GH_ParamAccess.item, 0.5);
        }

        // ===== OUTPUT =====
        protected override void RegisterOutputParams(GH_OutputParamManager p)
        {
            // keluarannya satu objek material; nanti dipakai TM_MeshSetup / solver lain
            p.AddGenericParameter("Material", "Mat",
                "Custom Material | Material custom yang siap dilempar ke komponen lain.",
                GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string name = null;
            double E = 0.0;
            double thick = 0.0;
            double nu = 0.0;
            double warp = 0.0;
            double weft = 0.0;
            double maxStrain = 0.0;
            double minRadius = 0.0;

            // ambil input pelan-pelan
            if (!DA.GetData(0, ref name)) return;
            if (!DA.GetData(1, ref E)) return;
            if (!DA.GetData(2, ref thick)) return;
            if (!DA.GetData(3, ref nu)) return;
            if (!DA.GetData(4, ref warp)) return;
            if (!DA.GetData(5, ref weft)) return;
            if (!DA.GetData(6, ref maxStrain)) return;
            if (!DA.GetData(7, ref minRadius)) return;

            // sedikit sanity check biar nggak terlalu aneh
            if (E < 0) E = 0;
            if (thick < 0) thick = 0;
            if (maxStrain < 0) maxStrain = 0;
            if (minRadius < 0) minRadius = 0;

            // kalau lupa ngisi nama, isi aja biar nggak kosong
            if (string.IsNullOrWhiteSpace(name))
                name = "Custom Material";

            // bikin instance material; tipe ini sama kayak yang dipakai TM_MaterialLibrary
            var mat = new TM_Material
            {
                Name       = name,
                E          = E,
                Thickness  = thick,
                Poisson    = nu,
                WarpStiff  = warp,
                WeftStiff  = weft,
                MaxStrain  = maxStrain,
                MinRadius  = minRadius
            };

            // kecil di bawah komponen, biar kelihatan lagi pakai material apa
            this.Message = name;

            // lempar ke output
            DA.SetData(0, mat);
        }
    }

    // Simple material container used by TM_CustomMaterial and other project components.
    // Add or replace with the real implementation if TM_Material exists elsewhere.
    public class TM_Material
    {
        public string Name { get; set; }
        public double E { get; set; }
        public double Thickness { get; set; }
        public double Poisson { get; set; }
        public double WarpStiff { get; set; }
        public double WeftStiff { get; set; }
        public double MaxStrain { get; set; }
        public double MinRadius { get; set; }

        public TM_Material()
        {
            Name = "Custom Material";
            E = 0.0;
            Thickness = 0.0;
            Poisson = 0.0;
            WarpStiff = 1.0;
            WeftStiff = 1.0;
            MaxStrain = 0.0;
            MinRadius = 0.0;
        }
    }
}
