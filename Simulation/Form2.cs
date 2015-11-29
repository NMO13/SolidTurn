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

namespace Simulation
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            Form1 f1 = new Form1();
            f1.Show(dockPanel1, DockState.DockLeft);
        }
    }
}
