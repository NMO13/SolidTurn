using Geometry;
using Geometry.FloatingPointStuff;
using Simulation.CNC_Turning.Code;
using Simulation.Machine_Stuff;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mesh.CNC_Turning.Machine_Stuff
{
    /*
     * author:      Florian Ennemoser
     * desc:        Utility class to calculate circular tool motions
     */
   
    class ArcCalculator
    {

        public ArcCalculator()
        {
        }

        /*
        * author:      Florian Ennemoser
        * desc:        Calculates the values for a circle segment.
        * parameters:  curCommand: The currend cnc command including x,y,i and k values
        *              prevCommand: The previous command where the tool is currently positioned
        */
        public void ProcessCurve(Sentence curCommand, Sentence prevCommand, Queue<Vector3D> goBuffer, ref Vector3D absStart)
        {
            if (EpsilonTests.IsNearlyZeroEpsHigh(curCommand.I) && EpsilonTests.IsNearlyZeroEpsHigh(curCommand.K))
                throw new Exception("Either I or K must be greater/less than zero");

            double l = 0.00; // the distance between the start point on the circle segment and the end point on the circle segment
            double r = Math.Sqrt(Math.Abs(curCommand.I) * Math.Abs(curCommand.I) + Math.Abs(curCommand.K) * Math.Abs(curCommand.K)); // circle radius
            double arc = 0.00; //the arc

            if (EpsilonTests.IsNearlyZeroEpsHigh(prevCommand.Y - curCommand.Y)) //special case: angle is 180°
            {
                if (EpsilonTests.IsNearlyZeroEpsHigh(curCommand.I)) //circle origin lies directly on the horizontal axis of start- end endpoint of circle
                {
                    arc = 2 * r * Math.PI * 0.5;
                }
                else
                {
                    l = Math.Abs(prevCommand.X - curCommand.X);
                    arc = Math.Acos(1 - Math.Pow(l, 2) / (2 * Math.Pow(r, 2))) * r;
                }
            }
            else if (EpsilonTests.IsNearlyZeroEpsHigh(Math.Abs(prevCommand.X - curCommand.X) - Math.Abs(curCommand.K)) && EpsilonTests.IsNearlyZeroEpsHigh(Math.Abs(curCommand.I))) //special case: angle is 90° or 270°
            {
                if(IsGreaterHalfCircle())
                    arc = 2 * r * Math.PI * 0.75;
                else
                    arc = 2 * r * Math.PI * 0.25;
            }
            else //angle is ]90°,180°[
            {
                l = Math.Sqrt(Math.Pow(Math.Abs(prevCommand.Y) - Math.Abs(curCommand.Y), 2) + Math.Pow(Math.Abs(prevCommand.X) - Math.Abs(curCommand.X), 2));
                if (IsGreaterHalfCircle())
                    arc = 2 * r * Math.PI -  Math.Acos(1 - Math.Pow(l, 2) / (2 * Math.Pow(r, 2))) * r;
                else
                    arc = Math.Acos(1 - Math.Pow(l, 2) / (2 * Math.Pow(r, 2))) * r;
            }


            Debug.Assert(arc > 0, "The arc must be greater than 0");
            Debug.Assert(r > 0, "The radius must be greater than 0");

           // double segment = 12.566370614359172953850573533118;
           // double segment = 6.28139;
            double segment = 0.2;
            int counter = 1;
            double xPrev = Math.Abs(curCommand.K); //x-distance from circle center point to tool-position of last frame 
            double yPrev = Math.Abs(curCommand.I); //y-distance from circle center point to tool-position of last frame 
            Quadrant startQuadrant = GetQuadrant(curCommand.I, curCommand.K); //quadrant where arc starts
            Quadrant curQuadrant = startQuadrant;
            Quadrant prevQuadrant = startQuadrant;
            double alpha = CalcStartAngle(curCommand.interpolationMode, r, startQuadrant, curCommand.I);
            Vector3D amount;

            while (segment * counter < arc || EpsilonTests.IsNearlyZeroEpsHigh(arc - segment * counter))
            {
                alpha += segment / r;
                GetQuadrant(alpha, startQuadrant, ref curQuadrant, ref prevQuadrant, curCommand.interpolationMode);
                double deltaX = calcX(ref xPrev, alpha, r, startQuadrant, curQuadrant, prevQuadrant, curCommand.interpolationMode);
                double deltaY = calcY(ref yPrev, alpha, r, startQuadrant, curQuadrant, prevQuadrant, curCommand.interpolationMode);
                prevQuadrant = curQuadrant;
                amount = new Vector3D(deltaX,deltaY,0);

                if (!EpsilonTests.IsGreaterEpsHigh(amount.Length()))
                    throw new Exception("Length too small: " + amount.Length());
                goBuffer.Enqueue(amount);

                counter++;
            }

            double arcLeft = arc - segment * (counter-1);
            if (!EpsilonTests.IsNearlyZeroEpsHigh(arcLeft))
            {
                alpha += arcLeft / r;
                GetQuadrant(alpha, startQuadrant, ref curQuadrant, ref prevQuadrant, curCommand.interpolationMode);
                double deltaX = calcX(ref xPrev, alpha, r, startQuadrant,  curQuadrant, prevQuadrant, curCommand.interpolationMode);
                double deltaY = calcY(ref yPrev, alpha, r, startQuadrant, curQuadrant, prevQuadrant, curCommand.interpolationMode);
                amount = new Vector3D(deltaX, deltaY, 0);
                double l1 = amount.Length();
                if (amount.Length() >= 0.0001)
                    goBuffer.Enqueue(amount);
                else
                {
                    Vector3D last = goBuffer.Last();
                    last.X += amount.X;
                    last.Y += amount.Y;
                    Debug.Assert(last.Z == 0);
                }
            }

#if DEBUG
            double overallX = 0;
            double overallY = 0;
            foreach (Vector3D v in goBuffer)
            {
                overallX += v.X;
                overallY += v.Y;
            }
#endif
            absStart = new Vector3D(curCommand.X,curCommand.Y,0);
        }

        /*
        * author:      Florian Ennemoser
        * desc:        Calculates the x-difference between two consecutive x-coordinates
        */
        private double calcX(ref double xPrev, double alpha, double r, Quadrant startQuadrant, Quadrant curQuadrant, Quadrant prevQuadrant, int interpolationMode)
        {
            double deltaX = 0;
            double curX = 0;

            if (((startQuadrant == Quadrant.UPPER_LEFT || startQuadrant == Quadrant.LOWER_RIGHT) && interpolationMode == 3) ||
                ((startQuadrant == Quadrant.UPPER_RIGHT || startQuadrant == Quadrant.LOWER_LEFT) && interpolationMode == 2) ||
                startQuadrant == Quadrant.NONE_VERTICAL_NORTH || startQuadrant == Quadrant.NONE_VERTICAL_SOUTH)
                curX = Math.Abs(r * Math.Sin(alpha));
            else
                curX = Math.Abs(r * Math.Cos(alpha));

            if (curQuadrant != prevQuadrant) //If tool moves from upper left quadrant to upper right-quadrant or vice-verca, distances have to be summed up
            {
                if ((curQuadrant == Quadrant.UPPER_RIGHT && interpolationMode == 2) || (curQuadrant == Quadrant.LOWER_RIGHT && interpolationMode == 3))
                    deltaX = curX + xPrev;
                else if ((curQuadrant == Quadrant.UPPER_LEFT && interpolationMode == 3) || (curQuadrant == Quadrant.LOWER_LEFT && interpolationMode == 2))
                    deltaX = -(curX + xPrev);
                else
                {
                    if (curQuadrant == Quadrant.UPPER_RIGHT || curQuadrant == Quadrant.LOWER_RIGHT || curQuadrant == Quadrant.NONE_HORIZONTAL_EAST ||
                        (curQuadrant == Quadrant.NONE_VERTICAL_NORTH && interpolationMode == 3) || (curQuadrant == Quadrant.NONE_VERTICAL_SOUTH && interpolationMode == 2))
                        deltaX = curX - xPrev;
                    else
                        deltaX = xPrev - curX;
                }
            }
            else
            {
                if (curQuadrant == Quadrant.UPPER_RIGHT || curQuadrant == Quadrant.LOWER_RIGHT || curQuadrant == Quadrant.NONE_HORIZONTAL_EAST ||
                   (curQuadrant == Quadrant.NONE_VERTICAL_NORTH && interpolationMode == 3) || (curQuadrant == Quadrant.NONE_VERTICAL_SOUTH && interpolationMode == 2))
                    deltaX = curX - xPrev;
                else
                    deltaX = xPrev - curX;
            }

            xPrev = curX;
            return deltaX;
        }

        /*
        * author:      Florian Ennemoser
        * desc:        Calculates the ydifference between two consecutive y-coordinates
        */
        private double calcY(ref double yPrev, double alpha, double r, Quadrant startQuadrant, Quadrant curQuadrant, Quadrant prevQuadrant, int interpolationMode)
        {

            double deltaY = 0;
            double curY = 0;

            if (((startQuadrant == Quadrant.UPPER_LEFT || startQuadrant == Quadrant.LOWER_RIGHT) && interpolationMode == 3) ||
                ((startQuadrant == Quadrant.UPPER_RIGHT || startQuadrant == Quadrant.LOWER_LEFT) && interpolationMode == 2) ||
                startQuadrant == Quadrant.NONE_VERTICAL_NORTH || startQuadrant == Quadrant.NONE_VERTICAL_SOUTH)
                curY = Math.Abs(r * Math.Cos(alpha));
            else
                curY = Math.Abs(r * Math.Sin(alpha));

            if (curQuadrant != prevQuadrant) //If tool moves from upper left quadrant to lower left-quadrant or vice-verca, distances have to be summed up
            {
                if ((curQuadrant == Quadrant.LOWER_RIGHT && interpolationMode == 2) || (curQuadrant == Quadrant.LOWER_LEFT && interpolationMode == 3))
                    deltaY = -(curY + yPrev);
                else if ((curQuadrant == Quadrant.UPPER_RIGHT && interpolationMode == 3) || (curQuadrant == Quadrant.UPPER_LEFT && interpolationMode == 2))
                    deltaY = curY + yPrev;
                else
                {
                    if(curQuadrant == Quadrant.UPPER_RIGHT || curQuadrant == Quadrant.UPPER_LEFT || curQuadrant == Quadrant.NONE_VERTICAL_NORTH ||
                       (curQuadrant == Quadrant.NONE_HORIZONTAL_WEST && interpolationMode == 3) || (curQuadrant == Quadrant.NONE_HORIZONTAL_EAST && interpolationMode == 2))
                       deltaY = curY - yPrev;
                    else
                       deltaY = yPrev - curY;
                }
            }
            else
                if (curQuadrant == Quadrant.UPPER_RIGHT || curQuadrant == Quadrant.UPPER_LEFT || curQuadrant == Quadrant.NONE_VERTICAL_NORTH ||
                        (curQuadrant == Quadrant.NONE_HORIZONTAL_WEST && interpolationMode == 3) || (curQuadrant == Quadrant.NONE_HORIZONTAL_EAST && interpolationMode == 2))
                    deltaY = curY - yPrev;
                else
                    deltaY = yPrev - curY;

            yPrev = curY;
            return deltaY;
        }

        /*
       * author:        Florian Ennemoser
       * desc:          Calculates the start angle of vertical/horizontal axis to the start coordinates
       * parameters:    curQuadrant: The quadrant where the tool starts processing the arc
       * return:        The start angle between the axis and the start coordinates
       */
        private double CalcStartAngle(int interpolationMode, double r, Quadrant curQuadrant, double i)
        {
            if (curQuadrant == Quadrant.NONE_HORIZONTAL_EAST || curQuadrant == Quadrant.NONE_HORIZONTAL_WEST || curQuadrant == Quadrant.NONE_VERTICAL_NORTH || curQuadrant == Quadrant.NONE_VERTICAL_SOUTH)
            {
                return 0;
            }

            if ((curQuadrant == Quadrant.UPPER_LEFT && interpolationMode == 3) || (curQuadrant == Quadrant.UPPER_RIGHT && interpolationMode == 2) ||
              (curQuadrant == Quadrant.LOWER_LEFT && interpolationMode == 2) || (curQuadrant == Quadrant.LOWER_RIGHT && interpolationMode == 3))
            {
                return  Math.Acos(Math.Abs(i) / r);
            }

            return Math.Asin(Math.Abs(i) / r);
        }

        /*
      * author:        Florian Ennemoser
      * desc:          Calculates the quadrant depending on the distance of the tool to the horizontal and vertical axis
      * return:        The quadrant where the tool is situated
      */
        private Quadrant GetQuadrant(double distToHorAxis, double distToVertAxis)
        {
            if (EpsilonTests.IsNearlyZeroEpsHigh(distToHorAxis) && distToVertAxis < 0)
                return Quadrant.NONE_HORIZONTAL_EAST;
            else if (EpsilonTests.IsNearlyZeroEpsHigh(distToHorAxis) && distToVertAxis > 0)
                return Quadrant.NONE_HORIZONTAL_WEST;
            else if (distToHorAxis < 0 && EpsilonTests.IsNearlyZeroEpsHigh(distToVertAxis))
                return Quadrant.NONE_VERTICAL_NORTH;
            else if (distToHorAxis > 0 && EpsilonTests.IsNearlyZeroEpsHigh(distToVertAxis))
                return Quadrant.NONE_VERTICAL_SOUTH;
            else if (distToHorAxis < 0 && distToVertAxis > 0)
                return Quadrant.UPPER_LEFT;
            else if (distToHorAxis < 0 && distToVertAxis < 0)
                return Quadrant.UPPER_RIGHT;
            else if (distToHorAxis > 0 && distToVertAxis < 0)
                return Quadrant.LOWER_RIGHT;
            else
                return Quadrant.LOWER_LEFT;
        }

        /*
        * author:        Florian Ennemoser
        * desc:          Calculates the quadrant depending on the angle
        * return:        The quadrant where the tool is situated
        */
        private void GetQuadrant(double alpha, Quadrant startQuadrant, ref Quadrant curQuadrant, ref Quadrant prevQuadrant, int interpolationMode)
        {
            double segmentParts = alpha / (Math.PI /2);
            int nextSegments = 0;
            if (startQuadrant == Quadrant.NONE_HORIZONTAL_EAST || startQuadrant == Quadrant.NONE_HORIZONTAL_WEST || startQuadrant == Quadrant.NONE_VERTICAL_NORTH || startQuadrant == Quadrant.NONE_VERTICAL_SOUTH)
                nextSegments = (EpsilonTests.IsNearlyZeroEpsHigh(segmentParts % 1)) ? (int)segmentParts * 2 : (int)segmentParts * 2 + 1;
            else
            {
                if (segmentParts >= 1)
                {
                    nextSegments = (EpsilonTests.IsNearlyZeroEpsHigh(segmentParts % 1)) ? (int)segmentParts * 2 - 1 : (int)segmentParts * 2;
                }
            }

            if (nextSegments > 0)
            {
                Quadrant quadrant = (interpolationMode == 2) ? (Quadrant)(mod((int)startQuadrant + nextSegments, 8)) : (Quadrant)(mod((int)startQuadrant - nextSegments, 8));
                if (quadrant != curQuadrant)
                {
                    prevQuadrant = curQuadrant;
                    curQuadrant = quadrant;
                }
            }
        }

        /*
      * author:        Florian Ennemoser
      * desc:          Checks if the processing circle or circle segment is greater or less than 180°
      * return:        True if greater than 180°, false otherwise
      */
        private bool IsGreaterHalfCircle()
        {
            //TODO: Implement method
            return false;
        }

        /*
       * author:        Florian Ennemoser
       * desc:          Helper function that calculates the modulus as positive remainder
       * return:        The positive remainder
       */
        private int mod(int x, int m)
        {
            return (x % m + m) % m;
        }

        private enum Quadrant
        {
            UPPER_LEFT,
            NONE_VERTICAL_NORTH,
            UPPER_RIGHT,
            NONE_HORIZONTAL_EAST,
            LOWER_RIGHT,
            NONE_VERTICAL_SOUTH,
            LOWER_LEFT,
            NONE_HORIZONTAL_WEST
        }
    }
}
