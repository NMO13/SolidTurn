using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    interface IRender
    {
        void Render();
        bool ShouldRender();
        void RenderIt(bool v);
    }
}
