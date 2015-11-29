using _3DMeshStructureLib.HalfEdgeStructure3D;
using Geometry;
using Mesh.CNC_Turning.Machine_Stuff;
using Simulation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoObjectStuff
{
    class Tool : VBOGeoObject, ICoordinateSystemObserver
    {
        internal string Name = string.Empty;
        internal Vector3D StartPoint { get; private set; }

        internal Tool(uint id)
            : base(id)
        {
        }

        internal void SetStartPoint(Vector3D start)
        {
            AbsolutePosition = Vector3D.Zero();
            Translate(start);
            StartPoint = start;
        }

        internal Vector3D AbsolutePosition = new Vector3D(0, 0, 0);

        // to distinct between cutting polygons and other polygons
        public List<int> CuttingEdgePolyIndices = new List<int>();

        public void CoordinateSystemChanged(object sender, object e)
        {
            object[] arr = e as object[];
            if ((CoordinateSystemType)arr[1] == CoordinateSystemType.PART_ZERO)
            {
                AbsolutePosition -= arr[0] as Vector3D;
            }
        }

        internal override void Translate(Vector3D v)
        {
            AbsolutePosition += v;
            base.Translate(v);
        }
    }
}
