using _3DMeshStructureLib.HalfEdgeStructure3D;
using Geometry;
using Geometry.FloatingPointStuff;
using MeshStructuresLib.HalfEdgeStructure2D;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesh
{
    public class VertexRing
    {
        internal VertexRing next;
        internal VertexRing prev;
        internal List<int> Ring = new List<int>();
    }
    public class ConcentricMesh3D : Mesh3
    {
        public BVHPolyMesh2D Poly2D = null;

        private void CreateConcentric(AABRHalfEdge2 l, int slices, ref VertexRing lastSegment)
        {
            VertexRing segment = new VertexRing();
            Vector3D v0 = new Vector3D(l.Origin.X, l.Origin.Y, 0);
            Vector3D v1 = new Vector3D(l.Next.Origin.X, l.Next.Origin.Y, 0);

            if (lastSegment != null)
            {
                lastSegment.next = segment;
                segment.prev = lastSegment;
            }
            else
            {
                start = segment;      
            }
            lastSegment = segment;
            double angle = 2 * Math.PI / slices;
            Vector3D v0Prev = v0;
            if (EpsilonTests.IsNearlyZeroEpsHigh(v0.Y))
            {
                int index = this.AddVertex(v0.X, v0.Y, v0.Z);
                segment.Ring.Add(index);
                return;
            }

            for (int i = 0; i < slices; i++)
            {
                Vector3D vNext = new Vector3D(l.Origin.X, l.Origin.Y * Math.Sin(angle * i), l.Origin.Y * Math.Cos(angle * i));
                int index = this.AddVertex(vNext.X, vNext.Y, vNext.Z);
                segment.Ring.Add(index);
            }

           
        }

        public void BuildMesh3D(int slices)
        {
            CreateVertices(slices);
            CreatePolys(slices);
        }

        private void CreatePolys(int slices)
        {
            VertexRing r1 = start;
            VertexRing r2 = start.next;
            do
            {
                if (r1.Ring.Count == 1 && r2.Ring.Count != 1) // create polys with 3 vertices
                {
                    for (int i = 0, ii = 1; i < slices; i++, ii++, ii %= slices)
                    {
                        Vector3D normal = Vector3D.PlaneNormal(Vertices[r1.Ring[0]].ToVector3(), Vertices[r2.Ring[ii]].ToVector3(), Vertices[r2.Ring[i]].ToVector3()).Unit();
                        int nindex = this.AddNormal(normal);
                        int[] nIndices = { nindex, nindex, nindex };
                        int[] vIndices = {r1.Ring[0], r2.Ring[ii], r2.Ring[i]};
                        this.AddPoly(vIndices, nIndices);
                    }
                }
                else if (r1.Ring.Count != 1 && r2.Ring.Count == 1) // create polys with 3 vertices
                {
                    for (int i = 0, ii = 1; i < slices; i++, ii++, ii %= slices)
                    {
                        Vector3D normal = Vector3D.PlaneNormal(Vertices[r1.Ring[i]].ToVector3(), Vertices[r1.Ring[ii]].ToVector3(), Vertices[r2.Ring[0]].ToVector3()).Unit();
                        int nindex = this.AddNormal(normal);
                        int[] nIndices = { nindex, nindex, nindex };
                        int[] vIndices = { r1.Ring[i], r1.Ring[ii], r2.Ring[0] };
                        this.AddPoly(vIndices, nIndices);
                    }
                }
                else if (r1.Ring.Count != 1 && r2.Ring.Count != 1) // create polys with 4 vertices
                {
                    List<Vector3D> normals = new List<Vector3D>();
                    // calculate the normals for each poly of each slice
                    for (int i = 0, ii = 1; i < slices; i++, ii++, ii %= slices)
                    {
                        Vector3D n1t = Vertices[r1.Ring[i]].ToVector3();
                        Vector3D n2t = Vertices[r2.Ring[i]].ToVector3();
                        Vector3D n3t = Vertices[r2.Ring[ii]].ToVector3();
                        Vector3D nPrev = (n2t - n1t).Cross(n1t - n3t);
                        normals.Add(nPrev);
                    }

                    // take the normal of the predecessor and the successor of a polygon and add it with the normal of current polygon
                    // this gives a vector which directs from the center outwards
                    Vector3D prev = normals[normals.Count - 1];
                    for (int i = 0, ii = 1; i < slices; i++, ii++, ii %= slices)
                    {
                        Vector3D n1t = normals[i];
                        Vector3D n2t = normals[ii];

                        Vector3D n0 = (prev + n1t).Unit();
                        Vector3D n1 = (n1t + n2t).Unit();

                        int index0 = this.AddNormal(n0);
                        int index1 = this.AddNormal(n1);
                        int[] nIndices = { index0, index1, index1, index0 };
                        int[] vIndices = { r1.Ring[i], r1.Ring[ii], r2.Ring[ii], r2.Ring[i] };
                        this.AddPoly(vIndices, nIndices);
                        prev = n1t;
                    }
                }
                else
                {
                }
                r1 = r2;
                r2 = r2.next;
                
            }
            while(r1 != start);
        }

        VertexRing start = null;
        private void CreateVertices(int slices)
        {
            VertexRing cur = null;
 	        foreach(AABRHalfEdge2 h in Poly2D.HalfEdgeIterator())
            {
                CreateConcentric(h, slices, ref cur);
            }
            Debug.Assert(start != null);
            cur.next = start;
            start.prev = cur;
        }

    }
}
