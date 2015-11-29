using Mesh.Geometry;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesh.GeoObjectStuff
{
    class OGLMesh : Mesh3D
    {
        public void Render()
        {
            GL.Begin(PrimitiveType.Triangles);
            foreach (Face face in Faces)
            {
                GL.Normal3(face.n0.x, face.n0.y, face.n0.z);
                GL.Vertex3(face.v0.x, face.v0.y, face.v0.z);
                GL.Normal3(face.n1.x, face.n1.y, face.n1.z);
                GL.Vertex3(face.v1.x, face.v1.y, face.v1.z);
                GL.Normal3(face.n2.x, face.n2.y, face.n2.z);
                GL.Vertex3(face.v2.x, face.v2.y, face.v2.z);
            }
            GL.End();
        }
    }
}
