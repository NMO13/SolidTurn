using Builder;
using GeoObjectStuff;
using Mesh.GeoObjectStuff;
using Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesh.Builder
{
    internal class JawChuckBuilder
    {
        private JawChuck JawChuck;

        internal JawChuck BuildChawJuck()
        {
            JawChuck = new JawChuck();
            BuildJaw();
            BuildChucks();
            BuildRotatingJaw();
            return JawChuck;
        }

        private void BuildRotatingJaw()
        {
            GeoObjectBuilder b = new GeoObjectBuilder();
            object[] oparams = new object[3];
            oparams[0] = 0; // length
            oparams[1] = 40; // slice
            oparams[2] = 0; // radius
            JawChuck.RotatingJaws = b.BuildConcentricGeoObject(TemplateMeshes.RotatingJaw, oparams);
        }

        protected void BuildJaw()
        {
            GeoObjectBuilder b = new GeoObjectBuilder();
            object[] oparams = new object[3];
            oparams[0] = 0; // length
            oparams[1] = 40; // slice
            oparams[2] = 0; // radius
            JawChuck.Chuck = b.BuildConcentricGeoObject(TemplateMeshes.SimpleChawJuckCylinder, oparams);
        }

        protected void BuildChucks()
        {
            GeoObjectBuilder b = new GeoObjectBuilder();
            GeoObject chuck1 = b.BuildVBOGeoObject(TemplateMeshes.Chuck, null);
            GeoObject chuck2 = b.BuildVBOGeoObject(TemplateMeshes.Chuck, null);
            GeoObject chuck3 = b.BuildVBOGeoObject(TemplateMeshes.Chuck, null);
            GeoObject chuck4 = b.BuildVBOGeoObject(TemplateMeshes.Chuck, null);

            chuck1.Rotate(Math.PI / 2, new Geometry.Vector3D(0, 0, 1));
            chuck1.Translate(0, 150, 15);

            chuck2.Rotate(Math.PI / 2, new Geometry.Vector3D(0, 0, 1));
            chuck2.Rotate(Math.PI, new Geometry.Vector3D(1, 0, 0));
            chuck2.Translate(0, -150, -15);

            chuck3.Rotate(Math.PI / 2, new Geometry.Vector3D(0, 0, 1));
            chuck3.Rotate(Math.PI / 2, new Geometry.Vector3D(1, 0, 0));
            chuck3.Translate(0, 15, -150);


            chuck4.Rotate(Math.PI / 2, new Geometry.Vector3D(0, 0, 1));
            chuck4.Rotate(Math.PI / 2, new Geometry.Vector3D(-1, 0, 0));
            chuck4.Translate(0, -15, 150);

            JawChuck.MotionLessJaws.AddPart(chuck1);
            JawChuck.MotionLessJaws.AddPart(chuck2);
            JawChuck.MotionLessJaws.AddPart(chuck3);
            JawChuck.MotionLessJaws.AddPart(chuck4);
        }
    }
}
