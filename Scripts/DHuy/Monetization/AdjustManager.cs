using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.adjust.sdk;

public class AdjustManager : MonoBehaviour
{
    public static AdjustManager Instance;

#if !UNITY_IOS
    public const string EVENT_LEVEL_START = "o1638r";
    public const string EVENT_LEVEL_PASSED = "1dav0c";
    public const string EVENT_LEVEL_FAILED = "cd06v9";
#endif

#if UNITY_IOS
    public const string EVENT_LEVEL_START = "j7uyi4";
    public const string EVENT_LEVEL_PASSED = "yqnsu9";
    public const string EVENT_LEVEL_FAILED = "nssk5e";
#endif

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
#if UNITY_IOS
            CheckPermissionStatus();
#endif
#if !UNITY_IOS
            StartAdjust();
#endif
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void CheckPermissionStatus()
    {
        int permissionAsked = PlayerPrefs.GetInt("permissionAsked", 0);
        if (permissionAsked == 0)
        {
            PlayerPrefs.SetInt("permissionAsked", 1);
            Adjust.requestTrackingAuthorizationWithCompletionHandler((status) => {
                StartAdjust();
            });
            return;
        }
        StartAdjust();
    }

    private void StartAdjust()
    {
        //real android id
        string tokenId = "a6cfs6a36o74";

#if UNITY_IOS
        tokenId = "d9r9ad5nnkzk";
#endif
        if (Constants.TEST_MODE)
        {
            AdjustConfig adjustConfig = new AdjustConfig(tokenId, AdjustEnvironment.Sandbox);
            adjustConfig.setSendInBackground(true);
            Adjust.start(adjustConfig);
        }
        else
        {
            AdjustConfig adjustConfig = new AdjustConfig(tokenId, AdjustEnvironment.Production);
            adjustConfig.setSendInBackground(true);
            Adjust.start(adjustConfig);
        }
    }

    public void TrackEvent(string eventId)
    {
        AdjustEvent adjustEvent = new AdjustEvent(eventId);
        Adjust.trackEvent(adjustEvent);
    }
}