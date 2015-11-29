using Simulation.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mesh.GUI
{
    public partial class RoughPartSpecDialog : Form
    {
        private bool m_InputValid = true;
        public RoughPartSpecDialog()
        {
            InitializeComponent();
        }

        private short StringToShort(string input, out bool valid)
        {
            short numVal = -1;
            // ToInt32 can throw FormatException or OverflowException. 
            // todo
            try
            {
                numVal = Convert.ToInt16(input);
                valid = true;
            }
            catch (FormatException)
            {
                valid = false;
            }
            catch (OverflowException)
            {
                valid = false;
            }
            return numVal;
        }

        private double StringToDouble(string input, out bool valid)
        {
            double numVal = -1;
            if (Double.TryParse(input, NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-US"), out numVal))
                valid = true;
            else
                valid = false;
            return numVal;
        }

        private void RoughPartSpec_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!m_InputValid)
            {
                e.Cancel = true;
                m_InputValid = true;
            }
        }

        private void Button_OK_Click(object sender, EventArgs e)
        {
            bool valid = true;
            Lengthd = StringToDouble(textBox1.Text, out valid);
            if (!valid)
            {
                m_InputValid = false;
                return;
            }
            Radiusd = StringToDouble(textBox2.Text, out valid);
            if (!valid)
                m_InputValid = false;
        }

        internal void Initialize(double length, double radius)
        {
            Lengthd = length;
            Radiusd = radius;
            textBox1.Text = Lengthd.ToString();
            textBox2.Text = Radiusd.ToString();
        }

        internal double Lengthd { get; private set; }
        internal double Radiusd { get; private set; }
    }
}
