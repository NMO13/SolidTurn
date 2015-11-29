using Simulation.CNC_Turning.Code;
using Simulation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesh.CNC_Turning.Code.Sinumerik
{
    class SinumerikSentence : Sentence
    {
        internal GWord ActiveG = GWord.EMPTY;

        internal enum GWord
        {
            EMPTY, G0, G1, G2, G3
        }

        internal short TWord = -1;
        internal short SWord = 0;
        internal short FWord = 0;
        internal short NWord = 0;
        internal bool[] MWords = new bool[99];
        internal bool[] GWords = new bool[99];

        internal double XWord = 0.0;
        internal double YWord = 0.0;
        internal double IWord = 0.0;
        internal double KWord = 0.0;

        internal override double X { get { return XWord; } }
        internal override double Y { get { return YWord; } }
        internal override double I { get { return IWord; } }
        internal override double K { get { return KWord; } }
        internal override bool[] M { get { return MWords; } }
        internal override bool[] G { get { return GWords; } }
        internal override short T { get { return TWord; } }
        internal override short F { get { return FWord; } }
        internal override short S { get { return SWord; } }
        internal override short N { get { return NWord; } }

        internal override int interpolationMode 
        {

            get
            {
                if (GWords[0])
                    return 0;
                if (GWords[1])
                    return 1;
                if (GWords[2])
                    return 2;
                if (GWords[3])
                    return 3;
                return -1;
            }
        }
    }

    class SinumerikProgram : NCProgram
    {
        internal void CheckCorrectness(DocumentModel doc, List<Simulation.CNC_Turning.Interpretation.Interpreter.Error> errorList)
        {
            //TODO
            // check that program does not collide with juck jaw
            // check that tool number >= 0
            // check that tool exists
            // [Optional] check that program is not empty
            // check that spindle is on
            // check that tool active
        }

        //private void bool CheckProgramEnd()
        //{
        //    Sentence s = list.Last();
        //    SinumerikSentence ss = s as SinumerikSentence;
        //    if(ss
        //    foreach(Sentence s in list)
        //    {
                
        //        foreach(int m in ss.MWords)
        //        {
        //            if(m == 3
        //        }
        //    }
        //}
    }
}
