using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PopupRemoveAds : MonoBehaviour
{
    public GameObject settingBoard;
    public TextMeshProUGUI txtPrice;
    public TextPricingIAP txtPriceIAP;
    public Vector3 popupScale;
    public float doSpeed;

    private Vector3 firstScale;
    private void OnEnable()
    {
        string id = IAP_ID.no_ads.ToString();
        bool boughNoAds = GameSystem.userdata.boughtItems.Contains(id);
        if (boughNoAds) { txtPriceIAP.enabled = false; txtPrice.SetText("Purchased"); }

        firstScale = transform.localScale;
        settingBoard.transform.DOScale(popupScale, doSpeed).SetEase(Ease.OutBack).SetUpdate(true);
        FirebaseManager.analytics.LogUIAppear(SceneManager.GetActiveScene().name, "popup_remove_ads");
    }
    private void Start()
    {

    }
    public void TurnOff()
    {
        settingBoard.transform.DOScale(firstScale, doSpeed).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
