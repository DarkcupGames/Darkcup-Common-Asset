using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;
using UnityEngine;
using DarkcupGames;

[RequireComponent(typeof(MainThreadScriptRunner))]
public class AdmobAppOpen : AdmobAds
{
    public string APPOPEN_ID;
    private AppOpenAd appOpenAd;
    private DateTime expireTime;
    private int retryCount;
    private Action onShowAdsCompleted;
    private MainThreadScriptRunner mainThread;
    private bool available;
    private bool isShowingAds;

    private void Awake()
    {
        mainThread = GetComponent<MainThreadScriptRunner>();
    }

    public override void Init()
    {
        //logic migrated to load ads
    }

    public override void LoadAds()
    {
        if (AdmobManager.Instance.showAds == false)
        {
            Debug.Log("skip by showads = false");
            return;
        }
        if (AdmobManager.isReady == false)
        {
            Debug.LogError("admob is not ready for load app open!");
            return;
        }
        if (appOpenAd != null)
        {
            appOpenAd.Destroy();
            appOpenAd = null;
        }
        if (showDebug) Debug.Log("Loading the app open ad.");
        var adRequest = new AdRequest();
        AppOpenAd.Load(APPOPEN_ID, adRequest, (AppOpenAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("app open ad failed to load an ad with error : " + error);
                available = false;
                retryCount++;
                float time = Mathf.Pow(2, retryCount);
                if (time > 64) time = 64;
                Invoke(nameof(LoadAds), time);
                return;
            }
            if (showDebug) Debug.Log("App open ad loaded with response : " + ad.GetResponseInfo());
            expireTime = DateTime.Now + TimeSpan.FromHours(4);
            appOpenAd = ad;
            available = true;
            retryCount = 0;
            appOpenAd.OnAdFullScreenContentClosed += () =>
            {
                if (showDebug) Debug.Log("App open ad full screen content closed.");
                LoadAds();
                mainThread.Run(onShowAdsCompleted);
                isShowingAds = false;
            };
        });
    }

    public override bool ShowAds(Action onShowAdsCompleted)
    {
        this.onShowAdsCompleted = onShowAdsCompleted;

        if (appOpenAd != null && appOpenAd.CanShowAd())
        {
            if (showDebug) Debug.Log("Showing app open ad.");
            appOpenAd.Show();
            isShowingAds = true;
            return true;
        }
        else
        {
            if (showDebug) Debug.LogError("App open ad is not ready yet.");
            isShowingAds = false;
            return false;
        }
    }

    public override bool IsAdsAvailable()
    {
        return available;
    }

    public override bool IsShowingAds()
    {
        return isShowingAds;
    }
}