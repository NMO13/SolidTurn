using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Geometry;

namespace Geometry_Mesh2D
{
    public class Mesh2D
    {
        protected Dictionary<Vertex2D, Vertex2D> m_Vertices = new Dictionary<Vertex2D, Vertex2D>();
        protected List<Line> m_Lines = new List<Line>();

        public List<Line> Lines { get { return m_Lines; } }

        public void CreateLine(Vertex2D v0, Vertex2D v1)
        {
            if (!m_Vertices.TryGetValue(v0, out v0))
                throw new Exception("Vertex was not found in dictionary");
            if (!m_Vertices.TryGetValue(v1, out v1))
                throw new Exception("Vertex was not found in dictionary");
            Line line = new Line(new Vector2D(v0.x, v0.y), new Vector2D(v1.x, v1.y));
            m_Lines.Add(line);
        }

        public void AddVertex(Vertex2D v)
        {
            Vertex2D res;
            if (!m_Vertices.TryGetValue(v, out res))
            {
                m_Vertices.Add(v, v);
            }
        }
    }
}
