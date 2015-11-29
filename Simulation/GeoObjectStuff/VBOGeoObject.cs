using _3DMeshStructureLib.HalfEdgeStructure3D;
using Geometry;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoObjectStuff
{
    class VBOGeoObject : MaterializedGeoObject
    {
        protected uint ID;
        protected int m_Size;
        protected List<Vertex3F> m_Vertex3FList = new List<Vertex3F>();
        private bool m_Modified = false;

        internal VBOGeoObject(uint id)
            : base(id)
        {
        }

        protected virtual void LoadVBO()
        {
            if (m_Vertex3FList.Count == 0)
                return;
            if (ID == 0)
            {
                GL.GenBuffers(1, out ID);
                ErrorCode code = GL.GetError();
                if (code != 0)
                    throw new Exception(code.ToString());
            }

            Vertex3F[] list = m_Vertex3FList.ToArray();
            m_Size = list.Length;

            GL.BindBuffer(BufferTarget.ArrayBuffer, ID);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(list.Length * Vertex3F.Stride), list, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void CreateVertexArray()
        {
            m_Vertex3FList.Clear();
            for(int f = 0; f < m_Mesh.Polys.Count; f++)
            {
                int[] hes = m_Mesh.GetHalfEdges(f);
                Triangulate(hes);
            }
            m_Modified = true;
        }

        protected void Triangulate(int[] hes)
        {
            if (hes.Length < 3)
                throw new Exception("Cannot triangulate");
            HalfEdge3 start = m_Mesh.HalfEdges[hes[0]];
            Debug.Assert(start.Normal != -1);
            HalfEdge3Vertex v0 = m_Mesh.Vertices[start.Origin];

            for (int i = 1; i < hes.Length - 1; i++)
            {
                HalfEdge3 h1 = m_Mesh.HalfEdges[hes[i]];
                Debug.Assert(h1.Normal != -1);
                HalfEdge3Vertex v1 = m_Mesh.Vertices[h1.Origin];

                HalfEdge3 h2 = m_Mesh.HalfEdges[hes[i+1]];
                Debug.Assert(h2.Normal != -1);
                HalfEdge3Vertex v2 = m_Mesh.Vertices[h2.Origin];

                m_Vertex3FList.Add(new Vertex3F((float)v0.X, (float)v0.Y, (float)v0.Z, (float)m_Mesh.Normals[start.Normal].X, (float)m_Mesh.Normals[start.Normal].Y, (float)m_Mesh.Normals[start.Normal].Z));
                m_Vertex3FList.Add(new Vertex3F((float)v1.X, (float)v1.Y, (float)v1.Z, (float)m_Mesh.Normals[h1.Normal].X, (float)m_Mesh.Normals[h1.Normal].Y, (float)m_Mesh.Normals[h1.Normal].Z));
                m_Vertex3FList.Add(new Vertex3F((float)v2.X, (float)v2.Y, (float)v2.Z, (float)m_Mesh.Normals[h2.Normal].X, (float)m_Mesh.Normals[h2.Normal].Y, (float)m_Mesh.Normals[h2.Normal].Z));
            }
        }

        protected override void RenderMesh()
        {
            base.RenderMesh();
            if (AnimationLock != null)
            {
                lock (AnimationLock)
                {
                    if (m_Modified)
                    {
                        LoadVBO();
                        m_Modified = false;
                    }
                    GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);
                    // Bind current context to Array Buffer ID
                    GL.BindBuffer(BufferTarget.ArrayBuffer, ID);
                    // Set the Pointer to the current bound array describing how the data ia stored
                    GL.VertexPointer(3, VertexPointerType.Float, Vertex3F.Stride, IntPtr.Zero);

                    // Enable the client state so it will use this array buffer pointer
                    GL.EnableClientState(ArrayCap.VertexArray);
                    // Enable the client state so it will use this array buffer pointer
                    GL.EnableClientState(ArrayCap.NormalArray);
                    GL.NormalPointer(NormalPointerType.Float, Vertex3F.Stride, (IntPtr)(3 * sizeof(float)));

                    GL.DrawArrays(PrimitiveType.Triangles, 0, m_Size);
                    GL.PopClientAttrib();
                }
            }
            else
            {
                    if (m_Modified)
                    {
                        LoadVBO();
                        m_Modified = false;
                    }
                    GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);
                    // Bind current context to Array Buffer ID
                    GL.BindBuffer(BufferTarget.ArrayBuffer, ID);
                    // Set the Pointer to the current bound array describing how the data ia stored
                    GL.VertexPointer(3, VertexPointerType.Float, Vertex3F.Stride, IntPtr.Zero);

                    // Enable the client state so it will use this array buffer pointer
                    GL.EnableClientState(ArrayCap.VertexArray);
                    // Enable the client state so it will use this array buffer pointer
                    GL.EnableClientState(ArrayCap.NormalArray);
                    GL.NormalPointer(NormalPointerType.Float, Vertex3F.Stride, (IntPtr)(3 * sizeof(float)));

                    GL.DrawArrays(PrimitiveType.Triangles, 0, m_Size);
                    GL.PopClientAttrib();
            }
            
        }

        internal override void Translate(Vector3D v)
        {
            base.Translate(v);
            CreateVertexArray();
        }

        internal override void Rotate(double angle, Vector3D direction)
        {
            base.Rotate(angle, direction);
            CreateVertexArray();
        }
    }
}
