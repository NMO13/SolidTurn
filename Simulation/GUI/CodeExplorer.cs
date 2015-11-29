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
    public partial class CodeExplorer : DockContent, INCCodeObserver
    {
        internal CodeExplorer()
        {
            InitializeComponent();
            textBox1.ReadOnly = true;
        }

        public void NewNCCode(object sender, object e)
        {
            textBox1.Clear();
            textBox1.Text = e as string;
        }
    }
}
