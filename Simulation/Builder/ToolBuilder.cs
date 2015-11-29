using Geometry;
using Mesh;
using Simulation.Persistence;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoObjectStuff;
using _3DMeshStructureLib.HalfEdgeStructure3D;

namespace Builder
{
    class ToolBuilder : GeoObjectBuilder
    {
        XMLToolReader m_ToolReader;
        internal ToolBuilder(XMLToolReader r)
        {
            m_ToolReader = r;
        }

        internal Tool BuildTool()
        {
            Tool t =  CreateNewTool();
            BuildMesh(t, null, null);
            t.CreateVertexArray();
            AddMaterial(t, true);
            m_geoObjectList.Add(t);
            return t;
        }

        protected override void BuildMesh(GeoObject o, Del3D meshBuilder, params object[] oparams)
        {
            Tool t = (Tool)o;
            Mesh3 pMesh = new Mesh3();

            t.Name = m_ToolReader.ReadName();
            List<Vector3D> l = m_ToolReader.ReadVertices();
            foreach (Vector3D v in l)
                pMesh.AddVertex(v.X, v.Y, v.Z);

            List<int[]> polyIndizes;
            List<int[]> ni; // currently not used
            m_ToolReader.ReadPolys(out polyIndizes, out ni);
            List<int> cuttingEdgePolys = m_ToolReader.ReadCuttingEdgePolyIndizes();

            for (int i = 0; i < polyIndizes.Count; i++) // for each polygon
            {
                int nindex = pMesh.AddNormal(Vector3D.PlaneNormal(l[polyIndizes[i][0]], l[polyIndizes[i][1]], l[polyIndizes[i][2]]).Unit());
                int[] normalIndizes = new int[polyIndizes[i].Length];
                for (int j = 0; j < normalIndizes.Length; j++) normalIndizes[j] = nindex;
                int findex = pMesh.AddPoly(polyIndizes[i], normalIndizes);
                if(findex == -1)
                    throw new Exception("Poly index not valid");
                if (cuttingEdgePolys.Contains(i))
                    t.CuttingEdgePolyIndices.Add(findex);
            }
            t.Mesh = pMesh;
        }
    }
}
