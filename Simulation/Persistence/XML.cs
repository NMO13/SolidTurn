using Geometry;
using Mesh;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Builder;

namespace Simulation.Persistence
{
    abstract class XMLMeshReader
    {
        protected string m_FilePath;
        protected XmlDocument doc = null;
        
        internal XMLMeshReader(string filePath)
        {
            doc = new XmlDocument();
            doc.Load(filePath);
            m_FilePath = filePath;
        }

        internal abstract List<Vector3D> ReadVertices();

        internal abstract void ReadPolys(out List<int[]> vertIndizes, out List<int[]> normIndizes);

        internal abstract List<Vector3D> ReadNormals();
    }

    class XMLToolReader : XMLMeshReader
    {
        List<int> m_IdList = new List<int>();
        internal XMLToolReader(string filePath)
            : base(filePath)
        {

        }

        internal List<int> ReadIds()
        {
            return m_IdList;
        }

        internal override List<Vector3D> ReadVertices()
        {
            List<Vector3D> list = new List<Vector3D>();
            XmlNodeList vertices = doc.SelectNodes(@"Tool/Mesh/Vertices/Vertex");
            foreach (XmlNode vertex in vertices)
            {
                double xVal = double.Parse(vertex["X"].InnerText, CultureInfo.InvariantCulture);
                double yVal = double.Parse(vertex["Y"].InnerText, CultureInfo.InvariantCulture);
                double zVal = double.Parse(vertex["Z"].InnerText, CultureInfo.InvariantCulture);
                list.Add(new Vector3D(xVal, yVal, zVal));
            }
            return list;
        }

        internal override List<Vector3D> ReadNormals()
        {
            List<Vector3D> normals = new List<Vector3D>();

            XmlNodeList xmlnormals = doc.SelectNodes(@"Tool/Mesh/Normals/Normal");
            foreach (XmlNode normal in xmlnormals)
            {
                double x = double.Parse(normal["X"].InnerText, CultureInfo.InstalledUICulture);
                double y = double.Parse(normal["Y"].InnerText, CultureInfo.InstalledUICulture);
                double z = double.Parse(normal["Z"].InnerText, CultureInfo.InstalledUICulture);
                normals.Add(new Vector3D(x, y, z));
            }
            return normals;
        }

        internal override void ReadPolys(out List<int[]> vertIndizes, out List<int[]> normIndizes)
        {
            vertIndizes = new List<int[]>();
            normIndizes = new List<int[]>();
            XmlNodeList xmlpolys = doc.SelectNodes(@"Tool/Mesh/Polys/Polygon");
            foreach (XmlNode polys in xmlpolys)
            {
                string id = polys.Attributes["id"].Value;
                m_IdList.Add(int.Parse(id));
                XmlNode verts = polys.SelectSingleNode(@"Verts");
                string countS = verts.Attributes["Count"].Value;
                int count = int.Parse(countS);

                int[] v = new int[count];
                string[] values = verts.InnerText.Split(' ');
                for (int i = 0; i < count; i++)
                    v[i] = int.Parse(values[i]);
                vertIndizes.Add(v);

                XmlNode normalsIndex = polys.SelectSingleNode(@"Normals");
                values = normalsIndex.InnerText.Split(' ');
                int[] n = new int[count];
                if (values.Length != count)
                    throw new Exception("Every vertex needs a normal");
                for (int i = 0; i < count; i++)
                    n[i] = int.Parse(values[i]);
                normIndizes.Add(n);
            }
        }

        internal List<int> ReadCuttingEdgePolyIndizes()
        {
            List<int> ids = new List<int>();
            XmlNodeList xmlpolys = doc.SelectNodes(@"Tool/Mesh/Polys/Polygon");
            foreach (XmlNode poly in xmlpolys)
            {
                bool val = bool.Parse(poly["CuttingEdge"].InnerText);
                if (val)
                {
                    string id = poly.Attributes["id"].Value;
                    ids.Add(int.Parse(id));
                }
            }
            return ids;
        }

        internal string ReadName()
        {
            XmlNode root = doc.SelectSingleNode(@"Tool");
            string s = root.Attributes["Name"].Value;
            return s;
        }
    }
}
