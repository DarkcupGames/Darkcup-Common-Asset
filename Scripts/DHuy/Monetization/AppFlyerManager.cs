using AppsFlyerSDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppFlyerManager : MonoBehaviour
{
    public static AppFlyerManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LogTutorialCompletion(bool success)
    {
        Dictionary<string, string> eventValues = new Dictionary<string, string>();
        eventValues.Add(AFInAppEvents.SUCCESS, success.ToString());
        AppsFlyer.sendEvent(AFInAppEvents.TUTORIAL_COMPLETION, eventValues);
    }

    public void LogLevelPass(int level)
    {
        Dictionary<string, string> eventValues = new Dictionary<string, string>();
        eventValues.Add(AFInAppEvents.LEVEL, level.ToString());
        AppsFlyer.sendEvent(AFInAppEvents.LEVEL_ACHIEVED, eventValues);
    }

    //inter
    public void LogInterIngameLogicCall()
    {
        Dictionary<string, string> eventValues = new Dictionary<string, string>();
        AppsFlyer.sendEvent("af_inters_ad_eligible", eventValues);
    }

    public void LogInterReady()
    {
        Dictionary<string, string> eventValues = new Dictionary<string, string>();
        AppsFlyer.sendEvent("af_inters_api_called", eventValues);
    }

    public void LogInterShow()
    {
        Dictionary<string, string> eventValues = new Dictionary<string, string>();
        AppsFlyer.sendEvent("af_inters_displayed", eventValues);
    }

    //reward
    public void LogRewardedIngameLogicCall()
    {
        Dictionary<string, string> eventValues = new Dictionary<string, string>();
        AppsFlyer.sendEvent("af_rewarded_ad_eligible", eventValues);
    }

    public void LogRewardedReady()
    {
        Dictionary<string, string> eventValues = new Dictionary<string, string>();
        AppsFlyer.sendEvent("af_rewarded_api_called", eventValues);
    }

    public void LogRewardedShow()
    {
        Dictionary<string, string> eventValues = new Dictionary<string, string>();
        AppsFlyer.sendEvent("af_rewarded_displayed", eventValues);
    }

    public void LogRewardedComplete()
    {
        Dictionary<string, string> eventValues = new Dictionary<string, string>();
        AppsFlyer.sendEvent("af_rewarded_ad_completed", eventValues);
    }
}
