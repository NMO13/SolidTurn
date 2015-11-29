using Mesh.third_party.Enterprise_Blocks;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Model
{
    internal delegate void ModelHandler<GlobalStateModel>(object sender, object args);

    internal interface IOutputObserver
    {
        void NewOutputMessage(object sender, object e);
    }

    internal interface IDebugMessageObserver
    {
        void NewDebugMessage(object sender, object e);
    }

    internal class GlobalStateModel
    {
        private ExceptionManager m_ExManager;
        internal LogWriter m_OutputWriter;
        private event ModelHandler<GlobalStateModel> m_OutputMessage;
        private event ModelHandler<GlobalStateModel> m_DebugMessage;
        internal bool RestrictFrameRate { get; set; }
        internal bool OutputVBOConsumption { get; set; }

        internal LogWriter OutputWriter { get { return m_OutputWriter; } }
        internal ExceptionManager ExManager { get { return m_ExManager; } }
        internal void InitEnterpriseBlocks(DocumentModel doc)
        {
            LoggingConfiguration loggingConfiguration = LoggingSupport.BuildLoggingConfig(this);
            m_OutputWriter = new LogWriter(loggingConfiguration);
            m_ExManager = ExceptionSupport.BuildExceptionManagerConfig(m_OutputWriter, doc);
        }

        internal string NewOutputMessage
        {
            set 
            { 
                if(m_OutputMessage != null)
                    m_OutputMessage.Invoke(this, value); 
            }
        }

        internal string NewDebugMessage
        {
            set
            {
                if (m_DebugMessage != null)
                    m_DebugMessage.Invoke(this, value);
            }
        }

        internal void AddOutputObserver(IOutputObserver obs)
        {
            m_OutputMessage += new ModelHandler<GlobalStateModel>(obs.NewOutputMessage);
        }

        internal void AddDebugMessageObserver(IDebugMessageObserver obs)
        {
            m_DebugMessage += new ModelHandler<GlobalStateModel>(obs.NewDebugMessage);
        }
    }
}
