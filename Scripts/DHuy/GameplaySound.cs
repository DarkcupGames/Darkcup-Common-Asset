using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplaySound : MonoBehaviour
{
    public static GameplaySound Instance;

    public AudioClip resetSound;
    public AudioClip winSound;

    private void Awake()
    {
        Instance = this;
    }
}