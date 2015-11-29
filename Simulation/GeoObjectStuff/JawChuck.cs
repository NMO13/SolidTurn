using GeoObjectStuff;
using Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesh.GeoObjectStuff
{
    class JawChuck : IRender
    {
        internal GeoObjectComposite MotionLessJaws = new GeoObjectComposite();
        internal GeoObject Chuck { get; set; }
        internal GeoObject RotatingJaws { get; set; }
        private bool m_RenderInMotion = false;
        internal void Start()
        {
            m_RenderInMotion = true;
        }

        internal void Stop()
        {
            m_RenderInMotion = false;
        }

        public virtual void Render()
        {
            Chuck.Render();
            if (m_RenderInMotion)
            {
                RotatingJaws.Render();
            }
            else
            {
                MotionLessJaws.Render();
            }
        }

        public virtual bool ShouldRender()
        {
            return true;
        }

        public virtual void RenderIt(bool v)
        {
        }

        internal void SetJawsPosition(double val)
        {

        }
    }
}
