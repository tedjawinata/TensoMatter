using System;
using Grasshopper.Kernel;

namespace TensoMatter
{
    public class TM_MaterialInfo : GH_Component
    {
        public TM_MaterialInfo()
          : base("TM Material Info",
                 "MatInfo",
                 "Ngeliat isi TM_Material biar nggak cuma keliatan type name doang.",
                 "TensoMatter",
                 "Materials")
        {
        }

        // ID bebas, yang penting unik di project kamu
        public override Guid ComponentGuid =>
            new Guid("2AC3DB2B-0154-4A5C-A9A1-0E7F8A6F9C22");

        // icon kosong dulu, nanti bisa diisi kalau mau
        protected override System.Drawing.Bitmap Icon => null;

        // ===== INPUT =====
        protected override void RegisterInputParams(GH_InputParamManager p)
        {
            // satu-satunya input: material yang mau dibongkar
            p.AddGenericParameter("Material", "Mat",
                "TM_Material dari TM_MaterialLibrary atau TM_CustomMaterial.",
                GH_ParamAccess.item);
        }

        // ===== OUTPUT =====
        protected override void RegisterOutputParams(GH_OutputParamManager p)
        {
            // keluaran dipisah biar gampang dipakai komponen lain
            p.AddTextParameter("Name", "Name", "Nama material.", GH_ParamAccess.item);
            p.AddNumberParameter("E", "E", "Modulus elastisitas.", GH_ParamAccess.item);
            p.AddNumberParameter("Thickness", "t", "Tebal material.", GH_ParamAccess.item);
            p.AddNumberParameter("Poisson", "ν", "Poisson ratio.", GH_ParamAccess.item);
            p.AddNumberParameter("Warp Stiff", "Warp", "Stiffness relatif arah warp.", GH_ParamAccess.item);
            p.AddNumberParameter("Weft Stiff", "Weft", "Stiffness relatif arah weft.", GH_ParamAccess.item);
            p.AddNumberParameter("Max Strain", "εmax", "Strain maksimum yang diijinkan.", GH_ParamAccess.item);
            p.AddNumberParameter("Min Radius", "Rmin", "Radius lengkung minimum.", GH_ParamAccess.item);

            // satu output teks biar bisa langsung colok ke Panel
            p.AddTextParameter("Summary", "S",
                "Ringkasan material dalam satu kalimat.",
                GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            object rawMat = null;
            if (!DA.GetData(0, ref rawMat))
                return;

            // cek dulu, kalau bukan TM_Material, kasih warning halus
            var mat = rawMat as TM_Material;
            if (mat == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                    "Yang masuk bukan TM_Material. Colok output dari TM_MaterialLibrary / TM_CustomMaterial ya.");
                return;
            }

            // kalau kamu sudah buat ToString() di TM_Material, ini akan pakai format itu
            string summary = mat.ToString();

            // biar di komponen juga kebaca, tulis nama di message kecil
            this.Message = mat.Name;

            // lempar satu-satu ke output
            DA.SetData(0, mat.Name);
            DA.SetData(1, mat.E);
            DA.SetData(2, mat.Thickness);
            DA.SetData(3, mat.Poisson);
            DA.SetData(4, mat.WarpStiff);
            DA.SetData(5, mat.WeftStiff);
            DA.SetData(6, mat.MaxStrain);
            DA.SetData(7, mat.MinRadius);
            DA.SetData(8, summary);
        }
    }
}
