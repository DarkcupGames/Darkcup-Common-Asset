using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectScaleTime : MonoBehaviour
{
    float nextScale = 1f;

    private void Update()
    {
        if (Time.time > nextScale)
        {
            nextScale = Time.time + Random.Range(13f, 15f);
            DarkcupGames.EasyEffect.Appear(gameObject, 1f, 1f, speed: 0.2f);
        }
    }
}
