using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

public abstract class ObjectProperties : MonoBehaviour
{
    public Dictionary<int, GameObject> collection = new Dictionary<int, GameObject> ();
    public UnityEvent onLoadComplete;
    public bool ready { get; protected set; }

    protected virtual void Awake ()
    {
        Init ();
    }

    public abstract void Init ();
}