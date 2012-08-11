/***************************************************************************\
Project:      Javascript Logger for Unity3D Webplayer
Copyright (c) Andreas Katzig, Chimera Entertainment GmbH

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace ChimeraEntertainment.Unity3DJavascriptLogger
{
    public class JavascriptLoggerDispatcher
    {
        private readonly IJavascriptLoggerDispatchAction m_dispatchAction;

        internal JavascriptLoggerDispatcher(IJavascriptLoggerDispatchAction dispatchAction)
        {
            m_dispatchAction = dispatchAction;
        }

        internal bool EvalJs(string javascript)
        {
			if (m_dispatchAction != null)
			{
            	m_dispatchAction.Dispatch (() => Application.ExternalEval(javascript));
            	return true;
			}
			return false;
        }
		
		public void Dequeue()
		{
			if (m_dispatchAction != null)
				m_dispatchAction.DeQueue();
		}

    }
	
	interface IJavascriptLoggerDispatchAction
	{
		void Dispatch(Action a);
		void DeQueue();
	}
	
	internal class JavascriptLoggerDispatchAction : IJavascriptLoggerDispatchAction
	{
		#region DispatcherAction
		static Queue<Action> DispatcherQueue = new Queue<Action>();
	
	    public void Dispatch(Action a)
	    {
	        lock(((ICollection)DispatcherQueue).SyncRoot)
	        {
	            DispatcherQueue.Enqueue(a);
	        }        
	    }
		
		public void DeQueue()
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
		#endregion
	}
}
