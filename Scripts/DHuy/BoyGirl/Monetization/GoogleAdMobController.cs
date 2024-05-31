using UnityEngine.Events;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Ump.Api;

public class GoogleAdMobController : MonoBehaviour
{
    public static GoogleAdMobController Instance;

#if UNITY_ANDROID
    public const string ADD_ID_BANNER = "ca-app-pub-3245796729995506/4269283388";
    public const string ADD_ID_REWARDED = "ca-app-pub-3245796729995506/3300365380";
    public const string ADD_ID_APP_OPEN = "ca-app-pub-3245796729995506/7048038708";
    public const string ADD_ID_INTERTISTIAL = "ca-app-pub-3245796729995506/5095279893";
#endif

#if UNITY_IOS
    public const string ADD_ID_BANNER = "ca-app-pub-3940256099942544/6300978111";
    public const string ADD_ID_REWARDED = "ca-app-pub-3940256099942544/5224354917";
    //public const string ADD_ID_APP_OPEN = "ca-app-pub-3940256099942544/3419835294"; //test id
    public const string ADD_ID_APP_OPEN = "ca-app-pub-9082660478786368/1492962994"; //real id commandoo
    public const string ADD_ID_INTERTISTIAL = "ca-app-pub-3940256099942544/1033173712";
#endif

    private readonly AdPosition BANNER_POSITION = AdPosition.Bottom;
    private readonly TimeSpan APPOPEN_TIMEOUT = TimeSpan.FromHours(4);
    private DateTime appOpenExpireTime;
    public AppOpenAd appOpenAd;
    public BannerView bannerView;
    public InterstitialAd interstitialAd;
    public RewardedAd rewardedAd;
    public bool isShowingAppOpenAd;
    private Action onIntertistialClose;
    private float gotoBackgroundTime;
    private float lastLoadIntertistial;
    private float lastLoadRewarded;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        MobileAds.SetiOSAppPauseOnBackground(true);
        List<String> deviceIds = new List<String>() { AdRequest.TestDeviceSimulator };
#if UNITY_IPHONE
        deviceIds.Add("96e23e80653bb28980d3f40beb58915c");
#elif UNITY_ANDROID
        deviceIds.Add("75EF8D155528C04DACBBA6F36F433035");
#endif
        RequestConfiguration requestConfiguration = new RequestConfiguration.Builder()
            .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.Unspecified)
            .SetTestDeviceIds(deviceIds).build();
        MobileAds.SetRequestConfiguration(requestConfiguration);
        //MobileAds.Initialize(HandleInitCompleteAction);
        AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
        ConsentRequestParameters request = new ConsentRequestParameters();
        ConsentInformation.Update(request, OnConsentInfoUpdated);
    }

    void OnConsentInfoUpdated(FormError consentError)
    {
        if (consentError != null)
        {
            UnityEngine.Debug.LogError(consentError);
            return;
        }
        ConsentForm.Load((form, error) =>
        {
            if (error != null)
            {
                UnityEngine.Debug.LogError(consentError);
                return;
            }
            form.Show((error) => {
                if (error != null)
                {
                    UnityEngine.Debug.LogError(consentError);
                    return;
                }
                MobileAds.Initialize(HandleInitCompleteAction);
            });
        });
    }

    private void HandleInitCompleteAction(InitializationStatus initstatus)
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            Debug.Log("Initialization complete.");
            RequestBannerAd();
            RequestAndLoadAppOpenAd();
            RequestAndLoadInterstitialAd();
            RequestAndLoadRewardedAd();
        });
    }

    public void RequestBannerAd()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
        }
        bannerView = new BannerView(ADD_ID_BANNER, AdSize.Banner, BANNER_POSITION);
        bannerView.LoadAd(CreateAdRequest());
        bannerView.Show();
    }

    public void DestroyBannerAd()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
        }
    }

    private AdRequest CreateAdRequest()
    {
        return new AdRequest.Builder().Build();
    }

    public void RequestAndLoadInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }
        PrintStatus("Loading the interstitial ad.");
        var adRequest = new AdRequest.Builder().Build();
        InterstitialAd.Load(ADD_ID_INTERTISTIAL, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("interstitial ad failed to load an ad " + "with error : " + error);
                return;
            }
            Debug.Log("Interstitial ad loaded with response : " + ad.GetResponseInfo());
            interstitialAd = ad;
            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                MainThreadManager.Instance.ExecuteInUpdate(() =>
                {
                    onIntertistialClose?.Invoke();
                    RequestAndLoadInterstitialAd();
                    AdManager.lastShowIntertistial = Time.time;
                });
            };
        });
    }

    public bool ShowIntertistialAds(System.Action onSuccess = null, System.Action onFail = null)
    {
        this.onIntertistialClose = onSuccess;
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            if (GameSystem.userdata.dicLevel[GameSystem.userdata.currentMode] < Constants.MIN_LEVEL_TO_SHOW_ADS)
            {
                onFail?.Invoke();
                return false;
            }
            Debug.Log("Showing interstitial ad.");
            interstitialAd.Show();
            return true;
        }
        else
        {
            if (Time.time - lastLoadIntertistial > 5f)
            {
                lastLoadIntertistial = Time.time;
                RequestAndLoadInterstitialAd();
            }
            Debug.LogError("Interstitial ad is not ready yet.");
            onFail?.Invoke();
            return false;
        }
    }

    public void RequestAndLoadRewardedAd()
    {
        RewardedAd.Load(ADD_ID_REWARDED, CreateAdRequest(), (RewardedAd ad, LoadAdError loadError) =>
        {
            if (loadError != null)
            {
                PrintStatus("Rewarded ad failed to load with error: " + loadError.GetMessage());
                return;
            }
            else if (ad == null)
            {
                PrintStatus("Rewarded ad failed to load.");
                return;
            }
            PrintStatus("Rewarded ad loaded.");
            rewardedAd = ad;
            ad.OnAdFullScreenContentClosed += () =>
            {
                PrintStatus("Rewarded ad closed.");
                AdManager.Instance.HandleEarnReward();
                RequestAndLoadRewardedAd();
            };
        });
    }

    public bool ShowRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Show((Reward reward) =>
            {
                PrintStatus("Rewarded ad granted a reward: " + reward.Amount);
            });
            return true;
        }
        else
        {
            if (Time.time - lastLoadRewarded > 5f)
            {
                lastLoadRewarded = Time.time;
                RequestAndLoadRewardedAd();
            }
            PrintStatus("Rewarded ad is not ready yet.");
            return false;
        }
    }

    public bool IsAppOpenAdAvailable
    {
        get
        {
            return (!isShowingAppOpenAd
                    && appOpenAd != null
                    && DateTime.Now < appOpenExpireTime);
        }
    }

    public void OnAppStateChanged(AppState state)
    {
        // Display the app open ad when the app is foregrounded.
        //UnityEngine.Debug.Log("App State is " + state);

        //if (state == AppState.Background)
        //{
        //    gotoBackgroundTime = Time.realtimeSinceStartup;
        //}

        //// OnAppStateChanged is not guaranteed to execute on the Unity UI thread.
        //MobileAdsEventExecutor.ExecuteInUpdate(() => {
        //    if (state == AppState.Foreground) {
        //        ShowAppOpenAd();
        //    }
        //});
    }

    public void OnAppOpenClose()
    {
        if (SceneManager.GetActiveScene().name == Constants.SCENE_LOADING)
        {
            FindObjectOfType<Loading>().ContinueToScene();
        }
        RequestAndLoadAppOpenAd();
    }

    public void RequestAndLoadAppOpenAd()
    {
        PrintStatus("Requesting App Open ad.");
        string adUnitId = ADD_ID_APP_OPEN;
        AppOpenAd.Load(adUnitId, ScreenOrientation.Portrait, CreateAdRequest(), OnAppOpenAdLoad);
    }

    private void OnAppOpenAdLoad(AppOpenAd ad, LoadAdError error)
    {
        if (error != null)
        {
            PrintStatus("App Open ad failed to load with error: " + error);
            return;
        }
        PrintStatus("App Open ad loaded. Please background the app and return.");
        this.appOpenAd = ad;
        this.appOpenExpireTime = DateTime.Now + APPOPEN_TIMEOUT;
        if (SceneManager.GetActiveScene().name == Constants.SCENE_LOADING)
        {
            ShowAppOpenAd();
        }
    }

    public void ShowAppOpenAd()
    {
        PrintStatus($"Calling show appopen ads, bacgroundtime = {Time.realtimeSinceStartup - gotoBackgroundTime}");
        PrintStatus($"App open available = {IsAppOpenAdAvailable}, appopen != null = {appOpenAd != null}");
        if (!IsAppOpenAdAvailable)
        {
            return;
        }
        if (SceneManager.GetActiveScene().name != Constants.SCENE_LOADING)
        {
            float time = Time.realtimeSinceStartup - gotoBackgroundTime;
            if (time < Constants.BACKGROUND_TIME_TO_SHOW_APP_OPEN)
            {
                Debug.Log($"background time = {time} not enough time to show app open");
                return;
            }
        }
        this.appOpenAd.OnAdFullScreenContentClosed += () =>
        {
            PrintStatus("App Open ad dismissed.");
            isShowingAppOpenAd = false;
            if (this.appOpenAd != null)
            {
                this.appOpenAd.Destroy();
                this.appOpenAd = null;
            }
            OnAppOpenClose();
        };
        this.appOpenAd.OnAdFullScreenContentFailed += (error) =>
        {
            PrintStatus("App Open ad failed to present with error: " + error.GetMessage());
            isShowingAppOpenAd = false;
            if (this.appOpenAd != null)
            {
                this.appOpenAd.Destroy();
                this.appOpenAd = null;
            }
            OnAppOpenClose();
        };
        isShowingAppOpenAd = true;
        appOpenAd.Show();
    }

    public void OpenAdInspector()
    {
        PrintStatus("Open ad Inspector.");
        MobileAds.OpenAdInspector((error) =>
        {
            if (error != null)
            {
                PrintStatus("ad Inspector failed to open with error: " + error);
            }
            else
            {
                PrintStatus("Ad Inspector opened successfully.");
            }
        });
    }

    private void PrintStatus(string message)
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            Debug.Log(message);
        });
    }
}