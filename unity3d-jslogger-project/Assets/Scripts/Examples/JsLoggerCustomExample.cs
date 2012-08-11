using UnityEngine;
using System;
using ChimeraEntertainment.Unity3DJavascriptLogger;

public class JsLoggerCustomExample : MonoBehaviour {
	
	JavascriptLogger m_jsLogger1;
	JavascriptLogger m_jsLogger2;
	
	// Use this for initialization
	void Start () {
		// You can use as much javascript loggers as you wich, giving each its own custom name!
		m_jsLogger1 = new JavascriptLogger("Core Logger Example");
		m_jsLogger2 = new JavascriptLogger("Trace Logger Example");
	}
	
	int frames = 0;
	
	// Update is called once per frame
	void Update () {
				
		if (frames % 100 == 0) // test output
			m_jsLogger2.LogTrace("Custom JS Logger trace output");
		
		if (frames % 150 == 0) // test output
			m_jsLogger1.LogInfo("Custom JS Logger info output");
		
		if (frames % 175 == 0) // test output
			m_jsLogger1.LogWarn("Custom JS Logger warn output");
		
		frames++;
	}
}
