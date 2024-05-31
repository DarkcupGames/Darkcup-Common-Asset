using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class PopupVisitNewCountry : MonoBehaviour
{
    [SerializeField] private Transform body;
    [SerializeField] private TextMeshProUGUI txtCountryName;
    [SerializeField] private TextMeshProUGUI txtTravelPercent;
    [SerializeField] private Image imgSlider;
    private CountryData countryData;

    private void OnEnable()
    {
        StartCoroutine(IEEffect());
        FirebaseManager.analytics.LogUIAppear(SceneManager.GetActiveScene().name, "popup_visit_new_country");
    }
    IEnumerator IEEffect()
    {
        float oldPercent = ((float)(GameSystem.userdata.countryLib.Keys.Count - 1) / MapManager.Instance.dicCountry.Count);
        float percent = ((float)GameSystem.userdata.countryLib.Keys.Count / MapManager.Instance.dicCountry.Count);
        if (oldPercent > 1f) percent = 1f;
        if (percent > 1f) percent = 1f;
        imgSlider.fillAmount = oldPercent;
        txtTravelPercent.DOFade(0f, 0f);
        body.localScale = Vector2.zero;
        body.DOScale(1f, 1f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(1f);
        txtTravelPercent.DOFade(1f, 1f);
        DOVirtual.Float(oldPercent, percent, 2f, (float f) =>
        {
            imgSlider.fillAmount = f;
            txtTravelPercent.text = $"TRAVEL MAP {(f * 100).ToString("F2")}%";
        });
    }

    public void ShowCoutry(CountryData countryData)
    {
        this.countryData = countryData;
        txtCountryName.text = countryData.countryName;
    }

    public void Play()
    {
        GameManager.Instance.LoadLevelAndStartGame(GameSystem.userdata.level);
        gameObject.SetActive(false);
        FirebaseManager.analytics.LogButtonClick(SceneManager.GetActiveScene().name, "button_new_country_play");
    }
}