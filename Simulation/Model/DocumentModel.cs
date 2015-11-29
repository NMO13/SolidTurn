using Geometry;
using Mesh;
using Simulation.CNC_Turning;
using Simulation.CNC_Turning.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoObjectStuff;
using System.IO;
using Simulation.CNC_Turning.Interpretation;
using Builder;
using Simulation.Persistence;
using Mesh.Config;
using Simulation.CNC_Turning.Machine_Stuff;
using Mesh.Rendering;
using System.Threading;
using System.Diagnostics;
using Mesh.CNC_Turning.Machine_Stuff;

namespace Simulation.Model
{ 
    internal interface ISpindleStateObserver
    {
        void SpindleStart(object sender, object e);
        void SpindleStop(object sender, object e);
        void SetChuckPositions(object sender, object e);
    }

    internal interface IProgramObserver
    {
        void NewProgram(object sender, object e);
    }

    internal interface IToolObserver
    {
        void SlotChanged(object sender, object e);
    }

    internal interface ICoordinateSystemObserver
    {
        void CoordinateSystemChanged(object sender, object e);
    }

    internal interface INCCodeObserver
    {
        void NewNCCode(object sender, object e);
    }

    internal interface IRoughPartObserver
    {
        void NewRoughPart(object sender, object e);
        void RemoveRoughPart(object sender, object e);
    }

    internal interface IAnimationStateObserver
    {
        void AnimationStateChanged(object sender, AnimationState e);
    }

    internal interface IGlobalOffsetObserver
    {
        void GlobalOffsetChanged(object sender, object offset);
    }

    /**
     * This class is an interface between the user and the program. It informs the program 
     * when the user changed a state.
     **/
    class DocumentModel
    {
        internal bool CNCProgramRead = false;
        internal bool CNCProgramValid = false;
        internal GlobalStateModel GlobalModel { get; set; }
        private string m_CNCProgramFilePath = string.Empty;
        internal delegate void ModelHandler(object sender, object args);
        internal delegate void AnimationStateHandler(object sender, AnimationState args);

        private event ModelHandler m_NewProgramEvent;
        private event ModelHandler m_NCCodeEvent;
        private event ModelHandler m_SlotChangeEvent;
        private event ModelHandler m_NewRoughPartEvent;
        private event ModelHandler m_RemoveRoughPartEvent;
        internal Object Lock = new Object(); // locks animation and processing
        private Object Lock2 = new Object(); // locks gui and processing
        private event AnimationStateHandler m_AnimationStateEvent;

        private event ModelHandler m_SpindleStartEvent;
        private event ModelHandler m_SpindleStopEvent;
        private event ModelHandler m_ChuckChangedEvent;

        private event ModelHandler m_GlobalOffsetEvent; // translate the whole scene so that it fits better the window

        private event ModelHandler m_CoordinateSystemChangedEvent;

        private NCProgram m_Program;
        private string m_NCCode = string.Empty;

        private Processor m_Processor;
        private Animation m_Animator;

        internal volatile AnimationState AnimationState = AnimationState.STOPPED;
        private Thread m_AnimationThread;

        private Initializer m_Config = new Initializer();
        private Vector3D m_GlobalOffset = new Vector3D(-200, 0, 0);
        private bool m_RoughPartExists = false;

        private Origins m_Origins = new Origins();
        internal ToolSet ToolSet = new ToolSet();

        private short[] m_SlotNames;
        private string[] m_ToolNames;

        public string OGLVersion { get; set; }

        internal DocumentModel()
        {
            m_Processor = new Processor(this);
            m_Animator = new Animation(this, Lock);
        }

        internal void ReadCNCProgram(string path)
        {
            NCCode = ReadTextFile(path);
            CNCProgramRead = true;
            CNCProgramValid = false;
            m_CNCProgramFilePath = path;
        }

        private string ReadTextFile(string filepath)
        {
            String code = string.Empty;
            try
            {
                using (StreamReader sr = new StreamReader(filepath))
                {
                    String line = sr.ReadToEnd();
                    code += line;
                }
            }
            catch (Exception e)
            {
                GlobalModel.OutputWriter.Write("The file could not be read: " + e.Message + Environment.NewLine, "Output");
            }
            GlobalModel.OutputWriter.Write("Opened NC file: " + filepath + Environment.NewLine, "Output");
            return code;
        }

        internal void InterpretCNCProgram()
        {
            if (!CNCProgramRead)
            {
                GlobalModel.OutputWriter.Write("No CNC program specified" + Environment.NewLine, "Output");
                return;
            }
            Interpreter i = new Interpreter(m_CNCProgramFilePath, this);
            string[] errorString;
            NCProgram p = i.Interpret(out errorString);
            if (i.IsErrorFree)
            {
                CNCProgramValid = true;
                GlobalModel.OutputWriter.Write("Parsing program ... OK" + Environment.NewLine, "Output");
                NCProgram = p;
            }
            else
            {
                foreach (string line in errorString)
                {
                    GlobalModel.OutputWriter.Write(line + Environment.NewLine, "Output", -1, -1, System.Diagnostics.TraceEventType.Error);
                }
            }
        }

        internal bool CheckStartingConditions()
        {
            InterpretCNCProgram();
            if(!CNCProgramValid)
                return false;
            if (Parts.Count == 0)
            {
                GlobalModel.OutputWriter.Write("No rough part specified" + Environment.NewLine, "Output", -1, -1, System.Diagnostics.TraceEventType.Error);
                return false;
            }
            return true;
        }

        private void StartAnimation()
        {
            m_Processor.Reset();
            m_Processor.CalcTrajectory();
            GlobalModel.OutputWriter.Write("Trajectories calculated" + Environment.NewLine, "Output");
            AnimationState = AnimationState.RUNNING;

            m_AnimationThread = new Thread(m_Animator.Run);
            m_AnimationThread.Name = "AnimationThread";
            m_AnimationThread.IsBackground = true;
            m_AnimationThread.Start();
            Process.GetCurrentProcess().PriorityClass =
        ProcessPriorityClass.High;  	// Prevents "Normal" processes 
            // from interrupting Threads
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
            // Spin for a while waiting for the started thread to become
            // alive:
            while (!m_AnimationThread.IsAlive) ;
        }

        internal Tool CreateToolFromFile(string file)
        {
            ToolBuilder builder = new ToolBuilder(new XMLToolReader(file));
            Tool newTool = builder.BuildTool();
            AddCoordinateSystemChangedObserver(newTool);
            newTool.AnimationLock = Lock;
            return newTool;
        }

        internal void CreateRoughPart()
        {
            if (m_RoughPartExists)
                RemoveRoughPart();

            Del2D handler = TemplateMeshes.Quad;
            GeoObjectBuilder builder = new GeoObjectBuilder();
            object[] oparams = new object[4];
            oparams[0] = RoughPartSpec.Length; // length
            oparams[1] = Initializer.RoughPartSlice; // slice
            oparams[2] = RoughPartSpec.Radius; // radius
            oparams[3] = Initializer.JawChuckOffset;

            RoughPartGeoObject o = builder.BuildRoughPartGeoObject(handler, oparams);
            o.LogWriter = GlobalModel.m_OutputWriter;
            o.AnimationLock = Lock;
            o.Render2D = false;
            Parts.Add(o);
            NotifyNewRoughPart(o);
            NotifyChuckPositions((double)oparams[2]);
            GlobalModel.OutputWriter.Write("Rough part created" + Environment.NewLine, "Output");
            m_RoughPartExists = true;
        }

        private void NotifyNewRoughPart(RoughPartGeoObject o)
        {
            if(m_NewRoughPartEvent != null)
                m_NewRoughPartEvent.Invoke(this, o);
        }

        private void NotifyRemoveRoughPart(RoughPartGeoObject o)
        {
            if (m_RemoveRoughPartEvent != null)
                m_RemoveRoughPartEvent.Invoke(this, o);
        }

        private void NotifySlotChanged(Mesh.CNC_Turning.Machine_Stuff.ToolSet.Slot slot)
        {
            if (m_SlotChangeEvent != null)
                m_SlotChangeEvent.Invoke(this, slot);
        }

        internal void NotifyToolZeroChanged(Vector3D trans)
        {
            object[] arr = new object[2];
            arr[0] = trans;
            arr[1] = CoordinateSystemType.TOOL_ZERO;
            if (m_CoordinateSystemChangedEvent != null)
                m_CoordinateSystemChangedEvent.Invoke(this, arr);
        }

        internal void NotifyPartZeroChanged(Vector3D trans)
        {
            object[] arr = new object[2];
            m_Origins.partZero += trans;
            arr[0] = trans;
            arr[1] = CoordinateSystemType.PART_ZERO;
            if (m_CoordinateSystemChangedEvent != null)
                m_CoordinateSystemChangedEvent.Invoke(this, arr);
        }

        internal void NotifyGlobalOffsetChanged()
        {
            if (m_GlobalOffsetEvent != null)
                m_GlobalOffsetEvent.Invoke(this, m_GlobalOffset);
        }

        internal void AddNCProgramObserver(IProgramObserver obs)
        {
            m_NewProgramEvent += new ModelHandler(obs.NewProgram);
        }

        internal void AddNCTextObserver(INCCodeObserver obs)
        {
            m_NCCodeEvent += new ModelHandler(obs.NewNCCode);
        }

        internal void AddAnimationStateObserver(IAnimationStateObserver obs)
        {
            m_AnimationStateEvent += new AnimationStateHandler(obs.AnimationStateChanged);
        }

        internal void AddCoordinateSystemChangedObserver(ICoordinateSystemObserver obs)
        {
            m_CoordinateSystemChangedEvent += new ModelHandler(obs.CoordinateSystemChanged);
        }

        internal string NCCode
        {
            get
            {
                return m_NCCode;
            }
            set
            {
                m_NCCode = value;
                if (m_NCCodeEvent != null)
                    m_NCCodeEvent.Invoke(this, value);
            }
        }

        internal NCProgram NCProgram
        {
            get
            {
                return m_Program;
            }
            set 
            {
                m_Program = value;
                if(m_NewProgramEvent != null)
                    m_NewProgramEvent.Invoke(this, value); 
            }
        }

        internal void Quit()
        {
            m_Processor.Stop();
        }

        internal Processor Processor { get { return m_Processor; } }

        internal void NotifyAnimationStateChanged()
        {
            if(m_AnimationStateEvent != null)
                m_AnimationStateEvent.Invoke(this, AnimationState);
        }

        internal List<RoughPartGeoObject> Parts = new List<RoughPartGeoObject>();
        internal bool OGLContextCreated = false;

        internal Renderer Renderer
        {
            set
            {
                value.LogWriter = GlobalModel.m_OutputWriter;
                m_SlotChangeEvent += new ModelHandler(value.SlotChanged);
                m_NewRoughPartEvent += new ModelHandler(value.NewRoughPart);
                m_RemoveRoughPartEvent += new ModelHandler(value.RemoveRoughPart);
                m_SpindleStartEvent += new ModelHandler(value.SpindleStart);
                m_SpindleStopEvent += new ModelHandler(value.SpindleStop);
                m_ChuckChangedEvent += new ModelHandler(value.SetChuckPositions);
                m_GlobalOffsetEvent += new ModelHandler(value.GlobalOffsetChanged);
                m_CoordinateSystemChangedEvent += new ModelHandler(value.CoordinateSystemChanged);
                NotifyGlobalOffsetChanged();
            }

        }

        internal void ReadConfigFile()
        {
            m_Config.Parse(GlobalModel.m_OutputWriter);
            InitializeApplication();
        }

        private void InitializeApplication()
        {
            AddCoordinateSystemChangedObserver(ToolSet);
            NotifyToolZeroChanged(Initializer.ToolZero);
            LoadToolsFromFiles();
            if (m_Config.ToolSlots == null || m_Config.ToolNames == null)
            {
                GlobalModel.OutputWriter.Write("Tools not properly specified in initializer.conf" + Environment.NewLine, "Output", -1, -1, System.Diagnostics.TraceEventType.Error);
                return;
            }
            if (m_Config.ToolSlots.Count != m_Config.ToolNames.Count)
            {
                GlobalModel.OutputWriter.Write("Specified number of tool slots and toolnames do not match in initializer.conf" + Environment.NewLine, "Output", -1, -1, System.Diagnostics.TraceEventType.Error);
                return;
            }
            InitializeToolSet(m_Config.ToolSlots.ToArray(), m_Config.ToolNames.ToArray());
        }

        internal void InitializeToolSet(short[] slotName, string[] toolName)
        {
            m_SlotNames = slotName;
            m_ToolNames = toolName;
            ToolSet.Clear();
            
            Debug.Assert(slotName.Length == toolName.Length);

            for (int i = 0; i < slotName.Length; i++)
            {
                if (toolName[i] != null && toolName[i] != "")
                {
                    Tool t = ToolSet.GetToolByName(toolName[i]);
                    if(t == null)
                    {
                        throw new Exception("Tool not found in tool set");
                    }
                    ToolSet.AddToolSlot(new ToolSet.Slot(slotName[i], t));
                }
            }
        }

        private void LoadToolsFromFiles()
        {
           string[] toolFiles = Directory.GetFiles("../../Resources/Tools");
            if(toolFiles.Length == 0)
                GlobalModel.OutputWriter.Write("No toolfiles found" + Environment.NewLine, "Output", -1, -1, System.Diagnostics.TraceEventType.Error);
            foreach (string toolFile in toolFiles)
            {
                Tool t = CreateToolFromFile(toolFile);
                ToolSet.AddTool(t);
            }
        }

        internal void NotifySpindleStop()
        {
            if (m_SpindleStopEvent != null)
                m_SpindleStopEvent.Invoke(this, null);
        }

        internal void NotifySpindleStart()
        {
            if (m_SpindleStartEvent != null)
                m_SpindleStartEvent.Invoke(this, null);
        }

        private void NotifyChuckPositions(double distance)
        {
            if (m_ChuckChangedEvent != null)
                m_ChuckChangedEvent.Invoke(this, distance);
        }

        internal void SetActiveSlot(short activeSlot)
        {
            Mesh.CNC_Turning.Machine_Stuff.ToolSet.Slot s = ToolSet.SetActiveSlot(activeSlot);
            NotifySlotChanged(s);
        }

        internal void ChangeState(AnimationState state)
        {
            lock (Lock2) // GUI and processor (other thread) are accessing this
            {
                if (state == AnimationState.RUNNING)
                {
                    if (AnimationState == AnimationState.RUNNING)
                        return;
                    if (AnimationState == AnimationState.FINISHED)
                    {
                        GlobalModel.OutputWriter.Write("The program is still running. Stop it first." + Environment.NewLine, "Output");
                        return;
                    }
                    if (CheckStartingConditions())
                        PerformStart();
                    else
                        return;
                }
                else if (state == AnimationState.STOPPED)
                {
                    if (AnimationState == AnimationState.STOPPED)
                        return;
                    AnimationState = state;
                    PerformStop();
                }
                else if (state == AnimationState.FINISHED)
                {
                    PerformFinished();
                }
                else if (state == AnimationState.PAUSED)
                {
                    if (AnimationState == AnimationState.PAUSED)
                        return;
                    PerformPaused();
                }
                else if (state == AnimationState.CONTINIUE)
                {
                    GlobalModel.OutputWriter.Write("Resume execution" + Environment.NewLine, "Output");
                    state = AnimationState.RUNNING;
                }
                else
                    throw new ArgumentException("Illegal State");

                AnimationState = state;
                NotifyAnimationStateChanged();
            }
        }

        private void PerformStart()
        {
            GlobalModel.OutputWriter.Write("Execution started" + Environment.NewLine, "Output");
            StartAnimation();
        }

        private void PerformStop()
        {
            m_AnimationThread.Join();
            GlobalModel.OutputWriter.Write("Execution stopped" + Environment.NewLine, "Output");
            PerformReset();
        }

        private void PerformFinished()
        {
            Console.WriteLine("send finished msg");
            // Can only be outputted to file,because there is a high chance that the main thread holds the lock
           GlobalModel.OutputWriter.Write("Execution finished" + Environment.NewLine, "OutputToFile");
        }

        private void PerformPaused()
        {
            GlobalModel.OutputWriter.Write("Execution paused" + Environment.NewLine, "Output");
        }

        private void PerformReset()
        {
            // reset rough part(s)
            RemoveRoughPart();
            CreateRoughPart();
            // reset coordinate axes
            NotifyPartZeroChanged(m_Origins.partZero.Negated());
            // reset tool
            SetActiveSlot(-1);
            ToolSet.Tools.Clear();
            LoadToolsFromFiles();
            InitializeToolSet(m_SlotNames, m_ToolNames);
            // stop spindle
            NotifySpindleStop();
        }

        private void RemoveRoughPart()
        {
            m_RoughPartExists = false;
            foreach (RoughPartGeoObject p in Parts)
            {
                p.FreeVBOs();
                NotifyRemoveRoughPart(p);
            }
            Parts.Clear();
        }

        internal void SetRoughPartParams(double l, double r)
        {
            RoughPartSpec.Length = l;
            RoughPartSpec.Radius = r;
        }
    }

    class Origins
    {
        internal Vector3D partZero = new Vector3D(Vector3D.Zero());
        internal Vector3D toolZero = new Vector3D(Vector3D.Zero());
        internal Vector3D machineZero = new Vector3D(Vector3D.Zero());
    }

    internal static class RoughPartSpec
    {
        internal static double Length = 0;
        internal static double Radius = 0;
    }
}
