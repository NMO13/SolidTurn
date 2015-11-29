using GeoObjectStuff;
using Mesh.CNC_Turning.Machine_Stuff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mesh.GUI
{
    public partial class ToolDialog : Form
    {
        public ToolDialog()
        {
            InitializeComponent();
        }

        internal short[] SlotNames;
        internal string[] ToolNames;
        private bool m_InputValid = true;
        private void Button_OK_Click(object sender, EventArgs e)
        {
            bool valid = true;
            SlotNames = collectSlotNames(out valid);
            if (!valid)
            {
                m_InputValid = false;
                return;
            }
            ToolNames = collectToolNames(out valid);
            if (!valid)
                m_InputValid = false;
        }

        private short[] collectSlotNames(out bool valid)
        {
            short[] arr = new short[5];
            arr[0] = StringToShort(textBox1.Text, out valid);
            if (!valid)
                return null;
            arr[1] = StringToShort(textBox2.Text, out valid);
            if (!valid)
                return null;
            arr[2] = StringToShort(textBox3.Text, out valid);
            if (!valid)
                return null;
            arr[3] = StringToShort(textBox4.Text, out valid);
            if (!valid)
                return null;
            arr[4] = StringToShort(textBox5.Text, out valid);
            if (!valid)
                return null;
            return arr;
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

        private string FormatToolNumber(short number)
        {
            if (number > 9999 || number < 0)
                throw new ArgumentException("specified number is out of range");
            if (number < 10)
                return "000" + number.ToString();
            if (number < 100)
                return "00" + number.ToString();
            if (number < 1000)
                return "0" + number.ToString();
            return number.ToString();
        }

        private string[] collectToolNames(out bool valid)
        {
            valid = true;
            string[] arr = new string[5];
            arr[0] = comboBox1.Text;
            arr[1] = comboBox2.Text;
            arr[2] = comboBox3.Text;
            arr[3] = comboBox4.Text;
            arr[4] = comboBox5.Text;
            return arr;
        }

        internal void Initialize(ToolSet toolSet)
        {
            if (toolSet.Slots.Count > 0)
            {
                textBox1.Text = FormatToolNumber(toolSet.Slots[0].Name);
                BindToolList(comboBox1, toolSet, 0);
            }
            if (toolSet.Slots.Count > 1)
            {
                textBox2.Text = FormatToolNumber(toolSet.Slots[1].Name);
                BindToolList(comboBox2, toolSet, 1);
            }
            if (toolSet.Slots.Count > 2)
            {
                textBox3.Text = FormatToolNumber(toolSet.Slots[2].Name);
                BindToolList(comboBox3, toolSet, 2);
            }
            if (toolSet.Slots.Count > 3)
            {
                textBox4.Text = FormatToolNumber(toolSet.Slots[3].Name);
                BindToolList(comboBox4, toolSet, 3);
            }
            if (toolSet.Slots.Count > 4)
            {
                textBox5.Text = FormatToolNumber(toolSet.Slots[4].Name);
                BindToolList(comboBox5, toolSet, 4);
            }
        }

        private void BindToolList(ComboBox comboBox, ToolSet toolSet, int index)
        {
            List<string> tools = new List<string>();
            foreach (Tool t in toolSet.Tools)
                tools.Add(t.Name);
            comboBox.DataSource = tools;
            comboBox.SelectedItem = toolSet.Slots[index].Tool.Name;
        }

        private void ToolDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!m_InputValid)
            {
                e.Cancel = true;
                m_InputValid = true;
            }
        }
    }

    internal class Item 
    {
        internal Tool m_Name;

        internal Item(Tool name) 
        {
            m_Name = name; 
        }

        internal Tool Name { get { return m_Name; } }

    }   
}
