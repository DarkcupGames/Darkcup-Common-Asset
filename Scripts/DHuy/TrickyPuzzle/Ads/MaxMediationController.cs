using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;
using UnityEngine.SceneManagement;
// using com.adjust.sdk;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

public class MaxMediationController : MonoBehaviour
{
    public static MaxMediationController Instance;
    public static bool isMaxSdkReady = false;
    public const string LOADING_SCENE = "Loading";
    public const float TIME_WAIT_TO_SHOW_AOA = 7f;
    public const string ID_ADS_BANNER = "28e91aa6bee54974"; // Retrieve the ID from your account
    public const string ID_ADS_INTERTISTIAL = "171d6e19c985e0d2"; // Retrieve the ID from your account
    public const string ID_ADS_REWARD = "6437ca2781b50ef6"; // Retrieve the ID from your account
    public const string ID_ADS_MREC = "8b67b47bcf73a165";
    public const string ID_APP_OPEN = "f5be3f6ce0594597";
    public long lastShowIntertistial;
    public float waitToShowAOA;
    public float currentTime;
    public bool isShowingAppOpenAds;
    public Action onIntertistialClose;
    public static bool isShowingIntertistial;
    public static bool isShowingRewardedAds;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            MaxSdk.SetSdkKey("zQf_Pgpk4i5eo1zxt0MCBulmp1dvzNe4pqc9fwsc4QLE7udaqsvoTKNe7M_35VKgRymgJT1GhIQLO2fpyiOwiM");
            MaxSdk.SetUserId("USER_ID");
            MaxSdk.InitializeSdk();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {
            isMaxSdkReady = true;
            MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAppOpenDismissedEvent;
            MaxSdkCallbacks.AppOpen.OnAdLoadedEvent += OnAppOpenLoaded;
            MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
            MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent += (adUnit, adInfo) =>
            {
                TrackAdRevenue(adInfo);
            };
            InitializeBannerAds(ID_ADS_BANNER);
            InitializeInterstitialAds();
            InitializeRewardedAds();
            InitializeMRecAds();
            RequestAndLoadAppOpen();
            if (Constants.SHOW_ADS)
            {
                MaxSdk.ShowBanner(ID_ADS_BANNER);
            }
        };
        //AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
    }

    private void OnAppOpenDismissedEvent(string arg1, MaxSdkBase.AdInfo adInfo)
    {
        try
        {
            isShowingAppOpenAds = false;

            //Scene scene = SceneManager.GetActiveScene();
            //if (scene.name == LOADING_SCENE)
            //{
            //    FindObjectOfType<Loading>().AllowChangeScene();
            //}
            RequestAndLoadAppOpen();
            if (BackgroundSong.Instance != null) BackgroundSong.Instance.SetMuted(false);
        }
        catch (Exception e)
        {
            Debug.Log("error: " + e.Message);
        }
    }

    public void RequestAndLoadAppOpen()
    {
        MaxSdk.LoadAppOpenAd(ID_APP_OPEN);
    }

    private void OnAppOpenLoaded(string arg1, MaxSdkBase.AdInfo arg2)
    {
        //if (SceneManager.GetActiveScene().name == Constants.SCENE_LOADING)
        //{
        //    FindAnyObjectByType<Loading>().FinishLoadAndShowAppOpen();
        //}
    }

    public void ShowAppOpenIfReady()
    {
        if (MaxSdk.IsAppOpenAdReady(ID_APP_OPEN))
        {
            MaxSdk.ShowAppOpenAd(ID_APP_OPEN);
            isShowingAppOpenAds = true;
            if (BackgroundSong.Instance != null) BackgroundSong.Instance.SetMuted(true);
        }
        else
        {
            MaxSdk.LoadAppOpenAd(ID_APP_OPEN);
        }
    }

    public void OnAppStateChanged(AppState state)
    {
        try
        {
            if (isShowingRewardedAds || isShowingIntertistial) return;
            if (state == AppState.Foreground)
            {
                ShowAppOpenIfReady();
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public void ShowBanner()
    {
        MaxSdk.SetBannerBackgroundColor(ID_ADS_BANNER, Color.clear);
        if (GameSystem.userdata.boughtItems.Contains("no_ads"))
        {
            HideBanner();
            return;
        }
        if (Constants.SHOW_ADS)
        {
            MaxSdk.ShowBanner(ID_ADS_BANNER);
        }
        else
        {
            HideBanner();
        }
    }

    public void HideBanner()
    {
        MaxSdk.HideBanner(ID_ADS_BANNER);
        MaxSdk.SetBannerBackgroundColor(ID_ADS_BANNER, Color.clear);
    }

    public void ShowIntertistialAds(Action onIntertistialClose = null)
    {
        Debug.Log($"calling intertisitial, ready = {MaxSdk.IsInterstitialReady(ID_ADS_INTERTISTIAL)}");
        this.onIntertistialClose = onIntertistialClose;
        if (Constants.SHOW_ADS == false)
        {
            onIntertistialClose?.Invoke();
            return;
        }
        if (GameSystem.userdata.boughtItems.Contains("no_ads"))
        {
            onIntertistialClose?.Invoke();
            return;
        }
        Dictionary<string, string> eventName = new Dictionary<string, string>();
        eventName.Add("Ad show", "Shown");
        if (MaxSdk.IsInterstitialReady(ID_ADS_INTERTISTIAL))
        {
            float seconds = (DateTime.Now.Ticks - lastShowIntertistial) / 10000;
            if (seconds < Constants.MIN_SECONDS_BETWEEN_INTERTISITAL)
            {
                //Debug.Log($"skip by last show intertisial,  Time.realtimeSinceStartup - lastShowIntertistial= {Time.realtimeSinceStartup - lastShowIntertistial}");
                onIntertistialClose?.Invoke();
                DeepTrack.LogEvent(DeepTrackEvent.inter_fail);
                return;
            }
            Debug.Log($"show intertisitial");
            MaxSdk.ShowInterstitial(ID_ADS_INTERTISTIAL);
            isShowingIntertistial = true;
            DeepTrack.LogEvent(DeepTrackEvent.inter_success);
        }
        else
        {
            Debug.Log($"load intertisitial");
            LoadInterstitial();
            onIntertistialClose?.Invoke();
            DeepTrack.LogEvent(DeepTrackEvent.inter_fail);
        }
    }

    public void ShowRewardedAds()
    {
        Debug.Log($"calling show reward ads, ready = {MaxSdk.IsRewardedAdReady(ID_ADS_REWARD)}");
        if (GameSystem.userdata.boughtItems.Contains("no_ads"))
        {
            AdManager.Instance.HandleEarnReward();
            return;
        }
        if (MaxSdk.IsRewardedAdReady(ID_ADS_REWARD))
        {
            MaxSdk.ShowRewardedAd(ID_ADS_REWARD);
            isShowingRewardedAds = true;
            DeepTrack.LogEvent(DeepTrackEvent.reward_success);
        }
        else
        {
            Debug.Log("load reward ads");
            LoadRewardedAd();
            DeepTrack.LogEvent(DeepTrackEvent.reward_fail);
        }
    }

    #region MREC 


    public void InitializeMRecAds()
    {
        MaxSdk.StopMRecAutoRefresh(ID_ADS_MREC);
        MaxSdk.CreateMRec(ID_ADS_MREC, MaxSdkBase.AdViewPosition.BottomCenter);
        MaxSdkCallbacks.MRec.OnAdLoadedEvent += OnMRecAdLoadedEvent;
        MaxSdkCallbacks.MRec.OnAdLoadFailedEvent += OnMRecAdLoadFailedEvent;
        MaxSdkCallbacks.MRec.OnAdClickedEvent += OnMRecAdClickedEvent;
        MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnMRecAdRevenuePaidEvent;
        MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += (adUnit, adInfo) =>
        {
            TrackAdRevenue(adInfo);
        };
        MaxSdkCallbacks.MRec.OnAdExpandedEvent += OnMRecAdExpandedEvent;
        MaxSdkCallbacks.MRec.OnAdCollapsedEvent += OnMRecAdCollapsedEvent;
    }

    public void OnMRecAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        if (SceneManager.GetActiveScene().name == Constants.SCENE_HOME)
        {
            MaxMediationController.Instance.SetMrecVisible(true);
        }
        else
        {
            MaxMediationController.Instance.SetMrecVisible(false);
        }
    }

    public void OnMRecAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo error) { }

    public void OnMRecAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    public void OnMRecAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    public void OnMRecAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    public void OnMRecAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }
    public void SetMrecVisible(bool visible)
    {
        if (GameSystem.userdata.boughtItems.Contains("no_ads"))
        {
            MaxSdk.HideMRec(ID_ADS_MREC);
            return;
        }
        if (visible)
        {
            MaxSdk.ShowMRec(ID_ADS_MREC);
        }
        else
        {
            MaxSdk.HideMRec(ID_ADS_MREC);
        }
    }
    #endregion


    #region BANNER
    public void InitializeBannerAds(string bannerAdUnitId)
    {
        MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);
        MaxSdk.SetBannerBackgroundColor(bannerAdUnitId, Color.clear);
        MaxSdk.StartBannerAutoRefresh(bannerAdUnitId);
        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
        MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += (adUnit, adInfo) =>
        {
            TrackAdRevenue(adInfo);
        };
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

    private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {

    }

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
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += (adUnit, adInfo) =>
        {
            TrackAdRevenue(adInfo);
        };
        LoadInterstitial();
    }
    #region TRACK_REVENUE
    private void TrackAdRevenue(MaxSdkBase.AdInfo adInfo)
    {
        // AdjustAdRevenue adjustAdRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAppLovinMAX);
        //
        // adjustAdRevenue.setRevenue(adInfo.Revenue, "USD");
        // adjustAdRevenue.setAdRevenueNetwork(adInfo.NetworkName);
        // adjustAdRevenue.setAdRevenueUnit(adInfo.AdUnitIdentifier);
        // adjustAdRevenue.setAdRevenuePlacement(adInfo.Placement);
        //
        // Adjust.trackAdRevenue(adjustAdRevenue);
        // LogAdValueAdjust(adInfo.Revenue);
    }

    public void LogAdValueAdjust(double value)
    {
        if (string.IsNullOrEmpty("ad_value"))
            return;
#if NOT_ADJUST
#else
        // AdjustEvent adjustEvent = new AdjustEvent("paid_ad_impression_value");
        // adjustEvent.setRevenue(value, "USD");
        // Adjust.trackEvent(adjustEvent);
#endif
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
            new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
        };
        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
    }
    #endregion

    private void LoadInterstitial()
    {
        //MaxSdk.LoadInterstitial(ID_ADS_INTERTISTIAL);
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        retryAttempt = 0;
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        retryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));
        Invoke(nameof(LoadInterstitial), (float)retryDelay);
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {

    }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        LoadInterstitial();
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {

    }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        lastShowIntertistial = DateTime.Now.Ticks;
        Debug.Log("Ad close");
        onIntertistialClose?.Invoke();
        LoadInterstitial();
        isShowingIntertistial = false;
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
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += (adUnit, adInfo) =>
        {
            TrackAdRevenue(adInfo);
        };
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        //MaxSdk.LoadRewardedAd(ID_ADS_REWARD);
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        retryAttemptReward = 0;
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        retryAttemptReward++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttemptReward));
        Invoke("LoadRewardedAd", (float)retryDelay);
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        LoadRewardedAd();
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { /*FirebaseAnalytics.LogEvent("ads_reward_click");*/ }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        LoadRewardedAd();
        isShowingRewardedAds = false;
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        AdManager.Instance.HandleEarnReward();
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {

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