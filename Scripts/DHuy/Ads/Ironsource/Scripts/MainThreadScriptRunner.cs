using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarkcupGames
{
    public class MainThreadScriptRunner : MonoBehaviour
    {
        private List<Action> mainThreadEvents = new List<Action>();
        private void Update()
        {
            if (mainThreadEvents.Count > 0)
            {
                for (int i = 0; i < mainThreadEvents.Count; i++)
                {
                    mainThreadEvents[i]?.Invoke();
                }
                mainThreadEvents.Clear();
            }
        }
        public void Run(Action action)
        {
            mainThreadEvents.Add(action);
        }
    }
}