using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFPS : MonoBehaviour
{
    public int targetFPS = 60;

    private void Start() {
        Application.targetFrameRate = targetFPS;
    }
}