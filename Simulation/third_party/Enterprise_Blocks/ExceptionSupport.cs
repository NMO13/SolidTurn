using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using OpenTK.Graphics.OpenGL;
using Simulation.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesh.third_party.Enterprise_Blocks
{
    static class ExceptionSupport
    {
        internal static string ErrorMsg = @"We're sorry, somethong went wrong." + Environment.NewLine + "An error occured which causes a shutdown. An error report was generated in '" + Directory.GetCurrentDirectory() + "\\" + "Error.log'. Please send this file to the developers.";
        internal static ExceptionManager BuildExceptionManagerConfig(LogWriter logWriter, DocumentModel doc)
        {
            var loggingAndReplacing = new List<ExceptionPolicyEntry>
            {
                new ExceptionPolicyEntry(typeof (Exception),
                    PostHandlingAction.ThrowNewException,
                    new IExceptionHandler[]
                     {
                       new MyLoggingExceptionHandler("Error", 9000, TraceEventType.Error,
                         "Simulation Exception", 5, typeof(TextExceptionFormatter), logWriter, doc),
                       new ReplaceHandler(ErrorMsg,
                         typeof(Exception))
                     })
            };

            var policies = new List<ExceptionPolicyDefinition>();
            policies.Add(new ExceptionPolicyDefinition("LoggingAndReplacingException", loggingAndReplacing));
            ExceptionManager exManager = new ExceptionManager(policies);
            return exManager;
        }
    }

    class MyLoggingExceptionHandler : LoggingExceptionHandler
    {
        private readonly LogWriter m_Writer;
        private DocumentModel doc;
        internal MyLoggingExceptionHandler(string logCategory, int eventId, TraceEventType severity, string title, int priority, 
            Type formatterType, LogWriter writer, DocumentModel doc) : base(logCategory, eventId, severity, title, priority, formatterType, writer)
        {
            this.m_Writer = writer;
            this.doc = doc;
        }

        protected override void WriteToLog(string logMessage, IDictionary exceptionData)
        {
            base.WriteToLog(logMessage, exceptionData);
            foreach (DictionaryEntry dataEntry in exceptionData)
            {
                if (dataEntry.Key is string)
                {
                    m_Writer.Write(dataEntry.Value);
                }
            }
            if (doc.OGLContextCreated)
                m_Writer.Write("OpenGL version: " + doc.OGLVersion);
            else
                m_Writer.Write("OpenGL version is unknown");
            m_Writer.Write("NC-Program: " + Environment.NewLine + doc.NCCode);
            string slots = string.Empty;
            doc.ToolSet.Slots.ForEach(s => slots += s.Name + " -- " + s.Tool.Name + Environment.NewLine);
            m_Writer.Write("Toolset: " + Environment.NewLine + slots);
            m_Writer.Write("Active Tool: " + doc.ToolSet.ActiveSlot.Name + " -- " + doc.ToolSet.ActiveSlot.Tool.Name);
            m_Writer.Write("RoughPart: \n Length: " + RoughPartSpec.Length + " Radius: " + RoughPartSpec.Radius);
        }
    }
}
