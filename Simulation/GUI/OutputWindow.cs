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

namespace Simulation.GUI
{
    // This delegate enables asynchronous calls for setting
    // the text property on a TextBox control.
    delegate void SetTextCallback(string text);

    public partial class OutputWindow : DockContent, IOutputObserver
    {
        internal OutputWindow()
        {
            InitializeComponent();
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

        public void NewOutputMessage(object sender, object e)
        {
            SetText(e as string);
        }
    }
}
