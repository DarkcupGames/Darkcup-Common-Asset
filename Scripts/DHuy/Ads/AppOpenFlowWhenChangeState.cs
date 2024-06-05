using DarkcupGames;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;

public class AppOpenFlowWhenChangeState : MonoBehaviour
{
    public const float BACKGROUND_TIME_SHOW_APP_OPEN = 10f;

    [SerializeField] private bool showDebug;

    public AdmobAppOpen appOpen;
    public IronsourceIntertistial intertistial;
    public IronsourceReward rewarded;
    private float gotoBackgroundTime;

    private void Awake()
    {
        AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
    }

    private void OnAppStateChanged(AppState state)
    {
        if (showDebug) Debug.Log("app state changed, app state = " + state);
        if (intertistial.IsShowingAds()) return;
        if (rewarded.IsShowingAds()) return;

        if (state == AppState.Background)
        {
            if (showDebug) Debug.Log("saving background time = " + Time.time);
            gotoBackgroundTime = Time.realtimeSinceStartup;
        }

        if (state == AppState.Foreground)
        {
            if (showDebug) Debug.Log("try showing app open, sleep time = " + (Time.realtimeSinceStartup - gotoBackgroundTime));
            if (Time.realtimeSinceStartup - gotoBackgroundTime > BACKGROUND_TIME_SHOW_APP_OPEN)
            {
                if (showDebug) Debug.Log("showing app open, available = " + appOpen.IsAdsAvailable());
                if (appOpen.IsAdsAvailable())
                {
                    appOpen.ShowAds(null);
                    FirebaseManager.analytics.LogAdsAppOpenRecorded("admob", "background_resume");
                }
            }
        }
    }
}