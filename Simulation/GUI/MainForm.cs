using Geometry;
using Mesh.Config;
using OpenTK.Graphics.OpenGL;
using Simulation.CNC_Turning;
using Simulation.CNC_Turning.Code;
using Simulation.CNC_Turning.Interpretation;
using Simulation.CNC_Turning.Machine_Stuff;
using Builder;
using GeoObjectStuff;
using Simulation.Machine_Stuff;
using Simulation.Model;
using Simulation.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Mesh.GUI;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Logging;
using Mesh.third_party.Enterprise_Blocks;
using Mesh.CNC_Turning.Machine_Stuff;

namespace Simulation.GUI
{
    internal partial class MainForm : Form, IAnimationStateObserver
    {
        private GlobalStateModel m_GlobalModel = null;
        internal DocumentModel m_Document = null;
        DebugView f4;
        public MainForm()
        {
            try // can't be handled by exception manager because it doesn't exist yet
            {
                InitializeComponent();

                m_GlobalModel = new GlobalStateModel();
                m_Document = new DocumentModel();
                m_Document.GlobalModel = m_GlobalModel;
                m_GlobalModel.InitEnterpriseBlocks(m_Document);
            }
            catch (Exception e)
            {
                throw e;
            }
            m_GlobalModel.ExManager.Process(() =>
            {
                Process.GetCurrentProcess().PriorityClass =
       ProcessPriorityClass.High;  	// Prevents "Normal" processes 

                OutputWindow f1 = new OutputWindow();
                f1.Show(dockPanel, DockState.DockBottom);
                CodeExplorer f3 = new CodeExplorer();
                f3.Show(dockPanel, DockState.DockRight);
                RenderWindow f2 = new RenderWindow(m_Document);
                f2.CloseButtonVisible = false;
                f2.Show(dockPanel, DockState.Document);
                f4 = new DebugView(m_Document);
                f4.Show(dockPanel, DockState.DockRight);
                f4.Hide();
                ActionView f5 = new ActionView();
                f5.Show(dockPanel, DockState.DockRight);
                f5.Hide();
                m_GlobalModel.AddOutputObserver(f1);
                m_GlobalModel.AddDebugMessageObserver(f4);
                m_Document.AddNCTextObserver(f3);
                m_Document.ReadConfigFile();
                m_Document.AddAnimationStateObserver(this);
                m_GlobalModel.OutputWriter.Write("Application started" + Environment.NewLine, "Output", 0);
            }, "LoggingAndReplacingException"); 


        }

        private void programToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_GlobalModel.ExManager.Process(() =>
                {
                    OpenFileDialog openFileDialog1 = new OpenFileDialog();
                    openFileDialog1.Filter = "cnc files|*.cnc";

                    // Call the ShowDialog method to show the dialog box.
                    DialogResult userClickedOK = openFileDialog1.ShowDialog();

                    // Process input if the user clicked OK.
                    if (userClickedOK == DialogResult.OK)
                    {
                        m_Document.ReadCNCProgram(openFileDialog1.FileName);
                    }
                }, "LoggingAndReplacingException");
        }

        private void createCylinderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_GlobalModel.ExManager.Process(() =>
                {
                    RoughPartSpecDialog d = new RoughPartSpecDialog();
                    d.Initialize(Initializer.RoughPartLength, Initializer.RoughPartRadius);
                    if (d.ShowDialog() == DialogResult.OK)
                    {
                        m_Document.SetRoughPartParams(d.Lengthd, d.Radiusd);
                        m_Document.CreateRoughPart();
                    }
                    
                }, "LoggingAndReplacingException");
        }


        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_GlobalModel.ExManager.Process(m_Document.Quit, "LoggingAndReplacingException");
        }

        private void toolStripInterpret_Click(object sender, EventArgs e)
        {
            m_GlobalModel.ExManager.Process(() =>
            {
                m_Document.InterpretCNCProgram();
            }, "LoggingAndReplacingException");
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            m_GlobalModel.ExManager.Process(() =>
            {
                ToolDialog toolDialog = new ToolDialog();
                toolDialog.Initialize(m_Document.ToolSet);
                if (toolDialog.ShowDialog(this) == DialogResult.OK)
                {
                    m_Document.InitializeToolSet(toolDialog.SlotNames, toolDialog.ToolNames);
                }
                toolDialog.Dispose();
            }, "LoggingAndReplacingException");
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.D))
            {
                f4.Show();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }


        private void startToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (m_Document.AnimationState == AnimationState.RUNNING)
            {
                m_GlobalModel.ExManager.Process(() => m_Document.ChangeState(AnimationState.PAUSED), "LoggingAndReplacingException");
            }
            else if(m_Document.AnimationState == AnimationState.STOPPED)
            {
                m_GlobalModel.ExManager.Process(() => m_Document.ChangeState(AnimationState.RUNNING), "LoggingAndReplacingException");
            }
            else if (m_Document.AnimationState == AnimationState.PAUSED)
            {
                m_GlobalModel.ExManager.Process(() => m_Document.ChangeState(AnimationState.CONTINIUE), "LoggingAndReplacingException");
            }
        }

        private void toolStripStart_Click(object sender, EventArgs e)
        {
            startToolStripMenuItem1_Click(sender, e);
        }

        private void stopToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            m_GlobalModel.ExManager.Process(() => m_Document.ChangeState(AnimationState.STOPPED), "LoggingAndReplacingException");
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stopToolStripMenuItem1_Click(sender, e);
        }

        public void AnimationStateChanged(object sender, AnimationState e)
        {
            if (e == AnimationState.RUNNING)
            {
                startToolStripMenuItem1.Image = Mesh.Properties.Resources.Breakall_6323;
                startToolStripLabel1.Image = Mesh.Properties.Resources.Breakall_6323;
                toolStripButton3.Enabled = false;
                toolStripButton4.Enabled = false;
                parseCNCProgramToolStripMenuItem.Enabled = false;
            }
            else if (m_Document.AnimationState == AnimationState.PAUSED)
            {
                startToolStripMenuItem1.Image = Mesh.Properties.Resources.Symbols_Play_32xLG;
                startToolStripLabel1.Image = Mesh.Properties.Resources.Symbols_Play_32xLG;
            }
            else if (m_Document.AnimationState == AnimationState.STOPPED)
            {
                startToolStripMenuItem1.Image = Mesh.Properties.Resources.Symbols_Play_32xLG;
                startToolStripLabel1.Image = Mesh.Properties.Resources.Symbols_Play_32xLG;
                toolStripButton3.Enabled = true;
                toolStripButton4.Enabled = true;
                parseCNCProgramToolStripMenuItem.Enabled = true;
            }
        }
    }
}
