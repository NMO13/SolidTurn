using Geometry;
using Mesh;
using Simulation.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoObjectStuff;
using MeshStructuresLib.HalfEdgeStructure2D;
using _3DMeshStructureLib.HalfEdgeStructure3D;
using Geometry.FloatingPointStuff;

namespace Simulation.Machine_Stuff
{
    /*
     * Calculates the tool swept volume for CONVEX polyhedra.
     */
    class TSVCalculator
    {
        internal TSVCalculator(DocumentModel m)
        {
        }

        internal BVHPolyMesh2D FeedRate2D(Tool tool, Vector3D direction)
        {

            BVHPolyMesh2D newMesh = new BVHPolyMesh2D();

            List<int> indices = tool.CuttingEdgePolyIndices;
            HalfEdge3Poly p = tool.Mesh.Polys[indices[0]];
            Debug.Assert(EpsilonTests.IsNearlyZeroEpsHigh(direction.Z));
            DataStructures.Tuple<double, HalfEdge3> min = new DataStructures.Tuple<double, HalfEdge3>(Double.MaxValue, null);
            DataStructures.Tuple<double, HalfEdge3> max = new DataStructures.Tuple<double, HalfEdge3>(Double.MinValue, null);
            
            // build 2d TSV for this poly

            //calc min and max vertex
            foreach (int i in tool.Mesh.GetFaceCirculator(p.OuterComponent))
            {
                HalfEdge3 h = tool.Mesh.HalfEdges[i];
                Debug.Assert(EpsilonTests.IsNearlyZeroEpsHigh(tool.Mesh.Vertices[h.Origin].Z));
                double c = direction.Y * tool.Mesh.Vertices[h.Origin].X - direction.X * tool.Mesh.Vertices[h.Origin].Y;
                if (c < min.Val1)
                {
                    min.Val1 = c;
                    min.Val2 = h;
                }
                if (c >= max.Val1)
                {
                    max.Val1 = c;
                    max.Val2 = h;
                }
            }

            Debug.Assert(min.Val2 != null && max.Val2 != null);
            Debug.Assert(min.Val2 != max.Val2);
            // Create tsv of poly
            List<HEVector2> vertexList = new List<HEVector2>();
            bool onFrontFace = true;
            HalfEdge3 start, it;
            it = start = max.Val2;
            do
            {
                if (it == max.Val2)
                {
                    vertexList.Add(new HEVector2(tool.Mesh.Vertices[it.Origin].X, tool.Mesh.Vertices[it.Origin].Y));
                }
                if (onFrontFace)
                {
                    vertexList.Add(new HEVector2(tool.Mesh.Vertices[it.Origin].X + direction.X, tool.Mesh.Vertices[it.Origin].Y + direction.Y));
                }
                else
                {
                    vertexList.Add(new HEVector2(tool.Mesh.Vertices[it.Origin].X, tool.Mesh.Vertices[it.Origin].Y));
                }
                if (it == min.Val2)
                {
                    vertexList.Add(new HEVector2(tool.Mesh.Vertices[it.Origin].X, tool.Mesh.Vertices[it.Origin].Y));
                    onFrontFace = false;
                }
                it = tool.Mesh.HalfEdges[it.Next];
            }
            while (it != start);
            newMesh.CreateMeshFromVertices(vertexList);
            newMesh.CreateBVH();

            return newMesh;
        }
    }
}
