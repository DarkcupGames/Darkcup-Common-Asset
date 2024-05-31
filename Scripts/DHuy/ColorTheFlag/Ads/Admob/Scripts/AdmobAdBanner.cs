using GoogleMobileAds.Api;
using System;
using UnityEngine;
using DarkcupGames;

public class AdmobAdBanner : AdmobAds
{
    public string BANNER_ID;
    public bool useCollapsible = true;
    public AdPosition position = AdPosition.Bottom;
    private BannerView bannerView;
    private string uuid;
    private bool available;
    private bool isShowingAds;

    public override void Init()
    {
        bannerView = new BannerView(BANNER_ID, AdSize.Banner, position);
        bannerView.OnBannerAdLoaded += () =>
        {
            available = true;
            CollapsibleBannerFlow.Instance.OnCollapsibleAdsLoaded();
        };
        bannerView.OnBannerAdLoadFailed += (err) =>
        {
            Debug.LogError("load banner failed");
            Debug.LogError(err.GetMessage());
            available = false;
            CollapsibleBannerFlow.Instance.OnCollapsibleAdsFailed();
        };
        GenerateNewUUID();
    }

    public override void LoadAds()
    {
        if (AdmobManager.Instance.showAds == false) return;
        if (AdmobManager.isReady == false)
        {
            Debug.LogError("admob is not ready for load banner");
            return;
        }
        var adRequest = new AdRequest();
        if (useCollapsible)
        {
            adRequest.Extras.Add("collapsible", "bottom");
            adRequest.Extras.Add("collapsible_request_id", uuid);
        }
        if (bannerView == null) Init();
        bannerView.LoadAd(adRequest);
    }

    public override bool ShowAds(Action onShowAdsComplete)
    {
        SetBannerVisible(available);
        return available;
    }
    public void SetBannerVisible(bool visible)
    {
        if (bannerView == null) return;
        if (visible)
        {
            bannerView.Show();
            isShowingAds = true;
            FirebaseManager.analytics.LogAdsBannerRecorded("admob", "bottom");
        } else
        {
            bannerView.Hide();
            isShowingAds = false;
        }
    }

    [ContextMenu("Test")]
    public void GenerateNewUUID()
    {
        uuid = Guid.NewGuid().ToString();
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