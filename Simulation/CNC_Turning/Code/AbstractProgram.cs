using Geometry;
using Mesh;
using Simulation.Machine_Stuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.CNC_Turning.Code
{
    abstract class Sentence
    {
        abstract internal double X { get; }
        abstract internal double Y { get; }
        abstract internal double I { get; }
        abstract internal double K { get; }
        abstract internal int interpolationMode { get; }
        abstract internal bool[] G { get; }
        abstract internal bool[] M { get; }
        abstract internal short T { get; }
        abstract internal short F { get; }
        abstract internal short S { get; }
        abstract internal short N { get; }

        internal Queue<Vector3D> GoBuffer = null;
    }

    abstract class NCProgram
    {
        //public int PathCounter = 0;
        protected List<Sentence> list = new List<Sentence>();
        internal void AddSentence(Sentence s)
        {
            list.Add(s);
        }

        internal List<Sentence> Sentences { get { return list; } }
    }
}
