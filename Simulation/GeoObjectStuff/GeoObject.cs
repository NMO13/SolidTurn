using _3DMeshStructureLib.HalfEdgeStructure3D;
using Geometry;
using Mesh;
using OpenTK.Graphics.OpenGL;
using Simulation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoObjectStuff
{
    abstract class GeoObject : IRender
    {
        internal Microsoft.Practices.EnterpriseLibrary.Logging.LogWriter LogWriter { get; set; }
        protected Mesh3 m_Mesh = null;
        protected TransformationMatrix m_transformationMatrix = new TransformationMatrix();
        public uint Id { get; private set; }
        protected bool m_RenderIt;
        public bool IsSelected { get; set; }
        public bool WireframeView { get; set; }
        public object AnimationLock { get; set; }

        public TransformationMatrix TransformationMatrix { get { return m_transformationMatrix; } set { m_transformationMatrix = value; } }

        public void RenderIt(bool val)
        {
            m_RenderIt = val;
        }
        public double[] TransformationMatrixAsArray
        {
            get { return TransformationMatrix.MatrixToColumnMajorArray(m_transformationMatrix); }
            set { TransformationMatrix.ArrayToMatrix(value, true); }
        }

        public GeoObject(uint id)
        {
            this.Id = id;
            IsSelected = false;
            TransformationMatrix = TransformationMatrix.IdentityMatrix();
            m_RenderIt = true;
        }

        public void Render()
        {
            if (m_Mesh != null)
            {
                RenderMesh();
            }
        }

        protected abstract void RenderMesh();


        internal Mesh3 Mesh
        {
            get { return m_Mesh; }
            set
            { 
                m_Mesh = value;
            }
        }

        internal virtual void Translate(Vector3D v)
        {
            if (v.X != 0.0 || v.Y != 0.0 || v.Z != 0.0)
            {
                foreach (HalfEdge3Vertex vert in m_Mesh.Vertices)
                {
                    vert.X += v.X;
                    vert.Y += v.Y;
                    vert.Z += v.Z;
                }
                m_transformationMatrix.Translate(v);
            }
        }

        internal virtual void Translate(double x, double y, double z)
        {
            Translate(new Vector3D(x, y, z));
        }

        internal virtual void Rotate(double angle, Vector3D direction)
        {
            Vector3D d = direction.Unit();
            double x = d.X;
            double y = d.Y;
            double z = d.Z;

            TransformationMatrix m = new TransformationMatrix();
            double c = Math.Cos(angle);
	        double s = Math.Sin(angle);

            m[0,0] = (x*x) * (1-c)+c;
            m[1,0] = x*y * (1-c)-z*s;
            m[2,0] = x*z * (1-c)+y*s;

            m[0,1] = y*x * (1-c)+z*s;
            m[1,1] = (y*y) * (1-c)+c;
            m[2,1] = y*z * (1-c)-x*s;

            m[0,2] = x*z * (1-c)-y*s;
            m[1,2] = y*z * (1-c)+x*s;
            m[2,2] = (z*z) * (1-c)+c;

            UpdateLocation(m);

            m = m.Invert().Transpose();
            foreach (Vector3D v in m_Mesh.Normals)
            {
                double x0 = m[0, 0] * v.X + m[0, 1] * v.Y + m[0, 2] * v.Z + m[0, 3];
                double y0 = m[1, 0] * v.X + m[1, 1] * v.Y + m[1, 2] * v.Z + m[1, 3];
                double z0 = m[2, 0] * v.X + m[2, 1] * v.Y + m[2, 2] * v.Z + m[2, 3];
                v.X = x0; v.Y = y0; v.Z = z0;
            }

        }

        internal void UpdateLocation(TransformationMatrix transform)
        {
            foreach (HalfEdge3Vertex v in m_Mesh.Vertices)
            {
                double x = transform[0, 0] * v.X + transform[0, 1] * v.Y + transform[0, 2] * v.Z + transform[0, 3];
                double y = transform[1, 0] * v.X + transform[1, 1] * v.Y + transform[1, 2] * v.Z + transform[1, 3];
                double z = transform[2, 0] * v.X + transform[2, 1] * v.Y + transform[2, 2] * v.Z + transform[2, 3];
                v.X = x; v.Y = y; v.Z = z;
            }

            m_transformationMatrix = transform * m_transformationMatrix;
        }


        public bool ShouldRender()
        {
            return m_RenderIt;
        }
    }
}
