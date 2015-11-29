using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesh.Rendering
{
    class PerformanceCounter
    {
        private static double accumulator = 0;
        private static int idleCounter = 0;
        private static Stopwatch sw = new Stopwatch();

        static PerformanceCounter()
        {
            if (!Stopwatch.IsHighResolution)
                throw new Exception("No high resolution timer found. This can cause inaccuracies!");
        }

        public static void Reset()
        {
            accumulator = 0;
            idleCounter = 0;
            sw.Reset();
        }

        public static long ComputeTimeSlice()
        {
            sw.Stop();
            long timeslice = sw.ElapsedMilliseconds;
            sw.Reset();
            sw.Start();
            return timeslice;
        }

        public static int Accumulate(double milliseconds)
        {
            idleCounter++;
            accumulator += milliseconds;
            int fps = -1;
            if (accumulator > 1000)
            {
                Console.WriteLine(idleCounter.ToString());
                fps = idleCounter;
                accumulator -= 1000;
                idleCounter = 0; // don't forget to reset the counter!
            }
            return fps;
        }
    }
}
