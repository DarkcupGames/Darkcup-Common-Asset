using DarkcupGames;
using DG.Tweening;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;

namespace DarkcupGames
{
    public class CollapsibleBannerFlow : MonoBehaviour
    {
        public static CollapsibleBannerFlow Instance;
        [SerializeField] private AdmobAdBanner admobBanner;
        [SerializeField] private IronsourceBanner ironsourceBanner;
        [SerializeField] private bool showDebug = true;

        private float lastShowCollapsible = 0;
        private float lastChangeUUID = 0;

        private void Awake()
        {
            Instance = this;
        }

        public void OnCollapsibleAdsLoaded()
        {
            if (FirebaseManager.remoteConfig.COLLAPSIBLE_BANNER_ENABLED)
            {
                admobBanner.SetBannerVisible(true);
                ironsourceBanner.SetBannerVisible(false);
            }
        }

        public void OnCollapsibleAdsFailed()
        {
            if (FirebaseManager.remoteConfig.COLLAPSIBLE_BANNER_ENABLED &&
                FirebaseManager.remoteConfig.COLLAPSIBLE_FALLBACK_ENABLED && 
                ironsourceBanner.IsAdsAvailable())
            {
                admobBanner.SetBannerVisible(false);
                ironsourceBanner.SetBannerVisible(true);
            }
        }

        public void OnNormalBannerAdsLoaded()
        {
            if (FirebaseManager.remoteConfig.COLLAPSIBLE_BANNER_ENABLED == false)
            {
                admobBanner.SetBannerVisible(false);
                ironsourceBanner.SetBannerVisible(true);
                return;
            }
            
            if (FirebaseManager.remoteConfig.COLLAPSIBLE_BANNER_ENABLED == true &&
                FirebaseManager.remoteConfig.COLLAPSIBLE_FALLBACK_ENABLED == true &&
                admobBanner.IsAdsAvailable() == false)
            {
                admobBanner.SetBannerVisible(false);
                ironsourceBanner.SetBannerVisible(true);
            }
        }

        private void Update()
        {
            if (Time.time > lastChangeUUID)
            {
                if (showDebug) Debug.Log($"load new uuid collapsible");
                admobBanner.GenerateNewUUID();
                admobBanner.LoadAds();
                lastChangeUUID = Time.time + FirebaseManager.remoteConfig.COLLAPSIBLE_BANNER_INTERVAL;
            }
        }
    }
}