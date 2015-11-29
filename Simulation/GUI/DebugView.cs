using Simulation.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Mesh.GUI
{
    public partial class DebugView : DockContent, IDebugMessageObserver
    {
        // This delegate enables asynchronous calls for setting
        // the text property on a TextBox control.
        delegate void SetTextCallback(string text);
        DocumentModel doc;
        internal DebugView(DocumentModel d)
        {
            InitializeComponent();
            doc = d;
            doc.GlobalModel.RestrictFrameRate = !checkBox1.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            doc.GlobalModel.RestrictFrameRate = !((CheckBox)sender).Checked;
        }

        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.outputMessageControl1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.outputMessageControl1.AddLine(text);
            }
        }

        public void NewDebugMessage(object sender, object e)
        {
            SetText(e as string);
        }
    }
}
