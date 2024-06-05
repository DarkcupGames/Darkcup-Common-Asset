using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace DarkcupGames
{
    public class IronsourceManager : MonoBehaviour
    {
        public string APP_KEY;

        public static IronsourceManager instance;
        public static bool isReady = false;
        public bool showAds = true;
        public static IronsourceBanner banner { get; private set; }
        public static IronsourceIntertistial intertistial { get; private set; }
        public static IronsourceReward rewarded { get; private set; }

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }

        public void Init()
        {
            IronSource.Agent.validateIntegration();
            IronSourceEvents.onSdkInitializationCompletedEvent += () =>
            {
                isReady = true;
                var ads = GetComponentsInChildren<IronsourceAds>().ToList();
                for (int i = 0; i < ads.Count; i++)
                {
                    if (ads[i] is IronsourceBanner) banner = (IronsourceBanner)ads[i];
                    if (ads[i] is IronsourceIntertistial) intertistial = (IronsourceIntertistial)ads[i];
                    if (ads[i] is IronsourceReward) rewarded = (IronsourceReward)ads[i];

                    ads[i].Init();
                    ads[i].LoadAds();
                }
            };
            IronSourceEvents.onImpressionDataReadyEvent += ImpressionDataReadyEvent;
            IronSource.Agent.init(APP_KEY, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL, IronSourceAdUnits.BANNER);
        }

        private void ImpressionDataReadyEvent(IronSourceImpressionData impressionData)
        {
            LogFirebaseRevenue(impressionData);
            LogAppsflyerRevenue(impressionData);
        }

        private void LogFirebaseRevenue(IronSourceImpressionData impressionData)
        {
            if (impressionData != null)
            {
                Firebase.Analytics.Parameter[] AdParameters = {
                new Firebase.Analytics.Parameter("ad_platform", "ironSource"),
                new Firebase.Analytics.Parameter("ad_source", impressionData.adNetwork),
                new Firebase.Analytics.Parameter("ad_unit_name", impressionData.adUnit),
                new Firebase.Analytics.Parameter("ad_format", impressionData.instanceName),
                new Firebase.Analytics.Parameter("currency","USD"),
                new Firebase.Analytics.Parameter("value", impressionData.revenue.Value)
                };
                Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", AdParameters);
                Firebase.Analytics.FirebaseAnalytics.LogEvent("ironsource_custom_ad_impression", AdParameters);
            }
        }
        private void LogAppsflyerRevenue(IronSourceImpressionData impressionData)
        {
            if (impressionData != null)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("ad_format", impressionData.instanceName);
                AppsFlyerObjectScript.logAdRevenue(impressionData.adNetwork, impressionData.revenue.Value, parameters);
            }
        }
    }
}