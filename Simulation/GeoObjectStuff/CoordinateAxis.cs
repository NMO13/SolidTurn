using Geometry;
using OpenTK.Graphics.OpenGL;
using Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Mesh.GeoObjectStuff
{
    class OGLCoordinateAxis : IRender
    {
        internal Vector3D Offset { get; set; }
        internal OGLCoordinateAxis(Vector3D offset)
        {
            Offset = offset;
        }
        public void RenderIt(bool val)
        {
        }

        public bool ShouldRender()
        {
            return true;
        }
        
        public void Render()
        {
            GL.Disable(EnableCap.Lighting);
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(1.0, 0.0, 0.0);
            GL.Vertex3(0.0f + Offset.X, 0.0f + Offset.Y, 0.0f + Offset.Z);
            GL.Vertex3(20.0f + Offset.X, 0.0f + Offset.Y, 0.0f + Offset.Z);

            GL.Color3(0.0, 1.0, 0.0);
            GL.Vertex3(0.0f + Offset.X, 0.0f + Offset.Y, 0.0f + Offset.Z);
            GL.Vertex3(0.0f + Offset.X, 20.0f + Offset.Y, 0.0f + Offset.Z);

            GL.Color3(0.0, 0.0, 1.0);
            GL.Vertex3(0.0f + Offset.X, 0.0f + Offset.Y, 0.0f + Offset.Z);
            GL.Vertex3(0.0f + Offset.X, 0.0f + Offset.Y, 20.0f + Offset.Z);
            GL.End();
            GL.Enable(EnableCap.Lighting);
        }
    }
}
