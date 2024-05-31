using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PopupLoading : MonoBehaviour
{
    public Slider slider;
    public Image imgFill;

    CanvasGroup canvasGroup;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        slider.value = 0;
        imgFill.fillAmount = 0;
    }
    public void ShowLoading(float time, System.Action onLoadingDone)
    {
        DOVirtual.Float(0f, 1f, time, (float f) =>
        {
            if (slider) slider.value = f;
            if (imgFill) imgFill.fillAmount = f;
        }).OnComplete(()=> {
            onLoadingDone?.Invoke();
        });
    }
    public void Close()
    {
        canvasGroup.DOFade(0f, 1f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}