using System.Collections.Generic;
using UnityEngine;

public class NormalProperty : ObjectProperties
{
    public static NormalProperty Instance { get; private set; }
    [SerializeField] private GameObject[] properties;

    protected override void Awake ()
    {
        base.Awake ();
        Instance = this;
    }

    public override void Init ()
    {
        for (int i = 0; i < properties.Length; i++)
        {
            if (collection.ContainsKey (i))
            {
                Debug.LogError ($"Duplicate key of object name {properties[i].name}");
                return;
            }
            collection.Add (i, properties[i]);
        }
    }
}