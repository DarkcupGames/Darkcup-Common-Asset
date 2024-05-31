using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarkcupGames
{
    [RequireComponent(typeof(MainThreadScriptRunner))]
    public class IronsourceReward : IronsourceAds
    {
        public float LOAD_TIME_CAPPING = 5;
        private MainThreadScriptRunner mainThread;
        private Action onShowAdsComplete;
        private float lastLoadAds;
        private bool isShowingAds;

        private void Awake()
        {
            mainThread = GetComponent<MainThreadScriptRunner>();
        }

        public override void Init()
        {
            IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
            IronSourceRewardedVideoEvents.onAdLoadFailedEvent += RewardedVideoAdFailed;
            IronSourceRewardedVideoEvents.onAdClosedEvent += (info)=> {
                isShowingAds = false;
            };
        }

        private void RewardedVideoAdFailed(IronSourceError error)
        {
            if (showDebug)
            {
                Debug.LogError("rewarded ads failed");
                Debug.LogError(error.getDescription());
            }
        }

        private void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo info)
        {
            mainThread.Run(onShowAdsComplete);
        }

        public override void LoadAds()
        {
            IronSource.Agent.loadRewardedVideo();
        }

        public override bool ShowAds(Action onShowAdsComplete)
        {
            this.onShowAdsComplete = onShowAdsComplete;
            if (showDebug) Debug.Log("calling show ads rewarded");
            if (IronSource.Agent.isRewardedVideoAvailable() == false)
            {
                if (showDebug) Debug.Log("rewarded ready fail");
                onShowAdsComplete?.Invoke();
                if (Time.time - lastLoadAds > LOAD_TIME_CAPPING)
                {
                    if (showDebug) Debug.Log("load rewaded ads");
                    LoadAds();
                    lastLoadAds = Time.time;
                }
                isShowingAds = false;
                return false;
            }
            if (showDebug) Debug.Log("ready = true, show rewarded ads");
            IronSource.Agent.showRewardedVideo();
            isShowingAds = true;
            return true;
        }

        public override bool IsAdsAvailable()
        {
            return IronSource.Agent.isRewardedVideoAvailable();
        }

        public override bool IsShowingAds()
        {
            return isShowingAds;
        }
    }
}