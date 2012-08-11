using System;
using UnityEngine;

namespace ChimeraEntertainment.Unity3DJavascriptLogger
{
    internal class JavascriptDispatcher
    {
        private readonly Action<Action> m_dispatchAction;

        internal JavascriptDispatcher(Action<Action> dispatchAction)
        {
            m_dispatchAction = dispatchAction;
        }

        internal bool EvalJs(string javascript)
        {
            if (m_dispatchAction != null)
            {
                m_dispatchAction(() => Application.ExternalEval(javascript));
                return true;
            }
            return false;
        }
    }
}
