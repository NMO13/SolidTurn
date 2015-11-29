using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mesh;
using System.Collections.Generic;

namespace MeshUnitTests
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void B1()
        {
            Cube();
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
    }
}
