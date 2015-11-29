using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;
using Mesh.Config;

namespace Simulation
{
    class WindowMouseKeyEvents
    {
        
        public void MouseMove(object sender, Point pt, SharpGLRenderer renderer)
        {
            renderer.WorldRotator.Drag(pt);
        }

        public void LeftMouseButtonDown(object sender, Point pt, SharpGLRenderer renderer)
        {
            
        }

        public void LeftMouseButtonUp(object sender, Point pt, SharpGLRenderer renderer)
        {
            
        }

        public void RightMouseButtonDown(object sender, Point pt)
        {
           
        }

        public void RightMouseButtonUp(object sender, Point pt)
        {
        }

        public void MiddleMouseButtonDown(object sender, Point pt, SharpGLRenderer renderer)
        {
            renderer.WorldRotator.StartDrag(pt);
        }

        public void MiddleMouseButtonUp(object sender, Point pt, SharpGLRenderer renderer)
        {
            renderer.WorldRotator.StopDrag();
        }

        public void MouseWheel(object sender, int Delta, SharpGLRenderer renderer)
        {
             if (Delta > 0 && renderer.ZoomFactor > Initializer.MinZoom)
                renderer.ZoomFactor -= Initializer.GranularityZoom;
            if (Delta < 0 && renderer.ZoomFactor < Initializer.MaxZoom)
                renderer.ZoomFactor += Initializer.GranularityZoom;

            renderer.ZoomFactor = renderer.ZoomFactor;
        }
    }
}
