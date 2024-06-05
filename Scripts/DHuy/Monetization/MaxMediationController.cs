using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;
using UnityEngine.SceneManagement;
public class MaxMediationController : MonoBehaviour
{
    public static MaxMediationController Instance;
    public static bool isMaxSdkReady = false;
    public const string LOADING_SCENE = "Loading";
    public const float TIME_WAIT_TO_SHOW_AOA = 10f;
    public const string SDK_KEY = "Q1a5bAd8OP4p-kQ9wF2EVkv8xDUB97MOqr92Tz13ADnNNvgTF9y3x5WidifrXzaIR-fFwVSGey9c1ZHSliv41a";
    public const string ID_ADS_BANNER = "96038777a1bf9f73";
    public const string ID_ADS_INTERTISTIAL = "a64c855b533903af";
    public const string ID_ADS_REWARD = "9d627b34d931d4a4";
    public const string ID_ADS_MREC = "0ff247d64a0f3a3a";
    public const string ID_APP_OPEN = "0406d6bfa615fcd3";
    public bool isShowingAppOpenAds;
    public Action onIntertistialClose;

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
        MaxSdk.SetSdkKey(SDK_KEY);
        MaxSdk.SetUserId("USER_ID");
        MaxSdk.InitializeSdk();
    }

    private void Start()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {
            isMaxSdkReady = true;
            MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAppOpenDismissedEvent;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
            MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
            InitializeBannerAds(ID_ADS_BANNER);
            InitializeInterstitialAds();
            InitializeRewardedAds();
            InitializeMRecAds();

            if (Constants.SHOW_ADS)
            {
                MaxSdk.ShowBanner(ID_ADS_BANNER);
            }
            MaxSdk.CreateMRec(ID_ADS_MREC, MaxSdkBase.AdViewPosition.BottomCenter);
            MaxSdk.LoadMRec(ID_ADS_MREC);
            MaxSdk.StartMRecAutoRefresh(ID_ADS_MREC);
        };
    }
    private void OnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo impressionData)
    {
        double revenue = impressionData.Revenue;
        var impressionParameters = new[] {
          new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
          new Firebase.Analytics.Parameter("ad_source", impressionData.NetworkName),
          new Firebase.Analytics.Parameter("ad_unit_name", impressionData.AdUnitIdentifier),
          new Firebase.Analytics.Parameter("ad_format", impressionData.AdFormat),
          new Firebase.Analytics.Parameter("value", revenue),
          new Firebase.Analytics.Parameter("currency", "USD"),
        };
        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
    }
    private void OnAppOpenDismissedEvent(string arg1, MaxSdkBase.AdInfo adInfo)
    {
        isShowingAppOpenAds = false;
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == LOADING_SCENE)
        {
            FindObjectOfType<Loading>().ContinueToScene();
        }
        RequestAndLoadAppOpen();
    }

    public void RequestAndLoadAppOpen()
    {
        MaxSdk.LoadAppOpenAd(ID_APP_OPEN);
    }

    public void ShowAppOpenIfReady()
    {
        if (MaxSdk.IsAppOpenAdReady(ID_APP_OPEN))
        {
            MaxSdk.ShowAppOpenAd(ID_APP_OPEN);
            isShowingAppOpenAds = true;
        }
        else
        {
            MaxSdk.LoadAppOpenAd(ID_APP_OPEN);
        }
    }

    public void ShowBanner()
    {
        MaxSdk.ShowBanner(ID_ADS_BANNER);
    }

    public void HideBanner()
    {
        MaxSdk.HideBanner(ID_ADS_BANNER);
    }

    public bool ShowIntertistialAds(Action onIntertistialClose = null)
    {
        this.onIntertistialClose = onIntertistialClose;
        if (Constants.SHOW_ADS == false)
        {
            return false;
        }
        if (AppFlyerManager.Instance) AppFlyerManager.Instance.LogInterIngameLogicCall();
        Dictionary<string, string> eventName = new Dictionary<string, string>();
        eventName.Add("Ad show", "Shown");
        if (MaxSdk.IsInterstitialReady(ID_ADS_INTERTISTIAL))
        {
            if (GameSystem.userdata.dicLevel[GameSystem.userdata.currentMode] < Constants.MIN_LEVEL_TO_SHOW_ADS)
            {
                return false;
            }
            MaxSdk.ShowInterstitial(ID_ADS_INTERTISTIAL);
            return true;
        }
        else
        {
            LoadInterstitial();
            return false;
        }
    }

    public bool ShowRewardedAds()
    {
        if (MaxSdk.IsRewardedAdReady(ID_ADS_REWARD))
        {
            Dictionary<string, string> eventName = new Dictionary<string, string>();
            if (eventName.ContainsKey("RewardsAd"))
            {
                eventName["RewardsAd"] = "Ready";
            }
            else
            {
                eventName.Add("RewardsAd", "Ready");
            }

            Dictionary<string, string> eventShowName = new Dictionary<string, string>();
            if (!eventName.ContainsKey("RewardsAd"))
            {
                eventName.Add("RewardsAd", "Show");
            }
            else
            {
                eventName["RewardsAd"] = "Show";
            }
            MaxSdk.ShowRewardedAd(ID_ADS_REWARD);
            return true;
        }
        else
        {
            LoadRewardedAd();
            return false;
        }
    }

    #region MREC 
    public void InitializeMRecAds()
    {
        MaxSdk.StopMRecAutoRefresh(ID_ADS_MREC);
        MaxSdk.CreateMRec(ID_ADS_MREC, MaxSdkBase.AdViewPosition.Centered);
        MaxSdkCallbacks.MRec.OnAdLoadedEvent += OnMRecAdLoadedEvent;
        MaxSdkCallbacks.MRec.OnAdLoadFailedEvent += OnMRecAdLoadFailedEvent;
        MaxSdkCallbacks.MRec.OnAdClickedEvent += OnMRecAdClickedEvent;
        MaxSdkCallbacks.MRec.OnAdExpandedEvent += OnMRecAdExpandedEvent;
        MaxSdkCallbacks.MRec.OnAdCollapsedEvent += OnMRecAdCollapsedEvent;
    }

    public void OnMRecAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { Debug.Log("Loaded mrec event"); }
    public void OnMRecAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo error) { }
    public void OnMRecAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }
    public void OnMRecAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }
    public void OnMRecAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }
    public void OnMRecAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }
    #endregion


    #region BANNER
    public void InitializeBannerAds(string bannerAdUnitId)
    {
        MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);
        MaxSdk.SetBannerBackgroundColor(bannerAdUnitId, Color.black);
        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
        MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;
    }
    private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        if (SceneManager.GetActiveScene().name == Constants.SCENE_LOADING)
        {
            HideBanner();
        }
        else
        {
            ShowBanner();
        }
    }
    private void OnBannerAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo) { }
    private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }
    private void OnBannerAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }
    private void OnBannerAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }
    #endregion


    #region INTERTISTIAL
    int retryAttempt;

    public void InitializeInterstitialAds()
    {
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
        LoadInterstitial();
    }

    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(ID_ADS_INTERTISTIAL);
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        if (AppFlyerManager.Instance) AppFlyerManager.Instance.LogInterReady();
        retryAttempt = 0;
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        retryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));
        Invoke("LoadInterstitial", (float)retryDelay);
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        if (AppFlyerManager.Instance) AppFlyerManager.Instance.LogInterShow();
    }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        LoadInterstitial();
    }
    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }
    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        onIntertistialClose?.Invoke();
        LoadInterstitial();
    }
    #endregion

    #region REWARDED
    int retryAttemptReward;

    public void InitializeRewardedAds()
    {
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(ID_ADS_REWARD);
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        retryAttemptReward = 0;
        if (AppFlyerManager.Instance) AppFlyerManager.Instance.LogRewardedReady();
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        retryAttemptReward++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttemptReward));
        Invoke("LoadRewardedAd", (float)retryDelay);
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        if (AppFlyerManager.Instance) AppFlyerManager.Instance.LogRewardedShow();
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        LoadRewardedAd();
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        LoadRewardedAd();
        if (AppFlyerManager.Instance) AppFlyerManager.Instance.LogRewardedComplete();
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        AdManager.Instance.HandleEarnReward();
    }
    #endregion

    const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
    public string GetRandomString(int length)
    {
        string rand = "";
        for (int i = 0; i < length; i++)
        {
            rand += alphabet[UnityEngine.Random.Range(0, alphabet.Length)];
        }
        return rand;
    }
}