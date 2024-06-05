using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DarkcupGames;
using UnityEngine.SceneManagement;
using Spine.Unity;
using System.Linq;
using DG.Tweening;

public class Loading : MonoBehaviour
{
    public Image fillImage;
    public Slider slider;
    public List<SkeletonGraphic> anims;
    public MaxMediationController maxMediation;
    public PopupNoInternet popupNoInternet;

    public bool showAds = false;
    float startLoadingTime;
    AsyncOperation load;

    private void Start()
    {
        //DVAH.FireBaseManager.Instant.Init(null);
        //DVAH.AdMHighFather_AppLovin.Instant.Init();
        CheckInternet();
    }

    public void CheckInternet()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ShowHideChangeSceneLogic.ShowPopup(popupNoInternet.gameObject);
        } else
        {
            popupNoInternet.gameObject.SetActive(false);
            StartLoading();
        }
    }

    private void StartLoading() {
        load = SceneManager.LoadSceneAsync(Constants.SCENE_GAMEPLAY);
        load.allowSceneActivation = false;
        fillImage.fillAmount = 0;
        fillImage.DOFillAmount(1f, Constants.APP_OPEN_LOADING_TIME_OUT);
        startLoadingTime = Time.time;
        AdManager.Instance.SetBannerVisible(false);
    }

    private void Update()
    {
        if (load == null) return;
        float time = Time.time - startLoadingTime;

        if (time > Constants.APP_OPEN_LOADING_TIME_OUT && !GoogleAdmobController.isShowingAppOpenAds)
        {
            load.allowSceneActivation = true;
            Firebase.Analytics.FirebaseAnalytics.LogEvent("loading_complete");
        }
    }

    public void FinishLoadAndShowAppOpen()
    {
        if (SceneManager.GetActiveScene().name != Constants.SCENE_LOADING) return;
        
        DOTween.Kill(fillImage);
        fillImage.DOFillAmount(1f, 3f).OnComplete(() =>
        {
            if (Constants.SHOW_ADS && Constants.SHOW_APP_OPEN_LOADING)
            {
                //MaxMediationController.Instance.ShowAppOpenIfReady();
                //DVAH.AdMHighFather_AppLovin.Instant.ShowAdOpen(0);
                GoogleAdmobController.Instance.ShowAppOpen();
            }
            else
            {
                AllowChangeScene();
            }
        });
    }

    public void AllowChangeScene()
    {
        load.allowSceneActivation = true;
        Firebase.Analytics.FirebaseAnalytics.LogEvent("loading_complete");
        Firebase.Analytics.FirebaseAnalytics.LogEvent("test_event");
        //Firebase.Analytics.FirebaseAnalytics.LogEvent("loading_complete");
    }
}


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;
//using DarkcupGames;
//using UnityEngine.SceneManagement;

//public class Loading : MonoBehaviour
//{
//    public Image fillImage;
//    public Slider slider;
//    public GoogleAdMobController googleAd;
//    bool showAds = false;
//    float startLoadingTime;
//    AsyncOperation load;

//    private void Start()
//    {
//        load = SceneManager.LoadSceneAsync("Home");
//        load.allowSceneActivation = false;
//        LeanTween.value(0f, 1f, Constants.LOADING_TIME).setOnUpdate((float f) => {
//            if (fillImage) fillImage.fillAmount = f;
//            if (slider) slider.value = f;
//        });
//        startLoadingTime = Time.time;
//    }

//    private void Update()
//    {
//        CheckForAppOpenAd();
//    }

//    public void CheckForAppOpenAd()
//    {
//        if (load == null) return;
//        float time = Time.time - startLoadingTime;

//        if (time > Constants.APP_OPEN_LOADING_TIME_OUT && !googleAd.isShowingAppOpenAd)
//        {
//            //showAds = true;
//            load.allowSceneActivation = true;
//        }
//    }

//    public void ShowAppOpenAds()
//    {
//        googleAd.ShowAppOpenAd();
//        //showAds = true;
//    }

//    public void ContinueToScene()
//    {
//        if (SceneManager.GetActiveScene().name != "Loading") return;
//        load.allowSceneActivation = true;
//    }
//}
