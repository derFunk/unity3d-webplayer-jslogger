/***************************************************************************\
Project:      Javascript Logger for Unity3D Webplayer
Copyright (c) Andreas Katzig, Chimera Entertainment GmbH

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using System;
using System.Text;
using UnityEngine;
using System.IO;
using System.Reflection;

namespace ChimeraEntertainment.Unity3DJavascriptLogger
{
	public class JavascriptLogger
	{
		protected string m_loggerName;
	    private bool m_isInitialized;
	    public readonly JavascriptLoggerDispatcher Dispatcher;
		
		public JavascriptLogger(string name)
			: this(new JavascriptLoggerDispatchAction())
		{
			m_loggerName = name;
		}

		private JavascriptLogger(IJavascriptLoggerDispatchAction dispatchAction)
		{
            if (m_loggerName == null)
                m_loggerName = "DEFAULT";

            Dispatcher = new JavascriptLoggerDispatcher(dispatchAction);
            m_isInitialized = false;
			EnsureJavascriptEnvironment();
		}
		
		public JavascriptLogger()
			: this(new JavascriptLoggerDispatchAction())
		{
		}

	    public void EnableConsoleDebugLog(bool enable)
	    {
	        Dispatcher.EvalJs(enable ? "enableConsoleLog(true);" : "enableConsoleLog(false);");
	    }

	    public void RemoveLog()
        {
            Dispatcher.EvalJs("removeUnityLog();");
        }

        public void StartLog()
        {
            Dispatcher.EvalJs("ensureLog();");
        }

		public void SetMaxLogEntries(int max)
		{
			Dispatcher.EvalJs("unity_log_max_entries = " + max + ";");
		}
		
		public void SetConsoleDebugOutput(bool enabled)
		{
			Dispatcher.EvalJs("consoleLogEnabled = " + ((enabled) ? "true;" : "false;") );
		}
		
		/// <summary>
		/// Handles the standard unity log output.
		/// </summary>
		/// <param name='logString'>
		/// Log string.
		/// </param>
		/// <param name='stackTrace'>
		/// Stack trace.
		/// </param>
		/// <param name='type'>
		/// Type.
		/// </param>
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
            if (!Dispatcher.EvalJs(javascript))
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
            DispatchEvalJs(JavascriptLoggerHelper.GetContentsFromResource(injectedScriptFilename: "unity3d-webplayer-jslogger.js", resNamespace: GetType().Namespace, resIds: new [] { "FileSaver.min.js", "JavascriptLogger.js"}));
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
        /// <summary>
        /// Reads the scripts from embedded resource files.
        /// Having the javscript in a dedicated file makes it easier to edit.
        /// Concatenates all files to be certain that the sourceUrl js comment works.
        /// @see http://www.html5rocks.com/en/tutorials/developertools/sourcemaps/#toc-sourceurl
        /// @see http://updates.html5rocks.com/2013/06/sourceMappingURL-and-sourceURL-syntax-changed
        /// </summary>
        /// <param name="injectedScriptFilename"></param>
        /// <param name="resNamespace"></param>
        /// <param name="resIds"></param>
        /// <returns></returns>
        internal static string GetContentsFromResource(string injectedScriptFilename, string resNamespace, params string[] resIds)
        {
            var sb = new StringBuilder();
            sb.Append("//# sourceURL=").Append(injectedScriptFilename); // //@ sourceURL=jslogger.js
            
            sb.AppendLine();

            foreach (string resId in resIds)
            {
#if !MONO
		    	resId =  resNamespace + "." + resId;
#endif
                var resStream =
                    Assembly.GetExecutingAssembly().GetManifestResourceStream(resId);

                // read the js file into a string
                if (resStream != null)
                {
                    string resourceText = "";
                    using (var sr = new StreamReader(resStream))
                    {
                        resourceText = sr.ReadToEnd();
                    }
                    sb.Append(resourceText);
                }
            }

            return sb.ToString();
        }

    }
}
