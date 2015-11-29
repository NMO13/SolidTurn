using SharpGL;
using Simulation.GeoObject_Builder;
using Simulation.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Testing_Environment.src.GeoObjectBuilder;

namespace Simulation
{
    public partial class Form1 : Form
    {
        private List<SharpGLRenderer> m_RenderList = new List<SharpGLRenderer>();
        private RenderWindowMouseKeyEvents m_UserEvents = new RenderWindowMouseKeyEvents();
        public Form1()
        {
            // todo When clicking on "New" a new renderer should be created
            m_RenderList.Add(new SharpGLRenderer());
            InitializeComponent();
            this.MouseWheel += new MouseEventHandler(Form1_MouseWheel);
            m_RenderList[0].Doc.swivelInfoHandler += new SwivelInfoHandler(StatusStripObjectInfo);
            StatusStripObjectInfo(null, 0, 0, 0, 0);
        }

        private void openGLControl1_OpenGLDraw(object sender, SharpGL.RenderEventArgs args)
        {
            foreach (SharpGLRenderer r in m_RenderList)
                r.Draw();
        }

        private void openGLControl1_OpenGLInitialized(object sender, EventArgs e)
        {
            foreach (SharpGLRenderer r in m_RenderList)
                r.Initialize(this.openGLControl1.OpenGL);
        }

        private void openGLControl1_OpenGLResized(object sender, EventArgs e)
        {
            foreach (SharpGLRenderer r in m_RenderList)
                r.Resized(sender, e);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "xml files|*.xml";

            // Call the ShowDialog method to show the dialog box.
            DialogResult userClickedOK = openFileDialog1.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK == DialogResult.OK)
            {
                string file = openFileDialog1.FileName;
                try
                {
                    ToolReader r = new ToolReader(file);
                    XMLGeoObjectBuilder builder = new XMLGeoObjectBuilder(r);
                    m_RenderList[0].Doc.AddNewTool(builder.BuildMaterializedGeoObject(false, null));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        //----------------------- mouse and key events

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            m_UserEvents.MouseWheel(sender, e.Delta);
        }

        private void openGLControl1_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void openGLControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                m_UserEvents.LeftMouseButtonUp(sender, e.Location, m_RenderList[0]);
            if (e.Button == MouseButtons.Right)
                m_UserEvents.RightMouseButtonUp(sender, e.Location);
            if(e.Button == MouseButtons.Middle)
                m_UserEvents.MiddleMouseButtonUp(sender, e.Location, m_RenderList[0]); 
        }

        private void openGLControl1_MouseMove(object sender, MouseEventArgs e)
        {
            m_UserEvents.MouseMove(sender, e.Location, m_RenderList[0]);
        }

        private void openGLControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                m_UserEvents.LeftMouseButtonDown(sender, e.Location, m_RenderList[0]);
            else if (e.Button == MouseButtons.Right)
                m_UserEvents.RightMouseButtonDown(sender, e.Location);
            if (e.Button == MouseButtons.Middle)
                m_UserEvents.MiddleMouseButtonDown(sender, e.Location, m_RenderList[0]); 
        }

        private void createCylinderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Del handler = RoughPartMeshes.Cylinder;
            GeoObjectBuilder builder = new GeoObjectBuilder();
            object[] oparams = new object[3];
            oparams[0] = 40; // length
            oparams[1] = 40; // slice
            oparams[2] = 5; // radius
            
            m_RenderList[0].Doc.CreateSwivel(builder.BuildMaterializedGeoObject(false, handler, oparams));
        }

        private void wireframeViewToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            m_RenderList[0].Doc.SetWireFrameView((sender as ToolStripMenuItem).Checked);
        }

        private void StatusStripObjectInfo(object sender, int verts, int polys, int tris, int objects)
        {
            toolStripStatusLabel1.Text = "Verts " + verts + " | Polys " + polys + " | Tris " +
                 tris + " | Objects " + objects;
        }
    }
}
