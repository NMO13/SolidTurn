using GeoObjectStuff;
using Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesh.GeoObjectStuff
{
    internal class GeoObjectComposite : IRender, IEnumerable<GeoObject>
    {
        private List<GeoObject> parts = new List<GeoObject>();

        internal virtual void AddPart(GeoObject o)
        {
            parts.Add(o);
        }

        public virtual void Render()
        {
            foreach (GeoObject o in parts)
                o.Render();
        }

        public virtual bool ShouldRender()
        {
            return true;
        }

        public virtual void RenderIt(bool v)
        {
        }

        public IEnumerator<GeoObject> GetEnumerator()
        {
            foreach (GeoObject o in parts)
            {
                if (o == null)
                {
                    break;
                }
                yield return o;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
