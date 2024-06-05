using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using System.Collections.Generic;
using UnityEngine;


public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;
    public static bool isReady = false;
    [SerializeField] private bool showDebug;
    public static RemoteConfigManager remoteConfig;
    public static AnalyticsManager analytics;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            Debug.LogError("Only one FirebaseManager");
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        remoteConfig = GetComponentInChildren<RemoteConfigManager>();
        analytics = GetComponentInChildren<AnalyticsManager>();
    }
    private void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                var app = Firebase.FirebaseApp.DefaultInstance;
                isReady = true;
                remoteConfig.InitializeRemoteConfig();
            }
            else
            {
                DebugManager.Instance.AddDebug(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }
}
