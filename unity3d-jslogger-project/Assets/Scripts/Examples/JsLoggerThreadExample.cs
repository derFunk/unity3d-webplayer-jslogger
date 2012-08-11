using UnityEngine;
using System.Collections;
using System.ComponentModel;
using ChimeraEntertainment.Unity3DJavascriptLogger;
using System;
using System.Threading;

public class JsLoggerThreadExample : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// init a background worker thread
		BackgroundWorker worker = new BackgroundWorker();
		worker.DoWork += HandleWorkerDoWork;
		worker.RunWorkerAsync();
	}
	
	/// <summary>
	/// This runs in a different thread and outputs to the javascript log.
	/// </summary>
	/// <param name='sender'>
	/// Sender.
	/// </param>
	/// <param name='e'>
	/// E.
	/// </param>
	void HandleWorkerDoWork (object sender, DoWorkEventArgs e)
	{
		JavascriptLogger jsLogger = new JavascriptLogger("Threaded Logger");
		
		while(true)
		{
			jsLogger.LogInfo("Ping " + DateTime.Now);
			Thread.Sleep(3000);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
