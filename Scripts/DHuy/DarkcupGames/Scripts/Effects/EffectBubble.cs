using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBubble : MonoBehaviour
{
    [HideInInspector]
    public float scale;
    public float minScale = 0.95f;
    public float maxScale = 1f;
    public float speed = 0.05f;

    bool isBigger;

    private void OnEnable() {
        scale = transform.localScale.x;
    }

    private void Update() {
        scale += isBigger ? Time.deltaTime * speed : -Time.deltaTime * speed;

        if (isBigger && scale > maxScale) isBigger = false;
        if (!isBigger && scale < minScale) isBigger = true;
        gameObject.transform.localScale = new Vector3(scale, scale);
    }
}
