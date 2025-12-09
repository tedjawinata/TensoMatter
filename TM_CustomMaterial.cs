using System;
using Grasshopper.Kernel;

namespace TensoMatter
{
    public class TM_CustomMaterial : GH_Component
    {
        public TM_CustomMaterial()
          : base("TM Custom Material",
                 "CustomMat",
                 "Bikin material versi kamu sendiri tanpa harus ngikutin preset library.",
                 "TensoMatter",
                 "Materials")
        {
        }

        public override Guid ComponentGuid =>
            new Guid("A1EFC7D9-92CF-41E2-A997-998A7CBB7012");

        protected override System.Drawing.Bitmap Icon => null; // icon nanti gampang ditambah


        // ===== INPUT =====
        protected override void RegisterInputParams(GH_InputParamManager p)
        {
            p.AddTextParameter(
                "Name", "Name",
                "Nama material custom. Bebas banget, mau formal atau nyeleneh juga boleh.",
                GH_ParamAccess.item, "Custom Material");

            p.AddNumberParameter("E", "E",
                "Modulus elastisitas. Angkanya terserah standar kamu.",
                GH_ParamAccess.item, 1.0e9);

            p.AddNumberParameter("Thickness", "t",
                "Tebal material (m). Biasanya 0.0002–0.01 tergantung jenisnya.",
                GH_ParamAccess.item, 0.001);

            p.AddNumberParameter("Poisson", "ν",
                "Poisson ratio (0.2–0.45 biasanya).",
                GH_ParamAccess.item, 0.3);

            p.AddNumberParameter("Warp Stiff", "Warp",
                "Kekakuan arah warp. Skala relatif aja.",
                GH_ParamAccess.item, 1.0);

            p.AddNumberParameter("Weft Stiff", "Weft",
                "Kekakuan arah weft.",
                GH_ParamAccess.item, 1.0);

            p.AddNumberParameter("Max Strain", "εmax",
                "Strain maksimum sebelum dianggap bahaya. Contoh: 0.1 = 10%.",
                GH_ParamAccess.item, 0.05);

            p.AddNumberParameter("Min Radius", "Rmin",
                "Radius lengkung minimum yang masih aman buat material ini.",
                GH_ParamAccess.item, 0.5);
        }


        // ===== OUTPUT =====
        protected override void RegisterOutputParams(GH_OutputParamManager p)
        {
            p.AddGenericParameter(
                "Material", "Mat",
                "Material custom yang kamu racik sendiri.",
                GH_ParamAccess.item);
        }


        // ===== LOGIKA =====
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string name = null;
            double E = 0, thick = 0, nu = 0, warp = 0, weft = 0, maxStrain = 0, minRadius = 0;

            if (!DA.GetData(0, ref name)) return;
            DA.GetData(1, ref E);
            DA.GetData(2, ref thick);
            DA.GetData(3, ref nu);
            DA.GetData(4, ref warp);
            DA.GetData(5, ref weft);
            DA.GetData(6, ref maxStrain);
            DA.GetData(7, ref minRadius);

            // Ngalahin input aneh sedikit
            if (string.IsNullOrWhiteSpace(name))
                name = "Custom Material";

            if (E < 0) E = 0;
            if (thick < 0) thick = 0;
            if (maxStrain < 0) maxStrain = 0;
            if (minRadius < 0) minRadius = 0;

            // bikin materialnya
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

            // biar komponen kelihatan pakai material apa
            this.Message = name;

            // keluarin ke output
            DA.SetData(0, mat);
        }
    }

    // Simple material DTO so the component compiles if TM_Material isn't defined elsewhere.
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
    }
}
