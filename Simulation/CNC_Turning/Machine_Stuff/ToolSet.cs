using Geometry;
using GeoObjectStuff;
using Simulation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesh.CNC_Turning.Machine_Stuff
{
    class ToolSet : ICoordinateSystemObserver
    {
        internal Slot ActiveSlot = new Slot(-1, null);

        private List<Slot> m_ToolSlots = new List<Slot>();
        private List<Tool> m_Tools = new List<Tool>();

        internal List<Slot> Slots { get { return m_ToolSlots; } }
        internal ToolSet()
        {
            
        }

        internal void Clear()
        {
            m_ToolSlots.Clear();
        }

        internal void AddToolSlot(Slot slot)
        {
            m_ToolSlots.Add(slot);
        }

        internal class Slot
        {
            internal short Name = -1;
            internal Tool Tool;
            internal Slot(short name, Tool t)
            {
                Name = name;
                Tool = t;
            }
        }

        internal void AddTool(Tool t)
        {
            m_Tools.Add(t);
            t.SetStartPoint(Origin);
        }

        internal List<Tool> Tools { get { return m_Tools; } } 

        internal Tool GetToolByName(string toolName)
        {
            foreach (Tool t in m_Tools)
            {
                if (t.Name.Equals(toolName))
                    return t;
            }
            return null;
        }

        internal Slot GetSlotByName(short p)
        {
            foreach (Slot s in m_ToolSlots)
            {
                if (s.Name == p)
                    return s;
            }
            return null;
        }

        internal Slot SetActiveSlot(short activeSlot)
        {
            Slot s = GetSlotByName(activeSlot);
            ActiveSlot = s;
            return s;
        }

        public Geometry.Vector3D Origin { get; private set; }

        public void CoordinateSystemChanged(object sender, object e)
        {
            object[] arr = e as object[];
            if ((CoordinateSystemType) arr[1] == CoordinateSystemType.TOOL_ZERO)
            {
                Origin = arr[0] as Vector3D;
            }
        }
    }
}
