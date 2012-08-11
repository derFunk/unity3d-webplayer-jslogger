using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using ChimeraEntertainment.Unity3DJavascriptLogger;

public class JsLogConsole : MonoBehaviour
{
	static Queue<Action> DispatcherQueue = new Queue<Action>();

    public static void Dispatch(Action a)
    {
        lock(((ICollection)DispatcherQueue).SyncRoot)
        {
            DispatcherQueue.Enqueue(a);
        }        
    }

	void Awake()
    {
#if UNITY_WEBPLAYER
		JavascriptLogger jsLogger = new JavascriptLogger(Dispatch);
			
        Application.RegisterLogCallback(jsLogger.HandleUnityLog);
		
		Debug.Log("Log-Test: Javascript Logging Handler enabled!");
		Debug.LogWarning("LogWarning-Test: Javascript Logging Handler enabled!");
		Debug.LogError("LogError-Test: Javascript Logging Handler enabled!");
#endif
    }
	
	void Update()
	{
        lock(((ICollection)DispatcherQueue).SyncRoot)
        {
            while(DispatcherQueue.Count > 0)
            {
                Action a = DispatcherQueue.Dequeue();
                a();
            }
        }
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
