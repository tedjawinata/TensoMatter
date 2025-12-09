using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace TensoMatter
{
    public class TM_MeshSetup : GH_Component
    {
        public TM_MeshSetup()
          : base("TM Mesh Setup",
                 "MeshSetup",
                 "Prepare mesh and edge data for tensile/winding solver based on material and warp/weft directions. | Siapkan mesh dan data edge untuk solver tensile/membrane berdasarkan material dan arah warp-vertikal/weft-horizontal.",
                 "TensoMatter",
                 "Tensile")
        {
        }

        public override Guid ComponentGuid =>
            new Guid("F2B6C0C4-1A24-4E86-9E87-3B4EB8F8C0AA");

        protected override System.Drawing.Bitmap Icon => null; // nanti bisa diisi icon

        // ===== INPUT =====
        protected override void RegisterInputParams(GH_InputParamManager p)
        {
            p.AddMeshParameter("Mesh", "M", "Base mesh for tensile/membrane", GH_ParamAccess.item);
            p.AddGenericParameter("Material", "Mat", "TM_Material from TM_MaterialLibrary", GH_ParamAccess.item);
            p.AddNumberParameter("Pretension", "Pt", "Pretension ratio (e.g. 0.05 = 5%)", GH_ParamAccess.item, 0.05);
            p.AddVectorParameter("Warp Direction", "Warp", "Global warp direction", GH_ParamAccess.item, Vector3d.XAxis);
            p.AddVectorParameter("Weft Direction", "Weft", "Global weft direction", GH_ParamAccess.item, Vector3d.YAxis);
        }

        // ===== OUTPUT =====
        protected override void RegisterOutputParams(GH_OutputParamManager p)
        {
            p.AddMeshParameter("MeshOut", "Mout", "Prepared mesh", GH_ParamAccess.item);
            p.AddLineParameter("Edges", "E", "Mesh edges as lines", GH_ParamAccess.list);
            p.AddNumberParameter("L0", "L0", "Original edge lengths", GH_ParamAccess.list);
            p.AddNumberParameter("Lrest", "Lr", "Rest edge lengths with pretension applied", GH_ParamAccess.list);
            p.AddNumberParameter("Kedge", "K", "Edge stiffness (warp/weft)", GH_ParamAccess.list);
            p.AddBooleanParameter("IsWarp", "Warp?", "True if edge aligned to warp, false if weft", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh mIn = null;
            TmMaterial mat = null;
            double pretension = 0.0;
            Vector3d warpDir = Vector3d.Unset;
            Vector3d weftDir = Vector3d.Unset;

            if (!DA.GetData(0, ref mIn)) return;
            if (!DA.GetData(1, ref mat) || mat == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Material is null or invalid.");
                return;
            }
            if (!DA.GetData(2, ref pretension)) return;
            if (!DA.GetData(3, ref warpDir)) return;
            if (!DA.GetData(4, ref weftDir)) return;

            if (!warpDir.IsValid || !weftDir.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Warp/Weft directions must be valid vectors.");
                return;
            }

            // clamp pretension agar tidak negatif / terlalu besar
            if (pretension < 0.0) pretension = 0.0;
            if (pretension > 0.5) pretension = 0.5; // 50% sudah ekstrem

            // Duplicate mesh untuk output (jangan ubah input langsung)
            Mesh m = mIn.DuplicateMesh();
            m.Normals.ComputeNormals();
            // MeshTopologyVertexList does not provide a SortVertices() method;
            // ordering is left as-is and topology indices are used directly.
            
            int nV = m.TopologyVertices.Count;
            int nE = m.TopologyEdges.Count;

            var edges = new List<Line>(nE);
            var L0 = new List<double>(nE);
            var Lrest = new List<double>(nE);
            var Kedge = new List<double>(nE);
            var isWarpList = new List<bool>(nE);

            // Normalisasi arah warp/weft
            warpDir.Unitize();
            weftDir.Unitize();

            for (int e = 0; e < nE; e++)
            {
                // ambil index topo vertices
                var topoPair = m.TopologyEdges.GetTopologyVertices(e);
                int viTopo = topoPair.I;
                int vjTopo = topoPair.J;

                // convert topo vert ke vert index biasa
                int vi = m.TopologyVertices.MeshVertexIndices(viTopo)[0];
                int vj = m.TopologyVertices.MeshVertexIndices(vjTopo)[0];

                Point3d pi = m.Vertices[vi];
                Point3d pj = m.Vertices[vj];

                Vector3d vec = pj - pi;
                double len = vec.Length;

                if (len < 1e-9)
                {
                    // edge degenerate
                    edges.Add(new Line(pi, pj));
                    L0.Add(0.0);
                    Lrest.Add(0.0);
                    Kedge.Add(0.0);
                    isWarpList.Add(true);
                    continue;
                }

                Vector3d dir = vec;
                dir.Unitize();

                // hitung alignment terhadap warp & weft
                double dotWarp = Math.Abs(Vector3d.Multiply(dir, warpDir));
                double dotWeft = Math.Abs(Vector3d.Multiply(dir, weftDir));

                bool isWarp = (dotWarp >= dotWeft);
                double k = isWarp ? mat.WarpStiff : mat.WeftStiff;

                double L0_e = len;
                double Lr_e = L0_e * (1.0 - pretension); // rest length yang lebih pendek

                edges.Add(new Line(pi, pj));
                L0.Add(L0_e);
                Lrest.Add(Lr_e);
                Kedge.Add(k);
                isWarpList.Add(isWarp);
            }

            // set output
            DA.SetData(0, m);
            DA.SetDataList(1, edges);
            DA.SetDataList(2, L0);
            DA.SetDataList(3, Lrest);
            DA.SetDataList(4, Kedge);
            DA.SetDataList(5, isWarpList);
        }
    }
}
