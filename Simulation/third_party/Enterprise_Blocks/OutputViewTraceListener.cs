using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using Simulation.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mesh.third_party.Enterprise_Blocks
{
    class OutputViewTraceListener : CustomTraceListener
    {
        private GlobalStateModel m_Model;
        private ILogFormatter m_Formatter;
        internal OutputViewTraceListener(GlobalStateModel model, ILogFormatter formatter)
        {
            this.m_Model = model;
            this.m_Formatter = formatter;
        }
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            if (source == "Output")
                m_Model.NewOutputMessage = m_Formatter.Format(data as LogEntry);
            else if (source == "Debug")
                m_Model.NewDebugMessage = m_Formatter.Format(data as LogEntry);
        }

        public override void Write(string message)
        {
            Debug.Write(message);
        }

        public override void WriteLine(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
