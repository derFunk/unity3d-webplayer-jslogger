using UnityEngine;
using ChimeraEntertainment.Unity3DJavascriptLogger;

public class JsLoggerCallback : MonoBehaviour
{
	#if UNITY_WEBPLAYER && !UNITY_EDITOR
	
	JavascriptLogger jsLogger;
		
	void Awake()
	{
        DontDestroyOnLoad(transform.gameObject);

		jsLogger = new JavascriptLogger();
		
		// Register the javascript logger for the standard unity log output.
	    Application.RegisterLogCallback(jsLogger.HandleUnityLog);
		
		Debug.Log("Log-Test: Javascript Logging Handler enabled!");
		Debug.LogWarning("LogWarning-Test: Javascript Logging Handler enabled!");
		Debug.LogError("LogError-Test: Javascript Logging Handler enabled!");
	}
	
	void Update()
	{
		jsLogger.Dispatcher.Dequeue();
	}
	
	void OnDisable()
	{
	    Application.RegisterLogCallback(null);
	}
	
	void OnDestroy()
	{
	    Application.RegisterLogCallback(null);
	}

#endif
}
