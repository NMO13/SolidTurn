using Mesh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;
using MeshStructuresLib.HalfEdgeStructure2D;
using _3DMeshStructureLib.HalfEdgeStructure3D;
namespace Simulation
{
    public class TemplateMeshes
    {
        public static RoughPartConcentricMesh3D Quad(params object[] o)
        {
            double length = (double)o[0];
            double radius = (double)o[2];
            int offset = (int)o[3];
            RoughPartConcentricMesh3D mesh = new RoughPartConcentricMesh3D();

            BVHPolyMesh2D poly2D = new BVHPolyMesh2D();
            List<HEVector2> list = new List<HEVector2>();
            list.Add(new HEVector2(0 + offset, 0));
            list.Add(new HEVector2(length + offset, 0));
            list.Add(new HEVector2(length + offset, radius));
            list.Add(new HEVector2(0 + offset, radius));

            poly2D.CreateMeshFromVertices(list);
            poly2D.CreateBVH(19, 1);
            mesh.Poly2D = poly2D;
            return mesh;
        }

        public static RoughPartConcentricMesh3D SimpleChawJuckCylinder(params object[] o)
        {
            //double length = (double)o[0];
            //double radius = (double)o[2];

            double length = -120;
            double radius = 150;
            RoughPartConcentricMesh3D mesh = new RoughPartConcentricMesh3D();
            BVHPolyMesh2D poly2D = new BVHPolyMesh2D();
            List<HEVector2> list = new List<HEVector2>();
            list.Add(new HEVector2(length, 50));
            list.Add(new HEVector2(0, 50));
            list.Add(new HEVector2(0, radius));
            list.Add(new HEVector2(length, radius));

            poly2D.CreateMeshFromVertices(list);
            poly2D.CreateBVH(19, 1);
            mesh.Poly2D = poly2D;
            return mesh;
        }

        public static RoughPartConcentricMesh3D RotatingJaw(params object[] o)
        {
            double length = 45;
            RoughPartConcentricMesh3D mesh = new RoughPartConcentricMesh3D();
            BVHPolyMesh2D poly2D = new BVHPolyMesh2D();
            List<HEVector2> list = new List<HEVector2>();
            list.Add(new HEVector2(0, 30));
            list.Add(new HEVector2(length, 30));
            list.Add(new HEVector2(length, 60));
            list.Add(new HEVector2(length-15, 60));
            list.Add(new HEVector2(length-15, 110));
            list.Add(new HEVector2(length - 30, 110));
            list.Add(new HEVector2(length - 30, 150));
            list.Add(new HEVector2(0, 150));

            poly2D.CreateMeshFromVertices(list);
            mesh.Poly2D = poly2D;
            return mesh;
        }

        public static Mesh3 Chuck(params object[] o)
        {
            Mesh3 mesh = new Mesh3();
            int v0 = mesh.AddVertex(0, 0, 0);
            int v1 = mesh.AddVertex(40, 0, 0);
            int v2 = mesh.AddVertex(40, 15, 0);
            int v3 = mesh.AddVertex(0, 15, 0);
            int v4 = mesh.AddVertex(90, 0, 0);
            int v5 = mesh.AddVertex(90, 30, 0);
            int v6 = mesh.AddVertex(40, 30, 0);
            int v7 = mesh.AddVertex(110, 0, 0);
            int v8 = mesh.AddVertex(110, 45, 0);
            int v9 = mesh.AddVertex(90, 45, 0);

            int v12 = mesh.AddVertex(90, 45, -30);
            int v13 = mesh.AddVertex(90, 30, -30);
            int v14 = mesh.AddVertex(40, 30, -30);
            int v15 = mesh.AddVertex(40, 15, -30);
            int v16 = mesh.AddVertex(0, 15, -30);

            int v17 = mesh.AddVertex(90, 0, -30);
            int v18 = mesh.AddVertex(40, 0, -30);
            int v19 = mesh.AddVertex(0, 0, -30);

            int v10 = mesh.AddVertex(110, 0, -30);
            int v11 = mesh.AddVertex(110, 45, -30);

            int v20 = mesh.AddVertex(120, 45, -20);
            int v21 = mesh.AddVertex(120, 45, -10);
            int v22 = mesh.AddVertex(120, 0, -10);
            int v23 = mesh.AddVertex(120, 0, -20);



            int n0 = mesh.AddNormal(0, 0, 1);
            int[] poly0 = { v0, v1, v2, v3 };
            int[] normal0 = { n0, n0, n0, n0 };
            mesh.AddPoly(poly0, normal0);

            int[] poly1 = { v1, v4, v5, v6 };
            mesh.AddPoly(poly1, normal0);

            int[] poly2 = { v4, v7, v8, v9 };
            mesh.AddPoly(poly2, normal0);


            int n1 = mesh.AddNormal(1, 0, 0);
            int[] poly3 = { v21, v22, v23, v20 };
            int[] normal1 = { n1, n1, n1, n1 };
            mesh.AddPoly(poly3, normal1);

            int n3 = mesh.AddNormal(0, 1, 0);
            int[] poly4 = { v9, v8, v11, v12 };
            int[] normal3 = { n3, n3, n3, n3 };
            mesh.AddPoly(poly4, normal3);

            int[] poly5 = { v6, v5, v13, v14 };
            mesh.AddPoly(poly5, normal3);

            int[] poly6 = { v3, v2, v15, v16 };
            mesh.AddPoly(poly6, normal3);



            int n2 = mesh.AddNormal(0, 0, -1);
            int[] normal2 = { n2, n2, n2, n2 };
            int[] poly7 = { v12, v11, v10, v17 };
            mesh.AddPoly(poly7, normal2);

            int[] poly8 = { v14, v13, v17, v18 };
            mesh.AddPoly(poly8, normal2);

            int[] poly9 = { v16, v15, v18, v19 };
            mesh.AddPoly(poly9, normal2);

            int n4 = mesh.AddNormal(-1, 0, 0);
            int[] normal4 = { n4, n4, n4, n4 };
            int[] poly10 = { v9, v12, v13, v5 };
            mesh.AddPoly(poly10, normal4);

            int[] poly11 = { v6, v14, v15, v2 };
            mesh.AddPoly(poly11, normal4);

            int[] poly12 = { v0, v3, v16, v19 };
            mesh.AddPoly(poly12, normal4);

            int n5 = mesh.AddNormal(0, -1, 0);
            int[] normal5 = { n5, n5, n5, n5 };
            int[] poly13 = { v0, v19, v10, v7 };
            mesh.AddPoly(poly13, normal5);

            int[] poly14 = { v8, v7, v22, v21 };
            int n6 = mesh.AddNormal(new Vector3D(1, 0, 1).Unit());
            int[] normal6 = { n6, n6, n6, n6 };
            mesh.AddPoly(poly14, normal6);

            int[] poly15 = { v11, v20, v23, v10 };
            int n7 = mesh.AddNormal(new Vector3D(1, 0, -1).Unit());
            int[] normal7 = { n7, n7, n7, n7 };
            mesh.AddPoly(poly15, normal2);

            int[] poly16 = { v8, v21, v20, v11 };
            mesh.AddPoly(poly16, normal3);

            int[] poly17 = { v7, v10, v23, v22 };
            mesh.AddPoly(poly17, normal5);
            return mesh;
        }
    }
}
