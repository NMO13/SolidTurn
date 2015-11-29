using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using Simulation.GUI;
using Simulation.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mesh.third_party.Enterprise_Blocks
{
    static class LoggingSupport
    {
        internal static LoggingConfiguration BuildLoggingConfig(GlobalStateModel model)
        {
            // Formatters
            //TextFormatter formatter = new TextFormatter("Timestamp: {timestamp}{newline}Message: {message}{newline}Category: {category}{newline}Priority: {priority}{newline}EventId: {eventid}{newline}Severity: {severity}{newline}Title:{title}{newline}Machine: {localMachine}{newline}App Domain: {localAppDomain}{newline}ProcessId: {localProcessId}{newline}Process Name: {localProcessName}{newline}Thread Name: {threadName}{newline}Win32 ThreadId:{win32ThreadId}{newline}Extended Properties: {dictionary({key} - {value}{newline})}");
            TextFormatter errorFormatter = new TextFormatter("{timestamp} {message}");
            TextFormatter consoleFormatter = new TextFormatter("{timestamp} {severity} : {message}");
            // Listeners
            var errorFlatFileTraceListener = new FlatFileTraceListener(Directory.GetCurrentDirectory() + "\\" + "Error.log", "----------------------------------------", "----------------------------------------", errorFormatter);
            var outputFlatFileTraceListener = new FlatFileTraceListener(Directory.GetCurrentDirectory() + "\\" + "Output.log", "----------------------------------------", "----------------------------------------", errorFormatter);
            var outputViewTraceListener = new OutputViewTraceListener(model, consoleFormatter);
            //var consoleTraceListener = new CustomTraceListener();
            // Build Configuration
            var config = new LoggingConfiguration();
            config.AddLogSource("Error", SourceLevels.All, true).AddTraceListener(errorFlatFileTraceListener);
            config.AddLogSource("Output", SourceLevels.All, true).AddTraceListener(outputViewTraceListener);
            config.LogSources["Output"].AddTraceListener(outputFlatFileTraceListener);
            config.AddLogSource("Debug", SourceLevels.All, true).AddTraceListener(outputViewTraceListener);
            config.AddLogSource("OutputToFile", SourceLevels.All, true).AddTraceListener(outputFlatFileTraceListener);

            return config;
        }
    }
}
