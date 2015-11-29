using Simulation.CNC_Turning.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesh.CNC_Turning.Machine_Stuff
{
    class CNCCommandProcessor
    {
        CNCProgram m_Program;

        protected void SetProgram(object sender, CNCProgram prog)
        {
            m_Program = prog;
            Reset();
        }

        internal void Run()
        {
        }

        internal void Reset()
        {
            m_TrajCalculator = new TrajectoryCalculator(doc);
        }
    }
}
