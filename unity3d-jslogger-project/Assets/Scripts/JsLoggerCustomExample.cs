using UnityEngine;
using System;
using ChimeraEntertainment.Unity3DJavascriptLogger;

public class JsLoggerCustom : MonoBehaviour {
	
	JavascriptLogger m_jsLogger;
	
	// Use this for initialization
	void Start () {
		m_jsLogger = new JavascriptLogger(JavascriptLoggerDispatcher.Dispatch, "CustomJSLogger");
	}
	
	int frames = 0;
	
	// Update is called once per frame
	void Update () {
				
		if (frames % 100 == 0) // test output
			m_jsLogger.LogTrace("Custom JS Logger trace output");
		
		if (frames % 150 == 0) // test output
			m_jsLogger.LogInfo("Custom JS Logger info output");
		
		if (frames % 175 == 0) // test output
			m_jsLogger.LogWarn("Custom JS Logger warn output");
		
		frames++;
	}
}
