using Geometry;
using OpenTK.Graphics.OpenGL;
using Simulation.CNC_Turning.Machine_Stuff;
using Simulation.Machine_Stuff;
using Simulation.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing_Environment.src.GeometryUtility;
using GeoObjectStuff;
using Mesh.Config;
using OpenTK;
using Builder;
using Mesh.GeoObjectStuff;
using Mesh.Builder;
using Mesh.CNC_Turning.Machine_Stuff;

namespace Simulation
{
    abstract class Renderer : IToolObserver, IRoughPartObserver, ISpindleStateObserver, IGlobalOffsetObserver, ICoordinateSystemObserver
    {
        protected TransformationMatrix m_WorldTransformation = new TransformationMatrix();
        protected bool m_WireframeView = false;
        internal SceneRotator WorldRotator = new SceneRotator();
        internal int Width { get; set; }
        internal int Height { get; set; }

        internal TransformationMatrix WorldTransformationMatrix
        {
            get { return m_WorldTransformation; }
            set { m_WorldTransformation = value; }
        }


        public abstract void SlotChanged(object sender, object e);

        public abstract void NewRoughPart(object sender, object e);

        public abstract void RemoveRoughPart(object sender, object e);

        public abstract void SpindleStart(object sender, object e);

        public abstract void SpindleStop(object sender, object e);

        public abstract void SetChuckPositions(object sender, object e);

        public abstract void GlobalOffsetChanged(object sender, object offset);

        public Microsoft.Practices.EnterpriseLibrary.Logging.LogWriter LogWriter { get; set; }

        public abstract void CoordinateSystemChanged(object sender, object e);
    }

    abstract class OpenGLRenderer : Renderer
    {
        internal Color BackgroundColor
        {
            get { return m_BackgroundColor; }
            set { m_BackgroundColor = value; }
        }
        private Color m_BackgroundColor = Color.CornflowerBlue;
    }

    class SharpGLRenderer : OpenGLRenderer
    {
        private double m_ZoomFactor = 0;
        private Vector3D m_GlobalOffset = new Vector3D(0, 0, 0);
        private List<IRender> m_RenderableModels = new List<IRender>();
        private List<GeoObject> m_Parts = new List<GeoObject>();
        GeoObject ActiveTool;
        private System.Drawing.Font font = new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, 8.0f);
        private OGLCoordinateAxis machineZero;
        private OGLCoordinateAxis partZero;
        private OGLCoordinateAxis toolZero;
        private JawChuck m_JawChuck;

        internal SharpGLRenderer()
        {
            machineZero = new OGLCoordinateAxis(Vector3D.Zero());
            partZero = new OGLCoordinateAxis(Vector3D.Zero());
            toolZero = new OGLCoordinateAxis(Vector3D.Zero());
        }

        private IRender CreateJawChuck()
        {
            GeoObjectBuilder b = new GeoObjectBuilder();
            object[] oparams = new object[3];
            oparams[0] = Initializer.RoughPartLength; // length
            oparams[1] = 40; // Properties.RoughPartSlice; // slice
            oparams[2] = Initializer.RoughPartRadius; // radius
            return b.BuildConcentricGeoObject(TemplateMeshes.SimpleChawJuckCylinder, oparams);
        }

        internal double ZoomFactor
        {
            get { return m_ZoomFactor; }
            set { m_ZoomFactor = value; this.Resized(null, EventArgs.Empty, Width, Height); }
        }

        internal void MainLoop()
        {
            Draw();
        }


        protected void Draw()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            WorldTransformationMatrix.SetIdentity();
            WorldTransformationMatrix.Translate(m_GlobalOffset);
            WorldTransformationMatrix.Translate(new Vector3D(0, 0, m_ZoomFactor));
            TransformationMatrix rotationMatrix = WorldRotator.GetRotationMatrix();
            WorldTransformationMatrix = WorldTransformationMatrix * rotationMatrix;
            GL.MultMatrix(TransformationMatrix.MatrixToColumnMajorArray(WorldTransformationMatrix));
            RenderGeoObjects();
            RenderOtherStuff();
            GL.PopMatrix();
            GL.Finish();
        }

        protected void RenderGeoObjects()
        {
            if (ActiveTool != null)
            {
                SetWireframeViewAndSelection(ActiveTool);
                if (ActiveTool.ShouldRender())
                    ActiveTool.Render();
            }
            foreach (GeoObject part in m_Parts)
            {
                SetWireframeViewAndSelection(part);
                if (part.ShouldRender())
                    part.Render();
            }

        }

        protected void RenderOtherStuff()
        {
            foreach (IRender r in m_RenderableModels)
            {
                if (r.ShouldRender())
                    r.Render();
            }
        }

        protected void SetWireframeViewAndSelection(GeoObject o)
        {
            if (o.WireframeView)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                GL.Disable(EnableCap.Lighting);
                if (o.IsSelected)
                {
                    GL.Enable(EnableCap.ColorMaterial);
                    GL.Color3(0.5, 1, 0);
                }
                else
                {
                    GL.Disable(EnableCap.ColorMaterial);
                    GL.Color3(1.0, 0.0, 0.0);
                }
            }
            else
            {
                GL.Enable(EnableCap.Lighting);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                if (o.IsSelected)
                {
                    GL.Enable(EnableCap.ColorMaterial);
                    GL.Color3(0.5, 1, 0);
                }
                else
                {
                    GL.Disable(EnableCap.ColorMaterial);
                }
            }
        }


        internal void Initialize()
        {
            LogWriter.Write("OpenGL Version: " + GL.GetString(StringName.Version) + Environment.NewLine, "Output", 0);
            GL.ClearColor(BackgroundColor);
            GL.ShadeModel(ShadingModel.Smooth);
            GL.Enable(EnableCap.DepthTest);
            SetUpLighting();
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            m_ZoomFactor = Initializer.StartZoom;
            AddRenderableModel(machineZero);
            AddRenderableModel(partZero);
            AddRenderableModel(toolZero);

            JawChuckBuilder b = new JawChuckBuilder();
            m_JawChuck = b.BuildChawJuck();
            AddRenderableModel(m_JawChuck);
            OpenTK.Graphics.GraphicsContext.CurrentContext.VSync = true;
        }

        internal void Resized(object sender, EventArgs e, int width, int height)
        {
           //  Load and clear the projection matrix.
            GL.MatrixMode(MatrixMode.Projection);
           // GL.LoadIdentity();

            double r = ((double)width / (double)height);
            Matrix4d pers = Matrix4d.CreatePerspectiveFieldOfView(0.45*(1.5/r), r ,
               500, 2000.0);
            GL.LoadMatrix(ref pers);
            GL.Viewport(0, 0, width, height);
            // Load the modelview.
            GL.MatrixMode(MatrixMode.Modelview);
            Width = width;
            Height = height;
            WorldRotator.SetBounds(Width, Height);
        }

        protected void SetUpLighting()
        {
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            //m_GL.Enable(OpenGL.GL_LIGHT1);
            GL.LightModel(LightModelParameter.LightModelTwoSide, 1);
            OpenTK.Vector4 ambient = new OpenTK.Vector4(0.0f, 0.0f, 0.0f, 1.0f);
            OpenTK.Vector4 diffuse = new OpenTK.Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            OpenTK.Vector4 specular = new OpenTK.Vector4(0.3f, 0.3f, 0.3f, 1.0f);
            OpenTK.Vector4 position0 = new OpenTK.Vector4(0.0f, 0.0f, 1.0f, 0.0f);


            GL.Light(LightName.Light0, LightParameter.Ambient, ambient);
            GL.Light(LightName.Light0, LightParameter.Diffuse, diffuse);
            GL.Light(LightName.Light0, LightParameter.Specular, specular);
            GL.Light(LightName.Light0, LightParameter.Position, position0);
            //m_GL.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, ambient);
            //m_GL.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, diffuse);
            //m_GL.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPECULAR, specular);
            //m_GL.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, position1);
        }

        internal void AddRenderableModel(IRender r)
        {
            m_RenderableModels.Add(r);
        }

        internal void WireframeView(object sender, bool state)
        {
            if(ActiveTool != null)
                ActiveTool.WireframeView = state;

            foreach (GeoObject o in m_Parts)
                o.WireframeView = state;

            this.m_WireframeView = state;
        }

        public override void SlotChanged(object sender, object e)
        {
            Mesh.CNC_Turning.Machine_Stuff.ToolSet.Slot slot = e as Mesh.CNC_Turning.Machine_Stuff.ToolSet.Slot;
            if (slot != null)
            {
                Tool tool = slot.Tool;
                tool.WireframeView = this.m_WireframeView;
                ActiveTool = tool;
            }
            else
                ActiveTool = null;
        }

        public override void NewRoughPart(object sender, object e)
        {
            GeoObject roughPart = e as GeoObject;
            roughPart.WireframeView = this.m_WireframeView;
            m_Parts.Add(roughPart);
        }

        public override void RemoveRoughPart(object sender, object e)
        {
            GeoObject roughPart = e as GeoObject;
            m_Parts.Remove(roughPart);
        }

        public override void CoordinateSystemChanged(object sender, object e1)
        {
            object[] e = e1 as object[];
            CoordinateSystemType systemType = (CoordinateSystemType) e[1];
            if (systemType == CoordinateSystemType.PART_ZERO)
                partZero.Offset += e[0] as Vector3D;
            else if (systemType == CoordinateSystemType.TOOL_ZERO)
                toolZero.Offset += e[0] as Vector3D;
            else
                throw new ArgumentException("Invalid coordinate system");
        }

        public override void SpindleStart(object sender, object e)
        {
            m_JawChuck.Start();
        }

        public override void SpindleStop(object sender, object e)
        {
            m_JawChuck.Stop();
        }

        public override void SetChuckPositions(object sender, object e)
        {
            m_JawChuck.SetJawsPosition((double)e);
        }

        public override void GlobalOffsetChanged(object sender, object offset)
        {
            m_GlobalOffset = offset as Vector3D;
        }
    }
}
