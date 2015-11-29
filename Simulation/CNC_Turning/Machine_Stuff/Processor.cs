using Geometry;
using Mesh;
using Simulation.Machine_Stuff;
using Simulation.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GeoObjectStuff;
using Builder;
using MeshStructuresLib.Subtraction;
using MeshStructuresLib.HalfEdgeStructure2D;
using Geometry.Bounding_Volume_Hierarchy;
using Simulation.CNC_Turning.Code;
using Mesh.CNC_Turning.Machine_Stuff;
using Geometry.FloatingPointStuff;

namespace Simulation.CNC_Turning.Machine_Stuff
{
    // do the whole calculations here: boolean ops, tsv calculation, trajaectories, ...
    // notify renderer about changes (over document?)
    class Processor
    {
        private TSVCalculator m_TSVCalculator;
        private TrajectoryCalculator m_TrajCalculator;
        private DocumentModel doc;
        private Sentence m_ActiveSentence;
        private int m_SentencePointer;
        private bool m_GoBufferProcessing;
        internal Processor(DocumentModel doc)
        {
            Reset();
            this.doc = doc;
        }

        internal void ProcessNextFrame()
        {
            if (!m_GoBufferProcessing)
            {
                m_ActiveSentence = doc.NCProgram.Sentences[m_SentencePointer++];
                ProcessSentence();
            }
            else
            {
                Vector3D direction;
                if (m_GoBufferProcessing)
                {
                    if (m_ActiveSentence.GoBuffer.Count > 0)
                    {
                        direction = m_ActiveSentence.GoBuffer.Dequeue();
                        Debug.Assert(!EpsilonTests.IsNearlyZeroEpsHigh(direction.Length()));
                        BVHPolyMesh2D tsv = m_TSVCalculator.FeedRate2D(doc.ToolSet.ActiveSlot.Tool, direction);
                        Do2DBooleanOps(doc.Parts[0], tsv);
                        doc.ToolSet.ActiveSlot.Tool.Translate(direction);
                        if (m_ActiveSentence.GoBuffer.Count == 0)
                            m_GoBufferProcessing = false;
                    }
                    else
                        m_GoBufferProcessing = false;
                }
            }
            
        }

        private void ProcessSentence()
        {
            if (m_ActiveSentence.G[92])
            {
                doc.NotifyPartZeroChanged(new Vector3D(m_ActiveSentence.X, m_ActiveSentence.Y, 0));
            }
            if (m_ActiveSentence.M[3] || m_ActiveSentence.M[4])
                doc.NotifySpindleStart();
            if (m_ActiveSentence.M[5])
                doc.NotifySpindleStop();
            switch (m_ActiveSentence.interpolationMode)
            {
                case 0:
                case 1: m_GoBufferProcessing = true; break;
                case 2:
                case 3: m_GoBufferProcessing = true; break;
            }
            if (m_ActiveSentence.M[30])
            {
                Debug.Assert(m_ActiveSentence.GoBuffer == null);
                if (m_SentencePointer != doc.NCProgram.Sentences.Count)
                    throw new Exception("Sentence pointer is " + m_SentencePointer +" but program has " + doc.NCProgram.Sentences.Count + " sentences");
                doc.ChangeState(AnimationState.FINISHED);
            }
            if (m_ActiveSentence.T >= 0)
            {
                doc.SetActiveSlot(m_ActiveSentence.T);
            }
        }

        private void Do2DBooleanOps(GeoObject geoObject, BVHPolyMesh2D tsv)
        {
            RoughPartGeoObject o = geoObject as RoughPartGeoObject;
            RoughPartConcentricMesh3D mesh = o.Mesh as RoughPartConcentricMesh3D;
            if (mesh.Poly2D.HECount == 0) // nothing to do
                return;

            BoolDifference subtractor = new BoolDifference(o.VBOManager);

            BVHPolyMesh2D meshA = mesh.Poly2D;
            subtractor.Subtract(mesh.Poly2D.BVH, tsv.BVH);
            mesh.Poly2D = GetMesh(subtractor.MeshesA);
            mesh.Poly2D.BVH = new BVH(mesh.Poly2D);

#if DEBUG
            foreach (AABRHalfEdge2 h in mesh.Poly2D.HalfEdgeIterator())
            {
                if (h.Origin.X == h.Next.Origin.X && h.Origin.Y == h.Next.Origin.Y)
                    throw new Exception("Vertices are the same");
                if (h.Bucket == null)
                    throw new Exception("Halfedge has no corresponding bucket");
                if(h == h.Twin)
                    throw new Exception("HalfEdge is not correct");
                if (h == h.Next)
                    throw new Exception("HalfEdge is not correct");
            }
#endif

            o.UpdateMesh();
        }

        private BVHPolyMesh2D GetMesh(List<BVHPolyMesh2D> list)
        {
            if (list.Count == 1)
                return list[0];
            else
                foreach (BVHPolyMesh2D mesh in list)
                {
                    mesh.CreateBoundingRectangle();
                    if (mesh.AABR.Min[0] <= 0 || EpsilonTests.IsNearlyZeroEpsHigh(mesh.AABR.Min[0]))
                        return mesh;
                }
            throw new Exception("invalid list");
        }

        internal void CalcTrajectory()
        {
            m_TrajCalculator.Run();
        }

        internal void Reset()
        {
            m_TrajCalculator = new TrajectoryCalculator(doc);
            m_TSVCalculator = new TSVCalculator(doc);
            m_ActiveSentence = null;
            m_SentencePointer = 0;
            m_GoBufferProcessing = false;
        }

        internal void Stop()
        {
            m_TrajCalculator.RequestStop();
        }
    }
}
