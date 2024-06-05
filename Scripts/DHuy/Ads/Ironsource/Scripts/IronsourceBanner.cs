using System;
using UnityEngine;

namespace DarkcupGames
{
    public class IronsourceBanner : IronsourceAds
    {
        public IronSourceBannerPosition position = IronSourceBannerPosition.BOTTOM;
        public Func<bool> canShowAds;
        private int retryCount;
        private bool available;
        private bool isShowingAds;

        public override void Init()
        {
            IronSourceBannerEvents.onAdLoadedEvent += (IronSourceAdInfo info) =>
            {
                if (showDebug) Debug.Log("load banner ads complete");
                available = true;
                CollapsibleBannerFlow.Instance.OnNormalBannerAdsLoaded();
            };
            IronSourceBannerEvents.onAdLoadFailedEvent += (error) =>
            {
                if (showDebug)
                {
                    Debug.LogError("load banner ads failed");
                    Debug.LogError(error.getDescription());
                }
                retryCount++;
                float time = Mathf.Pow(2, retryCount);
                if (time > 64) time = 64;
                Invoke(nameof(LoadAds), time);
                available = false;
            };
        }

        public override void LoadAds()
        {
            if (IronsourceManager.instance.showAds == false) return;
            if (showDebug) Debug.Log("calling load ads");
            IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, position);
        }

        public override bool ShowAds(Action onShowAdsComplete)
        {
            if (showDebug) Debug.Log("showing ironsource banner ads");
            IronSource.Agent.displayBanner();
            return available;
        }
        public void SetBannerVisible(bool visible)
        {
            if (showDebug) Debug.Log($"set ironsource banner visible {visible}");
            if (visible)
            {
                IronSource.Agent.displayBanner();
                isShowingAds = true;
                FirebaseManager.analytics.LogAdsBannerRecorded("ironsource", "bottom");
            } else
            {
                IronSource.Agent.hideBanner();
                isShowingAds = false;
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
}