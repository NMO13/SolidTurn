using Mesh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;
using GeoObjectStuff;
using _3DMeshStructureLib.HalfEdgeStructure3D;

namespace Builder
{
    public delegate Mesh3 Del3D(params object[] strings);
    public delegate RoughPartConcentricMesh3D Del2D(params object[] strings);
    abstract class AbstractGeoObjectBuilder
    {
        private static uint GeoObjectId = 0;
        public List<GeoObject> GeoObjects
        {
            get { return m_geoObjectList; }
        }

        protected List<GeoObject> m_geoObjectList = new List<GeoObject>();
        protected RoughPartGeoObject createNewRoughPartGeoObject()
        {
            return new RoughPartGeoObject(GeoObjectId++);
        }

        protected VBOGeoObject CreateVBOGeoObject()
        {
            return new VBOGeoObject(GeoObjectId++);
        }

        protected VBOGeoObject CreateNewConcentricGeoObject()
        {
            VBOGeoObject o = new VBOGeoObject(GeoObjectId++);
            return o;
        }

        protected Tool CreateNewTool()
        {
            Tool t = new Tool(GeoObjectId++);
            return t;
        }

        protected abstract void BuildMesh(GeoObject o, Del3D meshBuilder, params object[] oparams);
    }
}
