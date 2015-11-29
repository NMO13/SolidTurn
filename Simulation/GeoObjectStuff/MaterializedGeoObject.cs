using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoObjectStuff;
using OpenTK.Graphics.OpenGL;
namespace GeoObjectStuff
{
    class MaterializedGeoObject : GeoObject
    {
        public MaterialProperties MaterialProps { get; set; }
        public MaterializedGeoObject(uint p) : base(p)
        {
            MaterialProps = new MaterialProperties();
        }

        protected override void RenderMesh()
        {
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, MaterialProps.ambient);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, MaterialProps.diffuse);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, MaterialProps.specular);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, MaterialProps.emission);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, MaterialProps.shininess);
        }
    }
}
