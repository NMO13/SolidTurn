using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;

namespace Testing_Environment.src.GeometryUtility 
{
    class SceneRotator
    {
        private Matrix4f LastTransformation = new Matrix4f();
        private Matrix4f ThisTransformation = new Matrix4f();
        private arcball m_arcBall = new arcball(0, 0);
        private Quat4f ThisQuat = new Quat4f();
        private double[] matrix = new double[16];
        private Boolean isRotating = false;

        public SceneRotator()
        {
            LastTransformation.SetIdentity(); // Reset Rotation
            ThisTransformation.SetIdentity(); // Reset Rotation
            ThisTransformation.get_Renamed(matrix);
        }

        public void StartDrag(Point MousePt)
        {
            LastTransformation.set_Renamed(ThisTransformation); // Set Last Static Rotation To Last Dynamic One
            m_arcBall.click(MousePt); // Update Start Vector And Prepare For Dragging
            isRotating = true;
        }

        public void Drag(Point MousePt)
        {
            if (isRotating)
            {
                Quat4f ThisQuat = new Quat4f();
                m_arcBall.drag(MousePt, ThisQuat);
                ThisTransformation.Pan = new Vector3f(0, 0, 0);
                ThisTransformation.Scale = 1.0f;
                ThisTransformation.Rotation = ThisQuat;
                Matrix4f.MatrixMultiply(ThisTransformation, LastTransformation);
                ThisTransformation.get_Renamed(matrix);
            }
        }

        public TransformationMatrix GetRotationMatrix()
        {
            return TransformationMatrix.ArrayToMatrix(matrix, false);
        }

        public void StopDrag()
        {
            isRotating = false;
        }

        public void SetBounds(int width, int height)
        {
            m_arcBall.setBounds(width, height);
        }
    }
}
