using Firebase.Analytics;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    public void LogLevelStart(int level, bool restart)
    {
        if (FirebaseManager.isReady == false) return;
        var param1 = new Parameter("level", level);
        var param2 = new Parameter("restart", restart.ToString());
        FirebaseAnalytics.LogEvent("level_start", param1, param2);
    }
    public void LogLevelPassed(int level, float timeSpent)
    {
        if (FirebaseManager.isReady == false) return;
        var param1 = new Parameter("level", level);
        var param2 = new Parameter("time_spent", timeSpent);
        FirebaseAnalytics.LogEvent("level_passed", param1, param2);
    }
    public void LogLevelFailed(int level, float timeSpent)
    {
        if (FirebaseManager.isReady == false) return;
        var param1 = new Parameter("level", level);
        var param2 = new Parameter("time_spent", timeSpent);
        FirebaseAnalytics.LogEvent("level_failed", param1, param2);
    }
    public void LogWillShowInterstitial(bool internetAvailable, string placement, bool hasAds)
    {
        if (FirebaseManager.isReady == false) return;
        var param1 = new Parameter("internet_available", internetAvailable.ToString());
        var param2 = new Parameter("placement", placement);
        var param3 = new Parameter("has_ads", hasAds.ToString());
        FirebaseAnalytics.LogEvent("will_show_interstitial", param1, param2, param3);
    }
    public void LogWillShowRewarded(bool internetAvailable, string placement, bool hasAds)
    {
        if (FirebaseManager.isReady == false) return;
        var param1 = new Parameter("internet_available", internetAvailable.ToString());
        var param2 = new Parameter("placement", placement);
        var param3 = new Parameter("has_ads", hasAds.ToString());
        FirebaseAnalytics.LogEvent("will_show_rewarded", param1, param2, param3);
    }
    public void LogUIAppear(string screenName, string name)
    {
        if (FirebaseManager.isReady == false) return;
        var param1 = new Parameter("screen_name", screenName);
        var param2 = new Parameter("name", name);
        FirebaseAnalytics.LogEvent("ui_appear", param1, param2);
    }
    public void LogButtonClick(string screenName, string buttonName)
    {
        if (FirebaseManager.isReady == false) return;
        var param1 = new Parameter("screen_name", screenName);
        var param2 = new Parameter("name", buttonName);
        FirebaseAnalytics.LogEvent("button_click", param1, param2);
    }
    public void LogAdsAppOpenRecorded(string adPlatform, string placement)
    {
        if (FirebaseManager.isReady == false) return;
        var param1 = new Parameter("ad_platform", adPlatform);
        var param2 = new Parameter("placement", placement);
        FirebaseAnalytics.LogEvent("aoa_show_success", param1, param2);
    }
    public void LogAdsBannerRecorded(string adPlatform, string placement)
    {
        if (FirebaseManager.isReady == false) return;
        var param1 = new Parameter("ad_platform", adPlatform);
        var param2 = new Parameter("placement", placement);
        FirebaseAnalytics.LogEvent("banner_show_success", param1, param2);
    }
    public void LogAdsCollapsibleBannerRecorded(string adPlatform, string placement)
    {
        if (FirebaseManager.isReady == false) return;
        var param1 = new Parameter("ad_platform", adPlatform);
        var param2 = new Parameter("placement", placement);
        FirebaseAnalytics.LogEvent("collap_banner_show_success", param1, param2);
    }
    public void LogAdsIntertistialRecorded(string adPlatform, string placement)
    {
        if (FirebaseManager.isReady == false) return;
        var param1 = new Parameter("ad_platform", adPlatform);
        var param2 = new Parameter("placement", placement);
        FirebaseAnalytics.LogEvent("inter_show_success", param1, param2);
    }
    public void LogAdsRewardedRecorded(string adPlatform, string placement)
    {
        if (FirebaseManager.isReady == false) return;
        var param1 = new Parameter("ad_platform", adPlatform);
        var param2 = new Parameter("placement", placement);
        FirebaseAnalytics.LogEvent("rewarded_show_success", param1, param2);
    }
    public void SetUserLevelMax(int level)
    {
        if (FirebaseManager.isReady == false) return;
        FirebaseAnalytics.SetUserProperty("level_max", level.ToString());
    }
    public void SetUserLastLevel(int level)
    {
        if (FirebaseManager.isReady == false) return;
        FirebaseAnalytics.SetUserProperty("last_level", level.ToString());
    }
    public void SetUserLastPlacement(string placement)
    {
        if (FirebaseManager.isReady == false) return;
        FirebaseAnalytics.SetUserProperty("last_placement", placement);
    }
    public void SetUserTotalInterstitialAds(int total)
    {
        if (FirebaseManager.isReady == false) return;
        FirebaseAnalytics.SetUserProperty("total_interstitial_ads", total.ToString());
    }
    public void SetUserTotalRewardedAds(int total)
    {
        if (FirebaseManager.isReady == false) return;
        FirebaseAnalytics.SetUserProperty("total_rewarded_ads", total.ToString());
    }
}