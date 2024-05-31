using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkcupGames;

public class EffectAppearOneByOne : MonoBehaviour
{
    public const float EFFECT_TIME = 0.25f;

    private void OnEnable()
    {
        DoEffect();
    }

    public void DoEffect()
    {
        StartCoroutine(IEEffect());
    }

    IEnumerator IEEffect()
    {
        WaitForSeconds wait = new WaitForSeconds(EFFECT_TIME / 2);

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            EasyEffect.Appear(transform.GetChild(i).gameObject, 0f,1f, speed: EFFECT_TIME);
            yield return wait;
        }
    }
}