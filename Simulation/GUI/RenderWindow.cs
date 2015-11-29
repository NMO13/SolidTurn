using Simulation.CNC_Turning.Interpretation;
using Simulation.CNC_Turning.Machine_Stuff;
using Simulation.Machine_Stuff;
using Simulation.Model;
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
using Builder;
using WeifenLuo.WinFormsUI.Docking;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;
using System.Threading;

namespace Simulation
{
    public partial class RenderWindow : DockContent
    {
        private WindowMouseKeyEvents m_UserEvents = new WindowMouseKeyEvents();
        private SharpGLRenderer renderer;
        private bool loaded = false;
        private DocumentModel m_Doc;
        internal RenderWindow(DocumentModel doc)
        {
            this.m_Doc = doc;
            renderer = new SharpGLRenderer();
            doc.Renderer = renderer;
            InitializeComponent();
            this.MouseWheel += new MouseEventHandler(Form1_MouseWheel);
        }
        //----------------------- mouse and key events

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!loaded)
                return;
           m_UserEvents.MouseWheel(sender, e.Delta, renderer);
        }

        private void openGLControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (!loaded)
                return;
            if (e.Button == MouseButtons.Left)
                m_UserEvents.LeftMouseButtonUp(sender, e.Location, renderer);
            if (e.Button == MouseButtons.Right)
                m_UserEvents.RightMouseButtonUp(sender, e.Location);
            if(e.Button == MouseButtons.Middle)
                m_UserEvents.MiddleMouseButtonUp(sender, e.Location, renderer); 
        }

        private void openGLControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!loaded)
                return;
            m_UserEvents.MouseMove(sender, e.Location, renderer);
        }

        private void openGLControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!loaded)
                return;
            if (e.Button == MouseButtons.Left)
                m_UserEvents.LeftMouseButtonDown(sender, e.Location, renderer);
            else if (e.Button == MouseButtons.Right)
                m_UserEvents.RightMouseButtonDown(sender, e.Location);
            if (e.Button == MouseButtons.Middle)
                m_UserEvents.MiddleMouseButtonDown(sender, e.Location, renderer); 
        }

        private void openGLControl_Resized(object sender, EventArgs e)
        {
            if (!loaded)
                return;
            renderer.Resized(sender, e, openGLControl.Width, openGLControl.Height);
            openGLControl.Invalidate();
        }

        private void openGLControl_Load(object sender, EventArgs e)
        {
            renderer.Initialize();
            Application.Idle += Application_Idle;
            loaded = true;
            m_Doc.OGLContextCreated = true;
            m_Doc.OGLVersion = GL.GetString(StringName.Version);
        }

        void Application_Idle(object sender, EventArgs e)
        {
            // no guard needed -- we hooked into the event in Load handler

            // double milliseconds = Mesh.Rendering.PerformanceCounter.ComputeTimeSlice();
           // Mesh.Rendering.PerformanceCounter.Accumulate(milliseconds);
           // Render();
            openGLControl.Invalidate();
            
        }

        private void Render()
        {
            if (!loaded) // Play nice
                return;
            renderer.MainLoop();

            openGLControl.SwapBuffers();
        }

        private void openGLControl_Paint(object sender, PaintEventArgs e)
        {
            //long milliseconds = Mesh.Rendering.PerformanceCounter.ComputeTimeSlice();
            //if (milliseconds > 0)
            //    Console.WriteLine("FPS Rendering: {0}", 1000 / milliseconds);
            m_Doc.GlobalModel.ExManager.Process(Render, "LoggingAndReplacingException");
        }
    }
}
