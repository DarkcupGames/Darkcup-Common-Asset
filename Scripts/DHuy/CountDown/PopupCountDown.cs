using System.Collections;
using UnityEngine;
using TMPro;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class PopupCountDown : MonoBehaviour
{
    public const float FADE_SPEED = 0.5f;

    [SerializeField] private TextMeshProUGUI txtCountDown;
    private Action onCountDownComplete;
    private CanvasGroup canvasGroup;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }
    public void CountDown(int time, Action onComplete)
    {
        this.onCountDownComplete = onComplete;
        StartCoroutine(IECountDown(time));
    }
    IEnumerator IECountDown(int time)
    {
        canvasGroup.alpha = 0f;
        LeanTween.value(0f, 1f, FADE_SPEED).setOnUpdate((float f) =>
        {
            if (canvasGroup) canvasGroup.alpha = f;
        });
        for (int i = time; i >= 1; i--)
        {
            txtCountDown.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        LeanTween.value(canvasGroup.alpha, 0f, FADE_SPEED).setOnUpdate((float f) =>
        {
            if (canvasGroup) canvasGroup.alpha = f;
        }).setOnComplete(() =>
        {
            if (gameObject != null)
            {
                onCountDownComplete?.Invoke();
                gameObject.SetActive(false);
            }
        });
    }
}