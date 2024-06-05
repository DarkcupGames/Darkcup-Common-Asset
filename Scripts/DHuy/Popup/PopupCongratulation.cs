using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class PopupCongratulation : MonoBehaviour
{
    public const float SCALE_TIME = 0.5f;
    [SerializeField] private MapCamera _cameraMovement;
    [SerializeField] private Transform completeTitle;
    [SerializeField] private TextMeshProUGUI txtCountryName;
    [SerializeField] private Button btnNext;

    private LevelDrawFlag nextLevel;

    private void OnEnable()
    {
        btnNext.gameObject.SetActive(false);
        BackgroundMusic.Instance.PlayVictoryMusic();
        GameSceneManager.Instance.HideNormalCanvas();
        GameManager.Instance.StartCoroutine(IEEffect());
        GameManager.Instance.LoadLevelAddressable(GameSystem.userdata.level + 1, (sourceLevel) =>
        {
            nextLevel = sourceLevel;
        });
        FirebaseManager.analytics.LogUIAppear(SceneManager.GetActiveScene().name, "popup_congratulation");
    }

    IEnumerator IEEffect()
    {
        CameraManager.Instance.ZoomOut();
        completeTitle.localScale = Vector2.zero;
        completeTitle.DOScale(1f, SCALE_TIME).SetEase(Ease.OutBack);
        txtCountryName.text = GameManager.Instance.currentLevel.countryData.countryName.ToUpper();
        yield return new WaitForSeconds(SCALE_TIME);
        yield return new WaitForSeconds(1f);
        btnNext.gameObject.SetActive(true);
        btnNext.transform.localScale = Vector2.zero;
        btnNext.transform.DOScale(1f, SCALE_TIME).SetEase(Ease.OutBack);
    }

    public void ShowAdsAndNext()
    {
        DarkcupGames.AdManagerIronsource.Instance.ShowIntertistial(Next, "inter_finish_level");
    }

    public void Next()
    {
        GameSceneManager.Instance.ShowMap();
        GameManager.Instance.HideDrawedFlag();
        string currentCountry = GameManager.Instance.currentLevel.countryData.countryName;
        if (nextLevel != null)
        {
            _cameraMovement.UpdateViewpoint(currentCountry, nextLevel.countryData.countryName);
            GameManager.Instance.popupVisitNewCountry.ShowCoutry(nextLevel.countryData);
        } else
        {
            GameManager.Instance.LoadLevelAddressable(GameSystem.userdata.level + 1, (nextLevel) =>
            {
                _cameraMovement.UpdateViewpoint(currentCountry, nextLevel.countryData.countryName);
                GameManager.Instance.popupVisitNewCountry.ShowCoutry(nextLevel.countryData);
            });
        }
        GameSystem.userdata.level++;
        if (GameSystem.userdata.level > GameSystem.userdata.maxLevel)
        {
            GameSystem.userdata.maxLevel = GameSystem.userdata.level;
        }
        GameSystem.SaveUserDataToLocal();
        gameObject.SetActive(false);
        FirebaseManager.analytics.LogButtonClick(SceneManager.GetActiveScene().name, "button_congratulation_next");
        FirebaseManager.analytics.SetUserLastLevel(GameSystem.userdata.level);
        FirebaseManager.analytics.SetUserLevelMax(GameSystem.userdata.maxLevel);
    }
}