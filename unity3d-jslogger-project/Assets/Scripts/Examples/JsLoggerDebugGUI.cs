using UnityEngine;
using System.Collections;
using System;

public class JsLoggerDebugGUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log("Debug GUI Start()");
	}
	
	int frames = 0;
	
	// Update is called once per frame
	void Update () {
		if (++frames % 150 == 0) // test output
			Debug.Log("Update every 150 frames...");
	}

	void OnGUI()
	{
	
		if (GUI.Button(new Rect(Screen.width/2-100, 20, 200, 30), "Output a Debug.Log"))
		{
			Debug.Log("New Debug.Log. The time is: " + DateTime.Now);
		}
		
		if (GUI.Button(new Rect(Screen.width/2-100, 60, 200, 30), "Output a Debug.LogWarning"))
		{
			Debug.LogWarning("New Debug.LogWarning. The time is: " + DateTime.Now);
		}
		
		if (GUI.Button(new Rect(Screen.width/2-100, 100, 200, 30), "Output a Debug.LogError"))
		{
			Debug.LogError("New Debug Log. The time is: " + DateTime.Now);
		}
		
		if (GUI.Button(new Rect(Screen.width/2-100, 140, 200, 30), "Throw Exception"))
		{
			throw new Exception("Exception thrown on purpose.");
		}
	}
}
