using Geometry;
using Simulation.CNC_Turning.Code;
using Simulation.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Builder;
using GeoObjectStuff;
using System.Collections.Concurrent;
using System.Threading;
using Mesh.CNC_Turning.Machine_Stuff;
using Geometry.FloatingPointStuff;

namespace Simulation.Machine_Stuff
{
    class TrajectoryCalculator
    {
        protected readonly double STEPSIZE = 1;
        DocumentModel doc;
        private volatile bool m_ShouldStop = false;
        Vector3D absStartPos;
        Vector3D partZeroOffset = Vector3D.Zero();
        internal TrajectoryCalculator(DocumentModel d)
        {
            doc = d;
        }


        internal void Run()
        {
            absStartPos = doc.ToolSet.Origin.Clone() as Vector3D;
            Sentence prevCommand = null;
            ArcCalculator ac = new ArcCalculator();
            NCProgram program = doc.NCProgram;
            foreach (Sentence sentence in program.Sentences)
            {
                
                if (m_ShouldStop)
                    break;
                if (sentence.G[92])
                {
                    partZeroOffset = new Vector3D(sentence.X, sentence.Y, 0);
                }
                else if (sentence.T != -1)
                {
                    absStartPos = doc.ToolSet.Origin.Clone() as Vector3D;
                }
                switch (sentence.interpolationMode)
                {
                    case 0:
                    case 1: sentence.GoBuffer = new Queue<Vector3D>(); ProcessLine(sentence, sentence.GoBuffer); break;
                    case 2:
                    case 3: sentence.GoBuffer = new Queue<Vector3D>(); ac.ProcessCurve(sentence, prevCommand, sentence.GoBuffer, ref absStartPos); absStartPos += partZeroOffset; break;
                }
                prevCommand = sentence;
            }
        }

       
        protected void ProcessLine(Sentence command, Queue<Vector3D> buffer)
        {
            Vector3D absEndPos = new Vector3D(command.X, command.Y, 0);
            absEndPos += partZeroOffset;
            Vector3D trajectory = absEndPos - absStartPos;
            double length = trajectory.Length();
            if (EpsilonTests.IsNearlyZeroEpsHigh(length))
                return;
            double numSteps = length / STEPSIZE;
            Debug.Assert(numSteps > 0);

            long completeSteps = (long) Math.Floor(numSteps); //todo is this save?
            Vector3D amount = new Vector3D(trajectory.X / numSteps, trajectory.Y / numSteps, 0);
            absStartPos = absEndPos;

            Vector3D v = amount* completeSteps;
            double restStep = numSteps - completeSteps;

            Vector3D rest2 = amount * restStep; // only used to assert
            Vector3D rest = trajectory - v;
            Vector3D delta = rest2 - rest;
            Debug.Assert(EpsilonTests.IsNearlyZeroEpsHigh(Math.Abs(delta.X)));
            Debug.Assert(EpsilonTests.IsNearlyZeroEpsHigh(Math.Abs(delta.Y)));
            Debug.Assert(EpsilonTests.IsNearlyZeroEpsHigh(Math.Abs(delta.Z)));

            Debug.Assert(rest.Length() < amount.Length());
            for(int i = 0; i < completeSteps; i++)
                buffer.Enqueue(amount);

            if (EpsilonTests.IsGreaterEpsHigh(rest.Length()))
            {
                buffer.Enqueue(rest);
            }
        }

        internal void RequestStop()
        {
            m_ShouldStop = true;
        }
    }
}
