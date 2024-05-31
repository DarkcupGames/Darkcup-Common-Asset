using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class MainThreadManager : MonoBehaviour
{
    public static MainThreadManager Instance;

    List<Action> actions;

    private void Awake()
    {
        Instance = this;
        actions = new List<Action>();
    }

    private void Start()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    public void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        for (int i = 0; i < actions.Count; i++)
        {
            actions[i] = null;
        }
        actions.Clear();
    }

    private void Update()
    {
        if (actions.Count > 0)
        {
            actions[0]?.Invoke();
            actions[0] = null;
            actions.RemoveAt(0);
        }
    }

    public void ExecuteInUpdate(Action action)
    {
        actions.Add(action);
    }
}
