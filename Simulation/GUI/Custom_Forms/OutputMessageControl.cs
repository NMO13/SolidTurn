using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Simulation.Model;

namespace Mesh.GUI.Custom_Forms
{
    public partial class OutputMessageControl : UserControl
    {
        public OutputMessageControl()
        {
            InitializeComponent();
            textBox1.ReadOnly = true;
            textBox1.ScrollBars = ScrollBars.Vertical;
        }

        internal void AddLine(string s)
        {
            this.textBox1.AppendText(s);
        }

        internal void Clear()
        {
            this.textBox1.Clear();
        }
    }
}
