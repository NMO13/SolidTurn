using Geometry;
using Mesh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using MeshStructuresLib.HalfEdgeStructure2D;
using _2DMeshStructuresLib.VBO_Management;
using Geometry.FloatingPointStuff;

namespace Mesh
{
    // Has actually very little to do with ConcentricMesh3D but for compatibility reasons it derives from it
    public class RoughPartConcentricMesh3D : ConcentricMesh3D
    {
        public void CreatePolygonWithNormals(AABRHalfEdge2 l, List<Vector3D> verts, int count, Bucket bucket)
        {
            if (verts.Count < 3)
                throw new Exception("Too few vertices specified");
            Vector3D v0 = verts[0];
            Vector3D v1 = verts[1];
            Vector3D v2 = verts[2];
            
            List<Vertex3F> list;
            bucket.ConcentricSegments.TryGetValue(l, out list);
            if (list == null)
            {
                list = new List<Vertex3F>();
                bucket.ConcentricSegments.Add(l, list);
            }
           // List<Vertex3F> lf = new List<Vertex3F>(verts.Count);
            if (count == 3)
            {
                Vector3D normal = Vector3D.PlaneNormal(v0, v1, v2).Unit(); // important for lighting
                verts.ForEach(x => list.Add(new Vertex3F((float)x.X, (float)x.Y, (float)x.Z, (float)normal.X, (float)normal.Y, (float)normal.Z)));
            }
            else if (count == 4)
            {
                Vector3D v3 = verts[3];
                Vector3D n0 = new Vector3D(0, v0.Y, v0.Z).Unit();
                Vector3D n1 = new Vector3D(0, v1.Y, v1.Z).Unit();

                if (EpsilonTests.IsNearlyZeroEpsHigh(v0.X - v2.X))
                {
                    Debug.Assert(EpsilonTests.IsNearlyZeroEpsHigh(v1.X - v3.X));
                    n0 = Vector3D.PlaneNormal(v0, v1, v2).Unit();
                    n1 = n0;
                }
                list.Add(new Vertex3F((float)v0.X, (float)v0.Y, (float)v0.Z, (float)n0.X, (float)n0.Y, (float)n0.Z));
                list.Add(new Vertex3F((float)v1.X, (float)v1.Y, (float)v1.Z, (float)n1.X, (float)n1.Y, (float)n1.Z));
                list.Add(new Vertex3F((float)v2.X, (float)v2.Y, (float)v2.Z, (float)n1.X, (float)n1.Y, (float)n1.Z));

                list.Add(new Vertex3F((float)v0.X, (float)v0.Y, (float)v0.Z, (float)n0.X, (float)n0.Y, (float)n0.Z));
                list.Add(new Vertex3F((float)v2.X, (float)v2.Y, (float)v2.Z, (float)n1.X, (float)n1.Y, (float)n1.Z));
                list.Add(new Vertex3F((float)v3.X, (float)v3.Y, (float)v3.Z, (float)n0.X, (float)n0.Y, (float)n0.Z));
            }
            else
                throw new Exception("The count was " + Environment.NewLine + count);
        }

        public void Update(int slices, VBOManager VBOManager)
        {
            int counter = 0;
            foreach (Bucket b in VBOManager.buckets)
            {
                counter += b.Lines.Count;
                if (b.Modified)
                    UpdateBucket(b, slices);
            }
        }

        private void UpdateBucket(Bucket bucket, int slices)
        {
            bucket.ConcentricSegments.Clear();
            foreach (AABRHalfEdge2 h in bucket.Lines)
                CreateConcentric(h, slices, bucket);

            List<Vertex3F> completeList = new List<Vertex3F>();
            foreach (List<Vertex3F> l in bucket.ConcentricSegments.Values)
                completeList.AddRange(l);
            bucket.VertexArray = completeList.ToArray();
        }

        public void BuildCompleteMesh(int slices, VBOManager manager)
        {
            foreach (Bucket b in manager.buckets)
                UpdateBucket(b, slices);
        }

        private void CreateConcentric(AABRHalfEdge2 l, int slices, Bucket bucket)
        {
            //slices = 4;
            Vector3D v0 = new Vector3D(l.Origin.X, 0, l.Origin.Y);
            Vector3D v1 = new Vector3D(l.Next.Origin.X, 0, l.Next.Origin.Y);

            if (EpsilonTests.IsNearlyZeroEpsHigh(l.Origin.Y) && EpsilonTests.IsNearlyZeroEpsHigh(l.Next.Origin.Y))
                return;

            double angle = 2 * Math.PI / slices;
            Vector3D v0Prev = v0;
            Vector3D v1Prev = v1;

            Vector3D v0t0 = new Vector3D(l.Origin.X, 0, l.Origin.Y);
            Vector3D v1t0 = new Vector3D(l.Next.Origin.X, 0, l.Next.Origin.Y);

            Vector3D v0t1 = new Vector3D(l.Origin.X, l.Origin.Y * Math.Sin(angle), l.Origin.Y * Math.Cos(angle));
            Vector3D v1t1 = new Vector3D(l.Next.Origin.X, l.Next.Origin.Y * Math.Sin(angle), l.Next.Origin.Y * Math.Cos(angle));

            Vector3D v0t2 = new Vector3D(l.Origin.X, l.Origin.Y * Math.Sin(angle * 2), l.Origin.Y * Math.Cos(angle * 2));
            Vector3D v1t2 = new Vector3D(l.Next.Origin.X, l.Next.Origin.Y * Math.Sin(angle * 2), l.Next.Origin.Y * Math.Cos(angle * 2));

            Vector3D normal0 = Vector3D.PlaneNormal(v0t0, v0t1, v1t0).Unit();
            Vector3D normal1 = Vector3D.PlaneNormal(v0t1, v0t2, v1t1).Unit();

            double p0 = l.Origin.Y;
            double p1 = l.Next.Origin.Y;

            List<Vertex3F> list;
            bucket.ConcentricSegments.TryGetValue(l, out list);
            if (list == null)
            {
                list = new List<Vertex3F>();
                bucket.ConcentricSegments.Add(l, list);
            }

            for (int j = 0, i = 3; j < slices; j++, i++, i %= slices)
            {

                Vector3D v0t3 = new Vector3D(l.Origin.X, l.Origin.Y * Math.Sin(angle * i), l.Origin.Y * Math.Cos(angle * i));
                Vector3D v1t3 = new Vector3D(l.Next.Origin.X, l.Next.Origin.Y * Math.Sin(angle * i), l.Next.Origin.Y * Math.Cos(angle * i));
                Vector3D normal2 = Vector3D.PlaneNormal(v0t2, v0t3, v1t2).Unit();

                Vector3D n0 = (normal0 + normal1).Unit();
                Vector3D n1 = (normal1 + normal2).Unit();
                if (EpsilonTests.IsNearlyZeroEpsHigh(p0)) // create polys with 3 vertices
                {
                    normal0 = Vector3D.PlaneNormal(v0t0, v1t1, v1t0);
                    normal1 = Vector3D.PlaneNormal(v0t0, v1t2, v1t1);
                    normal2 = Vector3D.PlaneNormal(v0t0, v1t3, v1t2);
                    n0 = (normal0 + normal1).Unit();
                    n1 = (normal1 + normal2).Unit();
                }
                else if (EpsilonTests.IsNearlyZeroEpsHigh(p1)) // create polys with 3 vertices
                {
                    normal0 = Vector3D.PlaneNormal(v0t0, v0t1, v1t0);
                    normal1 = Vector3D.PlaneNormal(v0t1, v0t2, v1t0);
                    normal2 = Vector3D.PlaneNormal(v0t2, v0t3, v1t0);
                    n0 = (normal0 + normal1).Unit();
                    n1 = (normal1 + normal2).Unit();

                }
                else // create polys with 4 vertices
                {
                    normal0 = Vector3D.PlaneNormal(v0t0, v0t1, v1t0);
                    normal1 = Vector3D.PlaneNormal(v0t1, v0t2, v1t1);
                    normal2 = Vector3D.PlaneNormal(v0t2, v0t3, v1t2);
                    n0 = (normal0 + normal1).Unit();
                    n1 = (normal1 + normal2).Unit();
                }

                list.Add(new Vertex3F((float)v0t1.X, (float)v0t1.Y, (float)v0t1.Z, (float)n0.X, (float)n0.Y, (float)n0.Z));
                list.Add(new Vertex3F((float)v0t2.X, (float)v0t2.Y, (float)v0t2.Z, (float)n1.X, (float)n1.Y, (float)n1.Z));
                list.Add(new Vertex3F((float)v1t1.X, (float)v1t1.Y, (float)v1t1.Z, (float)n0.X, (float)n0.Y, (float)n0.Z));

                list.Add(new Vertex3F((float)v0t2.X, (float)v0t2.Y, (float)v0t2.Z, (float)n1.X, (float)n1.Y, (float)n1.Z));
                list.Add(new Vertex3F((float)v1t2.X, (float)v1t2.Y, (float)v1t2.Z, (float)n1.X, (float)n1.Y, (float)n1.Z));
                list.Add(new Vertex3F((float)v1t1.X, (float)v1t1.Y, (float)v1t1.Z, (float)n0.X, (float)n0.Y, (float)n0.Z));


                
                v0t0 = v0t1;
                v1t0 = v1t1;
                v0t1 = v0t2;
                v1t1 = v1t2;
                v0t2 = v0t3;
                v1t2 = v1t3;
                normal0 = normal1;
                normal1 = normal2;
            }
        }
    }
}
