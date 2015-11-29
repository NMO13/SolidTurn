using _2DMeshStructuresLib.VBO_Management;
using Geometry;
using Mesh;
using MeshStructuresLib.HalfEdgeStructure2D;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoObjectStuff
{
    class RoughPartGeoObject : MaterializedGeoObject
    {
        protected int m_Slices = 0;
        internal bool Render2D = false;
        protected long CAPACITY = 300000; // space for 40 lines
        private int MaxLinesPerBucket = 40; 
        private bool m_OutputVBOConsumption = false;
        VBOManager m_VBOManager;
        uint[] m_VBO_IDs;
        internal RoughPartGeoObject(uint id) : base(id)
        {
            m_VBOManager = new VBOManager(40, MaxLinesPerBucket);
            CreateVBOs();
        }

        internal void FreeVBOs()
        {
            foreach (Bucket b in m_VBOManager.buckets)
            {
                GL.DeleteBuffer(b.ID);
                b.ID = 0;
            }
        }

        internal VBOManager VBOManager { get { return m_VBOManager; } }

        protected void CreateVBOs()
        {
            m_VBO_IDs = new uint[m_VBOManager.BucketCount];
            GL.GenBuffers(m_VBOManager.BucketCount, m_VBO_IDs);
            if (GL.GetError() != ErrorCode.NoError)
            {
                throw new ApplicationException("Error while creating VERTICES Buffer Object.\n\nERROR: " + Enum.GetName(typeof(ErrorCode), GL.GetError()));
            }

            for (int i = 0; i < m_VBOManager.BucketCount; i++)
                LoadVBO(i);
        }

        protected void LoadVBO(int i)
        {
            VBOManager.buckets[i].ID = m_VBO_IDs[i];
            // create buffers
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_VBO_IDs[i]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(CAPACITY), IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        protected void LoadVBO(Bucket b, uint id)
        {
            b.ID = id;
            // create buffers
            GL.BindBuffer(BufferTarget.ArrayBuffer, id);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(CAPACITY), IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        internal new RoughPartConcentricMesh3D Mesh
        {
            get { return m_Mesh as RoughPartConcentricMesh3D; }
            set
            {
                m_Mesh = value;
            }
        }

        private Line GetParent(Line l)
        {
            while (l.Parent != null)
                l = l.Parent;
            return l;
        }

        internal void BuildMesh3D(int slices = -1)
        {
            slices = slices == -1 ? m_Slices : slices;
            m_Slices = slices;
            Mesh.BuildCompleteMesh(slices, m_VBOManager);
        }

        protected override void RenderMesh()
        {
            if (Render2D)
            {
                GL.Disable(EnableCap.CullFace);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                GL.Begin(PrimitiveType.Polygon);
                foreach (HalfEdge2 l in ((RoughPartConcentricMesh3D)m_Mesh).Poly2D.HalfEdgeIterator())
                {
                    GL.Vertex3(l.Origin.X, 0, l.Origin.Y);
                }
                GL.End();
                GL.Enable(EnableCap.CullFace);
            }
            else
            {
                base.RenderMesh();

                lock (AnimationLock)
                {
                    foreach (Bucket b in m_VBOManager.buckets)
                    {
                        if(b.Lines.Count > 0) // if a new bucket has no lines then it might be possible that it also has no vertex array
                            RevalidateBucket(b);
                    }
                }
            }
        }

        protected void RevalidateBucket(Bucket bucket)
        {
            GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);
            if (bucket.New)
                CreateNewVBO(bucket);
            if (bucket.Modified)
            {
                bucket.Modified = false;
                int bufferSize;
               
                // Vertex Array Buffer
                {
                    // Bind current context to Array Buffer ID
                    GL.BindBuffer(BufferTarget.ArrayBuffer, bucket.ID);

                    // Send data to buffer
                    GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(bucket.VertexArray.Length * Vertex3F.Stride), bucket.VertexArray, BufferUsageHint.StreamDraw);

                    // Validate that the buffer is the correct size
                    //TODO update this variable
                    if(m_OutputVBOConsumption)
                        LogWriter.Write("Bucket " + bucket.ID + " modified. New size: " + bucket.VertexArray.Length * Vertex3F.Stride + Environment.NewLine, "Debug");
                    GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
                    if (GL.GetError() != ErrorCode.NoError)
                    {
                        throw new ApplicationException("Error while creating VERTICES Buffer Object." + Environment.NewLine + "ERROR: " + Enum.GetName(typeof(ErrorCode), GL.GetError()));
                    }

                    // this code does not run on Intel graphics
                    //if (bucket.VertexArray.Length * Vertex3F.Stride != bufferSize)
                    //{
                    //    String str = "Vertex array not uploaded correctly. Length of vertex array was " + bucket.VertexArray.Length * Vertex3F.Stride +
                    //        "but size of buffer was " + bufferSize;
                    //    throw new ApplicationException(str);
                    //}

                    //// Set the Pointer to the current bound array describing how the data ia stored
                    GL.VertexPointer(3, VertexPointerType.Float, Vertex3F.Stride, IntPtr.Zero);

                    //// Enable the client state so it will use this array buffer pointer
                    GL.EnableClientState(ArrayCap.VertexArray);
                    //// Enable the client state so it will use this array buffer pointer
                    GL.EnableClientState(ArrayCap.NormalArray);
                    GL.NormalPointer(NormalPointerType.Float, Vertex3F.Stride, (IntPtr)(3 * sizeof(float)));

                    GL.DrawArrays(PrimitiveType.Triangles, 0, bucket.VertexArray.Length);

                    ////invalidate VBO
                    //GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(CAPACITY), IntPtr.Zero, BufferUsageHint.StreamDraw);
                    //GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                }

            }
            else
            {
                // Bind current context to Array Buffer ID
                GL.BindBuffer(BufferTarget.ArrayBuffer, bucket.ID);
                // Set the Pointer to the current bound array describing how the data ia stored
                GL.VertexPointer(3, VertexPointerType.Float, Vertex3F.Stride, IntPtr.Zero);

                // Enable the client state so it will use this array buffer pointer
                GL.EnableClientState(ArrayCap.VertexArray);
                // Enable the client state so it will use this array buffer pointer
                GL.EnableClientState(ArrayCap.NormalArray);
                GL.NormalPointer(NormalPointerType.Float, Vertex3F.Stride, (IntPtr)(3 * sizeof(float)));

                GL.DrawArrays(PrimitiveType.Triangles, 0, bucket.VertexArray.Length);

                ////invalidate VBO
                //GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(CAPACITY), IntPtr.Zero, BufferUsageHint.StreamDraw);
                //GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            }
            GL.PopClientAttrib();
        }

        private void CreateNewVBO(Bucket bucket)
        {
            bucket.New = false;
            uint[] newIdArr = new uint[m_VBO_IDs.Length + 1];
            for (int i = 0; i < m_VBO_IDs.Length; i++)
                newIdArr[i] = m_VBO_IDs[i];

            GL.GenBuffers(1, out newIdArr[newIdArr.Length - 1]);
            if (GL.GetError() != ErrorCode.NoError)
            {
                throw new ApplicationException("Error while creating VERTICES Buffer Object.\n\nERROR: " + Enum.GetName(typeof(ErrorCode), GL.GetError()));
            }
            m_VBO_IDs = newIdArr;
            LoadVBO(bucket, newIdArr[newIdArr.Length - 1]);
        }

        internal void UpdateMesh()
        {
            Mesh.Update(m_Slices, m_VBOManager);
        }

        internal override void Translate(Vector3D v)
        {
            base.Translate(v);
            //todo translate lines in buckets
        }

        internal override void Rotate(double angle, Vector3D direction)
        {
            base.Rotate(angle, direction);
            //todo rotate lines in buckets
        }
    }
}
