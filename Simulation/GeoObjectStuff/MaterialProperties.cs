using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoObjectStuff
{
    class MaterialProperties
    {
        public float[] ambient = new float[4];
        public float[] diffuse = new float[4];
        public float[] specular = new float[4];
        public float[] emission = new float[4];
        public float shininess = 0f;
    }
}
