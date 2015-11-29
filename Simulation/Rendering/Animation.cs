using Geometry;
using Mesh.CNC_Turning.Machine_Stuff;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Simulation.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mesh.Rendering
{
    // create Animation in document
    // get processor
    // get document
    // calc new values in thread
    // send value to document
    // document notifies listeners (renderer)
    // hence, main thread will be unburdened
    // taken from http://www.koonsolo.com/news/dewitters-gameloop/
    class Animation
    {
        private Simulation.Model.DocumentModel m_DocumentModel;
        private Object m_AnimationLock;
        private const int SKIP_TICKS = 1000 / 30;
        private const int MAX_FRAMESKIP = 10;
        private static Stopwatch sw = new Stopwatch();
        public Animation(DocumentModel documentModel, Object animationLock)
        {
            m_DocumentModel = documentModel;
            m_AnimationLock = animationLock;
            Mesh.Rendering.PerformanceCounter.Reset();
        }

        internal void Run()
        {
            m_DocumentModel.GlobalModel.ExManager.Process(() => 
                {
                    sw.Start();
                    long nextGameTick = sw.ElapsedMilliseconds;
                    while (m_DocumentModel.AnimationState == AnimationState.RUNNING || m_DocumentModel.AnimationState == AnimationState.PAUSED)
                    {
                        lock (m_AnimationLock) // has to be locked since, VBOManager will be written here
                        {
                            long milliseconds = Mesh.Rendering.PerformanceCounter.ComputeTimeSlice();
                            if (milliseconds > 0)
                                Console.WriteLine("FPS Animation: {0}", 1000 / milliseconds);
                            int loops = 0;
                            if (m_DocumentModel.GlobalModel.RestrictFrameRate)
                            {
                                while (sw.ElapsedMilliseconds > nextGameTick && loops < MAX_FRAMESKIP && m_DocumentModel.AnimationState == AnimationState.RUNNING)
                                {
                                    m_DocumentModel.Processor.ProcessNextFrame();
                                    nextGameTick += SKIP_TICKS;
                                    loops++;
                                }
                            }
                            else
                            {
                                if(m_DocumentModel.AnimationState == AnimationState.RUNNING)
                                {
                                    m_DocumentModel.Processor.ProcessNextFrame();
                                    nextGameTick += SKIP_TICKS;
                                }
                            }
                        }
                        while (m_DocumentModel.AnimationState == AnimationState.PAUSED) { nextGameTick = sw.ElapsedMilliseconds; }
                    }
                }, "LoggingAndReplacingException");
            // Can only be outputted to file, otherwise the thread.join would lead this to a deadlock
            m_DocumentModel.GlobalModel.OutputWriter.Write("Render thread finished" + Environment.NewLine, "OutputToFile");
        }
    }
}
