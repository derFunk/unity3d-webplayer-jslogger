using UnityEngine;
using System;
using ChimeraEntertainment.Unity3DJavascriptLogger;

public class JsLoggerCallback : MonoBehaviour
{
	void Awake()
    {
#if UNITY_WEBPLAYER
		JavascriptLogger jsLogger = new JavascriptLogger(JavascriptLoggerDispatcher.Dispatch, "Javascript Logger");
			
		// Register the javascript logger for the standard unity log output.
        Application.RegisterLogCallback(jsLogger.HandleUnityLog);
		
		Debug.Log("Log-Test: Javascript Logging Handler enabled!");
		Debug.LogWarning("LogWarning-Test: Javascript Logging Handler enabled!");
		Debug.LogError("LogError-Test: Javascript Logging Handler enabled!");
#endif
    }
	
	void Update()
	{
#if UNITY_WEBPLAYER
		JavascriptLoggerDispatcher.DeQueue();
#endif
	}
    
	void OnDisable()
    {
#if UNITY_WEBPLAYER
        Application.RegisterLogCallback(null);
#endif
    }

    void OnDestroy()
    {
#if UNITY_WEBPLAYER
        Application.RegisterLogCallback(null);
#endif
    }

}
