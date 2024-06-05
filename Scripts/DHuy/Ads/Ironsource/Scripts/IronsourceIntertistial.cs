using System;
using UnityEngine;

namespace DarkcupGames
{
    [RequireComponent(typeof(MainThreadScriptRunner))]
    public class IronsourceIntertistial : IronsourceAds
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
            if (showDebug) Debug.Log("calling init intertistial");
            IronSourceInterstitialEvents.onAdClosedEvent += InterstitialOnAdClosedEvent;
            IronSourceInterstitialEvents.onAdLoadFailedEvent += (error) =>
            {
                if (showDebug)
                {
                    Debug.LogError("load inter ads failed");
                    Debug.LogError(error.getDescription());
                }
            };
            IronSourceInterstitialEvents.onAdReadyEvent += (info) =>
            {
                Debug.LogError("ad inter is ready");
            };
        }

        private void InterstitialOnAdClosedEvent(IronSourceAdInfo info)
        {
            if (showDebug) Debug.Log("on intertistial close");
            mainThread.Run(onShowAdsComplete);
            LoadAds();
            isShowingAds = false;
        }

        public override void LoadAds()
        {
            if (showDebug) Debug.Log("calling load ads intertistial");
            IronSource.Agent.loadInterstitial();
        }

        public override bool ShowAds(Action onShowAdsComplete)
        {
            this.onShowAdsComplete = onShowAdsComplete;
            if (showDebug) Debug.Log("calling show ads intertistial");
            if (IronSource.Agent.isInterstitialReady() == false)
            {
                if (showDebug) Debug.Log("intertistial ready fail");
                onShowAdsComplete?.Invoke();
                if (Time.time - lastLoadAds > LOAD_TIME_CAPPING)
                {
                    if (showDebug) Debug.Log("load intertisital");
                    LoadAds();
                    lastLoadAds = Time.time;
                }
                isShowingAds = false;
                return false;
            }
            if (showDebug) Debug.Log("ready = true, show ads inter");
            IronSource.Agent.showInterstitial();
            isShowingAds = true;
            return true;
        }

        public override bool IsAdsAvailable()
        {
            return IronSource.Agent.isInterstitialReady();
        }

        public override bool IsShowingAds()
        {
            return isShowingAds;
        }
    }
}