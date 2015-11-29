using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Mesh;
using Mesh.Triangulation;
namespace MeshUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Mesh3D m = new Mesh3D();
            Vertex3D v1 = m.AddVertex(new Vertex3D(0, 4, 0));
            Vertex3D v2 = m.AddVertex(new Vertex3D(2, 4, 0));
            Vertex3D v3 = m.AddVertex(new Vertex3D(2, 2, 0));
            Vertex3D v4 = m.AddVertex(new Vertex3D(1, 1, 0));
            List<Vertex3D> list = new List<Vertex3D>();

            list.Add(v2);
            list.Add(v1);
            list.Add(v3);
            m.CreatePolygon(list);

            List<HEPolygon> p = m.Polys;
            Assert.AreEqual(p.Count, 1);
            HEPolygon poly = p[0];

            Assert.AreEqual(poly.VertexCount, 3);
            Assert.IsTrue(poly.OuterComponent == m.Vertices[1].IncidentEdge);
            Assert.IsTrue(poly.OuterComponent.Next.Origin.Equals(v1));
            Assert.IsTrue(poly.OuterComponent.Next.Next.Origin.Equals(v3));
            Assert.IsTrue(poly.OuterComponent.Next.Next.Next.Origin.Equals(v2));

            Assert.IsTrue(poly.OuterComponent.Prev.Origin.Equals(v3));

            list.Clear();
            list.Add(v4);
            list.Add(v3);
            list.Add(v1);
            m.CreatePolygon(list);

            p = m.Polys;
            poly = p[1];
            Assert.AreEqual(p.Count, 2);
            Assert.IsTrue(poly.OuterComponent == m.Vertices[3].IncidentEdge);
            Assert.IsTrue(poly.OuterComponent.Next.Origin.Equals(v3));
            Assert.IsTrue(poly.OuterComponent.Next.Next.Origin.Equals(v1));
            Assert.IsTrue(poly.OuterComponent.Next.Next.Next.Origin.Equals(v4));
        }

        [TestMethod]
        public void TestMethod2()
        {

            Mesh3D m = Cube();
            CheckConnectivity(m);
            CheckPolyCount(m.Polys.Count, 6);
            CheckHECount(m.HEdges.Count, 24);


        }

        [TestMethod]
        public void TriangulateTest()
        {
            Mesh3D m = CreateQuad();
            m.GenerateFaces(new SimpleTriangulator());
            CheckPolyCount(m.Polys.Count, 1);
            CheckHECount(m.HEdges.Count, 8);
            CheckPolyCount(m.Faces.Count, 2);

        }

        [TestMethod]
        public void TriangulateTest2()
        {
            Mesh3D m = CreatePentagon();
            m.GenerateFaces(new SimpleTriangulator());
            CheckPolyCount(m.Polys.Count, 1);
            CheckHECount(m.HEdges.Count, 10);
            CheckPolyCount(m.Faces.Count, 3);

        }

        [TestMethod]
        public void TestPolyIterator()
        {
            Mesh3D m = CreatePentagon();
            HEPolygon p = m.Polys[0];
            int count = 0;
            foreach(Vertex3D v in p.VertexIterator())
            {
                count++;
                Assert.IsNotNull(v);
            }
            Assert.AreEqual(count, 5);
        }

        [TestMethod]
        public void TestIfNull()
        {
            Mesh3D m = CreateSimplePolys();

            List<HalfEdge> l = m.HEdges;
            Assert.IsNull(l[3].IncidentPoly);
            Assert.IsNull(l[3].Next);
            Assert.IsNull(l[3].Normal);
        }

        private Mesh3D CreateSimplePolys()
        {
            Mesh3D m = new Mesh3D();
            Vertex3D v0 = m.AddVertex(new Vertex3D(0, 0, 0));
            Vertex3D v1 = m.AddVertex(new Vertex3D(0, 5, 0));
            Vertex3D v2 = m.AddVertex(new Vertex3D(3, 3, 0));
            Vertex3D v3 = m.AddVertex(new Vertex3D(5, 0, 0));
            Vertex3D v4 = m.AddVertex(new Vertex3D(5, 5, 0));

            List<Vertex3D> poly = new List<Vertex3D>();
            poly.Add(v0); poly.Add(v2); poly.Add(v1);
            m.CreatePolygon(poly);

            poly = new List<Vertex3D>();
            poly.Add(v0); poly.Add(v3); poly.Add(v2);
            m.CreatePolygon(poly);

            poly = new List<Vertex3D>();
            poly.Add(v3); poly.Add(v4); poly.Add(v2);
            m.CreatePolygon(poly);

            return m;
        }

        private Mesh3D CreatePentagon()
        {
            Mesh3D m = new Mesh3D();
            Vertex3D v0 = m.AddVertex(new Vertex3D(1, 4, 0));
            Vertex3D v1 = m.AddVertex(new Vertex3D(5, 0, 0));
            Vertex3D v2 = m.AddVertex(new Vertex3D(3, -4, 0));
            Vertex3D v3 = m.AddVertex(new Vertex3D(-1, -4, 0));
            Vertex3D v4 = m.AddVertex(new Vertex3D(-3, 0, 0));
            List<Vertex3D> poly = new List<Vertex3D>();
            poly.Add(v0); poly.Add(v1); poly.Add(v2); poly.Add(v3); poly.Add(v4);
            m.CreatePolygon(poly);
            return m;
        }

        private Mesh3D CreateQuad()
        {
            Mesh3D m = new Mesh3D();
            Vertex3D v0 = m.AddVertex(new Vertex3D(-1, -1, 1));
            Vertex3D v1 = m.AddVertex(new Vertex3D(1, -1, 1));
            Vertex3D v2 = m.AddVertex(new Vertex3D(1, 1, 1));
            Vertex3D v3 = m.AddVertex(new Vertex3D(-1, 1, 1));
            List<Vertex3D> poly = new List<Vertex3D>();
            poly.Add(v0); poly.Add(v1); poly.Add(v2); poly.Add(v3);
            m.CreatePolygon(poly);
            return m;
        }

        private Mesh3D Cube()
        {
            Mesh3D m = new Mesh3D();
            Vertex3D v0 = m.AddVertex(new Vertex3D(-1, -1, 1));
            Vertex3D v1 = m.AddVertex(new Vertex3D(1, -1, 1));
            Vertex3D v2 = m.AddVertex(new Vertex3D(1, 1, 1));
            Vertex3D v3 = m.AddVertex(new Vertex3D(-1, 1, 1));
            Vertex3D v4 = m.AddVertex(new Vertex3D(-1, -1, -1));
            Vertex3D v5 = m.AddVertex(new Vertex3D(1, -1, -1));
            Vertex3D v6 = m.AddVertex(new Vertex3D(1, 1, -1));
            Vertex3D v7 = m.AddVertex(new Vertex3D(-1, 1, -1));

            List<Vertex3D> list = new List<Vertex3D>();
            list.Add(v0);
            list.Add(v1);
            list.Add(v2);
            list.Add(v3);
            m.CreatePolygon(list);

            list.Clear();
            list.Add(v7);
            list.Add(v6);
            list.Add(v5);
            list.Add(v4);
            m.CreatePolygon(list);

            list.Clear();
            list.Add(v1);
            list.Add(v0);
            list.Add(v4);
            list.Add(v5);
            m.CreatePolygon(list);

            list.Clear();
            list.Add(v2);
            list.Add(v1);
            list.Add(v5);
            list.Add(v6);
            m.CreatePolygon(list);

            list.Clear();
            list.Add(v3);
            list.Add(v2);
            list.Add(v6);
            list.Add(v7);
            m.CreatePolygon(list);

            list.Clear();
            list.Add(v0);
            list.Add(v3);
            list.Add(v7);
            list.Add(v4);
            m.CreatePolygon(list);
            return m;
        }

        private void CheckPolyCount(int exp, int act)
        {
            Assert.AreEqual(exp, act);
        }

        private void CheckHECount(int exp, int act)
        {
            Assert.AreEqual(exp, act);
        }

        private void CheckConnectivity(Mesh3D m)
        {
            foreach (HalfEdge e in m.HEdges)
            {
                Assert.IsTrue(e.Next != null);
                Assert.IsTrue(e.Prev != null);
                Assert.IsTrue(e.Normal != null);
            }
        }
    }
}
