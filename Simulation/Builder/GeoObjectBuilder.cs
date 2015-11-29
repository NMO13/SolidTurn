using Mesh;
using Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;
using GeoObjectStuff;

namespace Builder
{
    class GeoObjectBuilder : AbstractGeoObjectBuilder
    {
        public RoughPartGeoObject BuildRoughPartGeoObject(Del2D meshBuilder, params object[] oparams)
        {
            RoughPartGeoObject o = this.createNewRoughPartGeoObject();
            this.BuildRoughPartMesh(o, meshBuilder, oparams);
            AddMaterial(o);
            m_geoObjectList.Add(o);
            return o;
        }

        public VBOGeoObject BuildVBOGeoObject(Del3D meshBuilder, params object[] oparams)
        {
            VBOGeoObject o = CreateVBOGeoObject();
            BuildMesh(o, meshBuilder, oparams);
            o.CreateVertexArray();
            AddMaterial(o);
            m_geoObjectList.Add(o);
            return o;
        }

        public VBOGeoObject BuildConcentricGeoObject(Del2D meshBuilder, params object[] oparams)
        {
            VBOGeoObject o = this.CreateNewConcentricGeoObject();
            BuildMeshConcentric(o, meshBuilder, oparams);
            o.CreateVertexArray();
            AddMaterial(o);
            m_geoObjectList.Add(o);
            return o;
        }

        private void BuildRoughPartMesh(RoughPartGeoObject o, Del2D meshBuilder, params object[] oparams)
        {
            RoughPartConcentricMesh3D m = meshBuilder(oparams);
            o.Mesh = m;
            o.VBOManager.Build(m.Poly2D);
            o.BuildMesh3D((int)oparams[1]);
        }

        private void BuildMeshConcentric(VBOGeoObject o, Del2D meshBuilder, params object[] oparams)
        {
            ConcentricMesh3D m = meshBuilder(oparams);
            m.BuildMesh3D((int)oparams[1]);
            o.Mesh = m;
        }

        protected void AddMaterial(MaterializedGeoObject o, bool tool = false)
        {
            float[] ambient, diffuse, specular;
            //// gold
            if (tool)
            {
                ambient = new float[]{ 0.24725f, 0.1995f, 0.0745f, 1 };
                diffuse = new float[]{ 0.75164f, 0.60648f, 0.22648f, 1 };
                specular = new float[]{ 0.628281f, 0.555802f, 0.366065f, 1 };
            }
            else
            {
                // silver
                ambient = new float[]{ 0.19225f, 0.19225f, 0.19225f, 1 };
                diffuse = new float[]{ 0.50754f, 0.50754f, 0.50754f, 1 };
                specular = new float[] { 0.508273f, 0.508273f, 0.508273f, 1 };
            }


            FromColor(o.MaterialProps.ambient, ambient[0], ambient[1], ambient[2], ambient[3]);
            FromColor(o.MaterialProps.diffuse, diffuse[0], diffuse[1], diffuse[2], diffuse[3]);
            FromColor(o.MaterialProps.specular, specular[0], specular[1], specular[2], specular[3]);
            FromColor(o.MaterialProps.emission, 0, 0, 0, 1.0f);
            o.MaterialProps.shininess = 51.2f;
        }

        protected override void BuildMesh(GeoObject o, Del3D meshBuilder, params object[] oparams)
        {
            if (meshBuilder != null)
            {
                o.Mesh = meshBuilder(oparams);
            }
        }

        private void FromColor(float[] f, float f0, float f1, float f2, float f3)
        {
            f[0] = f0;
            f[1] = f1;
            f[2] = f2;
            f[3] = f3;
        }
    }
}
