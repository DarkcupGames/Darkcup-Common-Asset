using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DarkcupGames;
using DG.Tweening;

public class FadeInOut : MonoBehaviour
{
    public const float FADE_SPEED = 0f;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    private void Start()
    {
        FadeIn();
    }
    public void FadeIn()
    {
        if (FADE_SPEED == 0)
        {
            gameObject.SetActive(false);
            return;
        }
        canvasGroup.alpha = 1f;
        canvasGroup.DOFade(0f, FADE_SPEED).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
    public void FadeOut(System.Action onDone)
    {
        if (FADE_SPEED == 0)
        {
            onDone?.Invoke();
            return;
        }
        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, FADE_SPEED).OnComplete(() =>
        {
            onDone?.Invoke();
        });
    }
}