using TMPro;
using UnityEngine;
using DarkcupGames;

public class TextDebug : MonoBehaviour
{
    private TextMeshProUGUI txtDebug;
    public AdmobAdBanner admobBanner;
    public IronsourceBanner ironsourceBanner;
    public IronsourceIntertistial ironsourceInter;
    public IronsourceReward ironsourceRewarded;
    public RemoteConfigManager remoteConfig;

    private void Awake()
    {
        txtDebug = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        txtDebug.text = GetDebugText();
    }

    public string GetDebugText()
    {
        string str = "";
        str += $"admob availble = {admobBanner.IsAdsAvailable()} \n";
        str += $"admob is showing ads = {admobBanner.IsShowingAds()} \n";
        str += $"ironsource banner availble = {ironsourceBanner.IsAdsAvailable()} \n";
        str += $"ironsource banner is showing ads = {ironsourceBanner.IsShowingAds()} \n";

        str += $"collapsible_enabled = {remoteConfig.COLLAPSIBLE_BANNER_ENABLED} \n";
        str += $"fallback_enabled = {remoteConfig.COLLAPSIBLE_FALLBACK_ENABLED} \n";
        str += $"collasible_banner_interval = {remoteConfig.COLLAPSIBLE_BANNER_INTERVAL} \n";
        str += $"time_between_ads = {remoteConfig.TIME_BETWEEN_ADS} \n";

        str += $"ironsource inter availble = {ironsourceInter.IsAdsAvailable()} \n";
        str += $"ironsource inter is showing ads = {ironsourceInter.IsShowingAds()} \n";
        str += $"ironsource reward availble = {ironsourceRewarded.IsAdsAvailable()} \n";
        str += $"ironsource reward is showing ads = {ironsourceRewarded.IsShowingAds()} \n";

        return str;
    }
}