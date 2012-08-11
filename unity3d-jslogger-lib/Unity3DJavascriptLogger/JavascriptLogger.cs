using System;
using UnityEngine;
using System.IO;
using System.Reflection;

namespace ChimeraEntertainment.Unity3DJavascriptLogger
{
	public class JavascriptLogger
	{
		protected string m_loggerName;
	    private bool m_isInitialized;
	    private JavascriptDispatcher m_dispatcher;
		
		public JavascriptLogger(Action<Action> dispatchAction, string name)
			: this(dispatchAction)
		{
			m_loggerName = name;
		}

		public JavascriptLogger(Action<Action> dispatchAction, Type type)
			: this(dispatchAction)
		{
			m_loggerName = type.Name;
		}

		public JavascriptLogger(Action<Action> dispatchAction)
		{
            if (m_loggerName == null)
                m_loggerName = "DEFAULT";

            m_dispatcher = new JavascriptDispatcher(dispatchAction);
            m_isInitialized = false;
			EnsureJavascriptEnvironment();
		}

        public void HandleUnityLog(string logString, string stackTrace, LogType type)
        {
            if (stackTrace == null)
                stackTrace = "(null)";

            if (logString == null)
                logString = "(null)";

                switch(type)
                {
                    case LogType.Log:
                        _log(logString, "Unity3D.Debug.Log", "DEBUG");
                        break;
                    case LogType.Warning:
                        _log(logString, "Unity3D.Debug.LogWarning", "WARN");
                        break;
                    case LogType.Error:
                        _log(logString, "Unity3D.Debug.LogError", "ERROR");
                        break;
                    case LogType.Exception:
                        
                        _log(logString + ": " + stackTrace, "Unity3D.Debug.LogException", "FATAL");
                        break;
                    case LogType.Assert:
                        _log(logString, "Unity3D.Debug.LogAssert", "INFO");
                        break;
                    default:
                        _log(logString, "Unity3D.Debug.Log", "INFO");
                        break;
                }
        }

		/// <summary>
		/// Need a dispatcher to run in gui thread
		/// </summary>
		/// <param name="javascript">Javascript code you want to have evaluated on the website!</param>
		private void DispatchEvalJs(string javascript)
		{
            if (!m_dispatcher.EvalJs(javascript))
                throw new Exception("Dispatcher for Javascript Logger not set!");
		}

        /// <summary>
        /// Makes sure that the javascript logger environment is available on the website.
        /// Javascript for initialization should be also safe to be executed multiple times.
        /// </summary>
		private void EnsureJavascriptEnvironment()
		{
            if (m_isInitialized)
                return;
            
            m_isInitialized = true;

            // execute / inject the js code into the website
            DispatchEvalJs(JavascriptLoggerHelper.GetDefaultLoggerEnvironmentFromResource(GetType().Namespace));
		}

        private static string cleanLogMessage(string msg)
        {
            msg = msg.Trim();
            return msg.Replace("'", "\"").Replace("\n","\\n\\t\\t").Replace("\r","");
        }

        private void _log(object message, string logname, string logtype)
        {
            if (!m_isInitialized)
                return;

            DispatchEvalJs("if (typeof unityLog == 'function') unityLog('" + logname + "', '" + logtype + "', '" + cleanLogMessage(message.ToString()) + "');");
        }

		public void LogDebug(object message)
		{
		    _log(message, m_loggerName, "DEBUG");
		}

		public void LogInfo(object message)
		{
            _log(message, m_loggerName, "INFO");
		}

		public void LogWarn(object message)
		{
            _log(message, m_loggerName, "WARN");
		}

		public void LogError(object message)
		{
            _log(message, m_loggerName, "ERROR");
		}

		public void LogFatal(object message)
		{
            _log(message, m_loggerName, "FATAL");
		}

	    public void LogTrace(object message)
	    {
            _log(message, m_loggerName, "TRACE");
	    }
	}

    static class JavascriptLoggerHelper
    {
        internal static string GetDefaultLoggerEnvironmentFromResource(string resNamespace)
        {
            // reads the script from a file. Having the javscript in a dedicated file makes it easier to edit
			string resId = "JavascriptLogger.js";
	
#if !MONO
			resId =  resNamespace + "." + resId;
#endif
            
			Stream jsLoggerStream =
                Assembly.GetExecutingAssembly().GetManifestResourceStream(resId);
		
            // read the js file into a string
            if (jsLoggerStream != null)
            {
                string jsLoggerScript = "";
                using (var sr = new StreamReader(jsLoggerStream))
                {
                    jsLoggerScript = sr.ReadToEnd();
                }
                return @jsLoggerScript;
            }
            
			throw new Exception("js file not found!");
        }

    }
}
