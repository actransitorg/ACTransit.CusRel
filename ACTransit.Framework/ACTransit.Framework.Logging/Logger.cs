using System;
using System.Reflection;
using log4net;

namespace ACTransit.Framework.Logging
{
    /// <summary>
    /// A wrapper for a Log4Net Logger.  This requires Log4Net to be configured in the consuming applications XML app/web config.
    /// </summary>
    public class Logger
    {
        private readonly ILog _log;

        public Logger()
        {
            log4net.Config.XmlConfigurator.Configure();

            _log = LogManager.GetLogger( MethodBase.GetCurrentMethod().DeclaringType );                        
        }

        public Logger(Type type)
        {
            log4net.Config.XmlConfigurator.Configure();

            _log = LogManager.GetLogger(type);
        }

        public Logger(string name)
        {
            log4net.Config.XmlConfigurator.Configure();

            _log = LogManager.GetLogger(name);            
        }


        /// <summary>
        /// Write debug.
        /// </summary>
        /// <param name="logText"></param>
        public void WriteDebug( string logText )
        {
            _log.Debug( logText );
        }

        /// <summary>
        /// Write info.
        /// </summary>
        /// <param name="logText"></param>
        public void Write(string logText)
        {
            _log.Info(logText);
        }

        /// <summary>
        /// Write error.
        /// </summary>
        /// <param name="logText"></param>
        public void WriteError( string logText )
        {
            _log.Error( logText );
        }

        /// <summary>
        /// Write error.
        /// </summary>
        /// <param name="logText"></param>
        /// <param name="ex"></param>
        public void WriteError( string logText, Exception ex )
        {
            _log.Error( logText, ex );
        }

        /// <summary>
        /// Write fatal.
        /// </summary>
        /// <param name="logText"></param>
        public void WriteFatal(string logText)
        {
            _log.Fatal(logText);
        }

        /// <summary>
        /// Write fatal.
        /// </summary>
        /// <param name="logText"></param>
        /// <param name="ex"></param>
        public void WriteFatal(string logText, Exception ex)
        {
            _log.Fatal(logText,ex);
        }

    }
}