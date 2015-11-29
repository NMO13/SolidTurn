using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesh.CNC_Turning.Machine_Stuff
{
    internal enum CoordinateSystemType { PART_ZERO, TOOL_ZERO, MACHINE_ZERO }
    
    [Flags]
    internal enum AnimationState : byte { RUNNING = 0x0, PAUSED = 0x1, STOPPED = 0x2, FINISHED=0x4, CONTINIUE=0x8}
}
