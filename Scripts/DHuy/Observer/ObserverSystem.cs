using System;
using System.Collections.Generic;
using UnityEngine;

public enum ObserverEvent
{
    SettingChange, StartGame, Win, Lose
}

public class ObserverSystem : MonoBehaviour
{
    public static ObserverSystem Instance;
    public Dictionary<ObserverEvent, List<ObserverListener>> dicListeners;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            dicListeners = new Dictionary<ObserverEvent, List<ObserverListener>>();
            var events = Enum.GetValues(typeof(ObserverEvent));
            foreach (ObserverEvent e in events)
            {
                dicListeners.Add(e, new List<ObserverListener>());
            }
        } else
        {
            Destroy(gameObject);
        }
    }
    public void Register(ObserverListener listener, ObserverEvent e)
    {
        var list = dicListeners[e];
        if (list.Contains(listener) == false) list.Add(listener);
    }
    public void Notify(ObserverEvent e)
    {
        var list = dicListeners[e];
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != null) list[i].NotifyEvent(e);
        }
    }
}