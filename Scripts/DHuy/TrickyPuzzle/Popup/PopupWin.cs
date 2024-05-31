using DarkcupGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupWin : MonoBehaviour
{
    public GameObject ruybang;
    public EffectAppearOneByOne effectStar;
    public EffectAppearOneByOne effectVictoryText;
    public EffectAppearOneByOne logo;
    public GameObject nextButton;
    public List<ParticleSystem> fireworks;

    private void OnEnable()
    {
        StartCoroutine(IEDoEffect());
        FireworkManager.Instance.DoEffect();
    }

    IEnumerator IEDoEffect()
    {
        ruybang.gameObject.SetActive(false);
        effectStar.gameObject.SetActive(false);
        effectVictoryText.gameObject.SetActive(false);
        logo.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);

        EasyEffect.Appear(ruybang.gameObject, 0f, 1f, speed: 0.2f);
        yield return new WaitForSeconds(0.2f);

        effectStar.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);

        effectVictoryText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);

        EasyEffect.Appear(logo.gameObject, 0f, 1f, speed: 0.2f);
        yield return new WaitForSeconds(2f);

        EasyEffect.Appear(nextButton.gameObject, 0f, 1f, speed: 0.2f);
    }
}