using Mesh.third_party.Enterprise_Blocks;
using Simulation.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simulation
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        static MainForm form;
        [STAThread]
        static void Main()
        {
            //static MainForm form = null;
            try
            {
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);
                Application.ThreadException += new ThreadExceptionEventHandler(ThreadExceptionHandler);
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                form = new MainForm();
                Application.Run(form);
            }
            catch
            {
                try
                {
                    ShowExceptionDetails(ExceptionSupport.ErrorMsg);
                }
                finally
                {
                    if(form != null)
                       form.m_Document.Quit();
                }
            }
        }

        internal static void ThreadExceptionHandler(object sender, ThreadExceptionEventArgs args)
        {
            try
            {
                form.m_Document.GlobalModel.m_OutputWriter.Write("UI thread exception thrown");
                ShowExceptionDetails(ExceptionSupport.ErrorMsg);
                form.m_Document.Quit();
            }
            finally
            {
                Application.Exit();
            }
        }

        internal static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            try
            {
                form.m_Document.GlobalModel.m_OutputWriter.Write("Unhandled exception thrown");
                //ShowExceptionDetails(ExceptionSupport.ErrorMsg);
                form.m_Document.Quit();

            }
            finally
            {
                Application.Exit();
            }

        }

        internal static void ShowExceptionDetails(string msg)
        {
            MessageBox.Show(msg);
        }
    }
}
