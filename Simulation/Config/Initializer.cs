using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Geometry;
using System.Text.RegularExpressions;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Globalization;

namespace Mesh.Config
{
    class Initializer
    {
        enum Token { TOOL_ZERO, MACHINE_ZERO, PART_ZERO, ROUGH_PART_LENGTH, ROUGH_PART_RADIUS, ROUGH_PART_SLICES, JAW_CHUCK_OFFSET, START_ZOOM, MAX_ZOOM, MIN_ZOOM, GRANULARITY_ZOOM, TOOL}

        public static Vector3D MachineZero { get; private set; }
        public static Vector3D PartZero { get; private set; }
        public static Vector3D ToolZero { get; private set; }
        public static double RoughPartLength {get; private set;}
        public static double RoughPartRadius { get; private set; }
        public static int RoughPartSlice { get; private set; }
        public static double MaxZoom { get; private set; }
        public static double StartZoom { get; private set; }
        public static double GranularityZoom { get; private set; }
        public static double MinZoom { get; private set; }
        public List<short> ToolSlots = new List<short>();
        public List<string> ToolNames = new List<string>();
        public static int JawChuckOffset { get; private set; }

        internal Initializer()
        {
            MachineZero = Vector3D.Zero();
            PartZero = Vector3D.Zero();
            ToolZero = Vector3D.Zero();
        }

        private static String path = "../../Resources/initializer.conf";

        internal void Parse(LogWriter outputWriter)
        {
            int counter = 0;
            //read property file
            try
            {
                string[] lines = System.IO.File.ReadAllLines(path);
                foreach (String line in lines)
                {
                    counter++;
                    String[] tokens = line.Split('=');
                  try
                    {
                        if (line == "" || line[0] == '#') //Check if line starts with a comment symbol (#) or is empty
                            continue;

                        Token t = (Token)Enum.Parse(typeof(Token),tokens[0]);
                        switch (t)
                        {
                            case Token.MACHINE_ZERO: GetVector2Values(tokens[1], MachineZero, outputWriter); break;
                            case Token.PART_ZERO: GetVector2Values(tokens[1], PartZero, outputWriter); break;
                            case Token.TOOL_ZERO: GetVector2Values(tokens[1], ToolZero, outputWriter); break;
                            case Token.ROUGH_PART_RADIUS: RoughPartRadius = Convert.ToDouble(tokens[1], System.Globalization.CultureInfo.InvariantCulture); break;
                            case Token.ROUGH_PART_SLICES: RoughPartSlice = Convert.ToInt16(tokens[1], System.Globalization.CultureInfo.InvariantCulture); break;
                            case Token.ROUGH_PART_LENGTH: RoughPartLength = Convert.ToDouble(tokens[1], System.Globalization.CultureInfo.InvariantCulture); break;
                            case Token.JAW_CHUCK_OFFSET: JawChuckOffset = Convert.ToInt32(tokens[1], System.Globalization.CultureInfo.InvariantCulture); break;
                            case Token.START_ZOOM: StartZoom = Convert.ToDouble(tokens[1], System.Globalization.CultureInfo.InvariantCulture); break;
                            case Token.MAX_ZOOM: MaxZoom = Convert.ToDouble(tokens[1], System.Globalization.CultureInfo.InvariantCulture); break;
                            case Token.MIN_ZOOM: MinZoom = Convert.ToDouble(tokens[1], System.Globalization.CultureInfo.InvariantCulture); break;
                            case Token.GRANULARITY_ZOOM: GranularityZoom = Convert.ToDouble(tokens[1], System.Globalization.CultureInfo.InvariantCulture); break;
                            case Token.TOOL: GetToolValues(tokens[1], outputWriter); break;
                            default: outputWriter.Write("Error in initializer.conf" + Environment.NewLine, "Output", -1, -1, System.Diagnostics.TraceEventType.Error); break;
                        }
                    }
                    catch(ArgumentException)
                    {
                        outputWriter.Write("Invalid token. See line " + counter + " in initializer.conf." + Environment.NewLine, "Output", -1, -1, System.Diagnostics.TraceEventType.Error);
                    }
                }
            }
            catch (Exception e)
            {
                outputWriter.Write(e.Message + " See line " + counter + " in initializer.conf." + Environment.NewLine, "Output", -1, -1, System.Diagnostics.TraceEventType.Error);
            }
        }

        private void GetToolValues(string value, LogWriter outputWriter)
        {
            value = value.Trim();
            value = Regex.Replace(value, @"\s", "");
            if (value[0] != '(' || value[value.Length - 1] != ')')
            {
                outputWriter.Write("Tool not properly specified in initializer.conf" + Environment.NewLine, "Output", -1, -1, System.Diagnostics.TraceEventType.Error);
                return;
            }

            string[] values = Regex.Split(value, @"\,");
            values[0] = values[0].Substring(1);
            values[1] = values[1].Remove(values[1].Length - 1);
            short i;
            if (!Int16.TryParse(values[0], NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-US"), out i))
            {
                outputWriter.Write("Tool not properly specified in initializer.conf" + Environment.NewLine, "Output", -1, -1, System.Diagnostics.TraceEventType.Error);
                return;
            }
            ToolSlots.Add(i);
            ToolNames.Add(values[1]);
        }

        private static void GetVector2Values(string value, Vector3D vec, LogWriter outputWriter)
        {
            value = value.Trim();
            value = Regex.Replace(value, @"\s", "");
            if (value[0] != '(' || value[value.Length - 1] != ')')
            {
                outputWriter.Write("Origin not properly specified in initializer.conf" + Environment.NewLine, "Output", -1, -1, System.Diagnostics.TraceEventType.Error);
                return;
            }
            string[] numbers = Regex.Split(value, @"\D+");
            List<int> nums = new List<int>();
            foreach (string v in numbers)
            {
                if (!string.IsNullOrEmpty(v))
                {
                    int i = int.Parse(v);
                    nums.Add(i);
                }
            }
            if (nums.Count != 2)
                throw new ArgumentException();
            vec.X = nums[0]; vec.Y = nums[1];
        }
    }
}
