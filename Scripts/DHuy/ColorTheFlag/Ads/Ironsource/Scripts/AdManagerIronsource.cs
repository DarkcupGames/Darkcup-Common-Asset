using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DarkcupGames
{
    public class AdManagerIronsource : MonoBehaviour
    {
        public static AdManagerIronsource Instance;
        [SerializeField] private bool showAds = true;
        public List<UnityEvent> events;
        public List<string> rewardPlacementNames;

        float lastShowIntertistial;

        private void Awake()
        {
            Instance = this;
        }
        public void ShowIntertistial(Action onWatchAdsComplete, string placementName = "default")
        {
            if (showAds == false)
            {
                onWatchAdsComplete?.Invoke();
                return;
            }
            if (Time.time - lastShowIntertistial < FirebaseManager.remoteConfig.TIME_BETWEEN_ADS)
            {
                onWatchAdsComplete?.Invoke();
                return;
            }
#if UNITY_EDITOR
            onWatchAdsComplete?.Invoke();
            return;
#endif
            bool haveInternet = Application.internetReachability != NetworkReachability.NotReachable;
            bool haveAds = IronsourceManager.intertistial.IsAdsAvailable();
            FirebaseManager.analytics.LogWillShowInterstitial(haveInternet, placementName, haveAds);
            FirebaseManager.analytics.SetUserLastPlacement(placementName);

            bool success = IronsourceManager.intertistial.ShowAds(onWatchAdsComplete);
            if (success) lastShowIntertistial = Time.time;

            if (success)
            {
                GameSystem.userdata.totalIntertistial++;
                GameSystem.SaveUserDataToLocal();
                FirebaseManager.analytics.LogAdsIntertistialRecorded("ironsource", placementName);
                FirebaseManager.analytics.SetUserTotalInterstitialAds(GameSystem.userdata.totalIntertistial);
            }
        }
        public void ShowAds(int id)
        {
            var onWatchAdsFinished = events[id];
            if (showAds == false)
            {
                onWatchAdsFinished?.Invoke();
                return;
            }
#if UNITY_EDITOR
            onWatchAdsFinished?.Invoke();
            return;
#endif
            string placementName = "default";
            if (id < rewardPlacementNames.Count)
            {
                placementName = rewardPlacementNames[id];
            }
            bool haveInternet = Application.internetReachability != NetworkReachability.NotReachable;
            bool haveAds = IronsourceManager.rewarded.IsAdsAvailable();
            FirebaseManager.analytics.LogWillShowRewarded(haveInternet, placementName, haveAds);

            bool success = IronsourceManager.rewarded.ShowAds(() =>
            {
                onWatchAdsFinished?.Invoke();
            });

            if (success)
            {
                GameSystem.userdata.totalRewarded++;
                GameSystem.SaveUserDataToLocal();
                FirebaseManager.analytics.LogAdsRewardedRecorded("ironsource", placementName);
                FirebaseManager.analytics.SetUserTotalRewardedAds(GameSystem.userdata.totalRewarded);
            }
        }
    }
}