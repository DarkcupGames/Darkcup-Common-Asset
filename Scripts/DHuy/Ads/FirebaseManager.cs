using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
//using Firebase.RemoteConfig;
using System.Threading.Tasks;
using System;
using TMPro;
using Newtonsoft.Json;
using Firebase;
//using Firebase.Database;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using DarkcupGames;

public enum AdsType
{
    Banner, Inter, Reward, AppOpen
}

public enum AdsAction
{
    Offer, Click, Show, Fail
}

public enum AdsErrorMessage
{
    Unknown, Offline, NoFill, InternalError, InvalidRequest, UnableToPrecached, FailToLoad, Unavailable
}

public enum FirebaseQuitReasonType
{
    Loading, BoringUI, BoringGameplay, IntertistialAds, RewardedAds, AppOpenAds
}

[System.Serializable]
public class FirebaseQuitReason
{
    public string type;
    public float sessionTime;
    public int level;
    public string detail;
    public int loseCount;
}

[System.Serializable]
public class FirebaseUserRevenue
{
    public int interSuccess;
    public int interFailed;
    public double interRevenue;

    public int rewardSuccess;
    public int rewardFailed;
    public double rewardRevenue;

    public int appOpenSuccess;
    public int appOpenFailed;
    public double appOpenRevenue;

    public int bannerSuccess;
    public int bannerFailed;
    public double bannerRevenue;
}

public enum PlayerType
{
    New, Noob, Pro
}

public class PlayerWinLoseData
{
    public int winCount;
    public int loseCount;
    public int timeSpent;
}
public static class FirebaseConstants
{
    public const bool TEST_MODE = false;
    public const int DEAD_AMOUNT_TO_BE_NOOB = 5;
    public const int WIN_STREAK_TO_BE_PRO = 5;
    public const int MIN_SECONDS_ANALYTICS_RATE = 20;
    public const int MIN_LEVEL_TO_SHOW_ADS = 5;
    public const int MIN_SECOND_BETWEEN_INTERTISTIAL = 30;

    public const int MAX_ACTION_LOG = 200;
    public const int FIRST_AMOUNT_LEVEL_TO_SEND_EVENT = 10;
    public const string LEVEL_1_PLAYED = "Level 1 played";
    public const string AFTER_LOADING_HOME = "Loading done, Game scene";
    public const string FINISH_ALL_FIRST_10_LEVEL = "Finish all current levels";
}

public enum PlacementRewarded
{
    reward_skip_level, reward_extra_coin, reward_hint, reward_skin, reward_all, reward_bonus_level
}

public enum PlacementIntertistial
{
    inter_click_button_setting, inter_next_level, inter_click_button_back, inter_select_level_in_home, inter_all, lose_game, inter_click_button_replay, inter_level_button
}

public enum PlacementBanner
{
    banner_bottom
}

public enum RemoteConfigKey
{
    min_seconds_between_intertistial, min_seconds_analytics, min_level_to_show_ads
}

public enum UserProperty
{
    level, level_max, last_level, last_placement, total_interstitial_ads, total_rewarded_ads, retent_type, days_played
}

[System.Serializable]
public class FirebaseUserData
{
    public string id;
    public double startPlayDay;
    public double lastPlayDay;
    public int totalPlayDay;
    public float totalPlayTime;
    public int retentionType;
    public int lastInterShow;
    public int level = -1;
    public int maxLevel;
    public string language;
    public bool internetDisable;
    public string appVersion;
    public string originalAppVersion;
    public int sessionCount;
    public int exceptionCount;
    public string installerName;
    public int totalSkippedIntertistial;
    public int currentLevelPlayed;
    public PlayerType playerType = PlayerType.New;
    public List<string> actionChecker = new List<string>();
    public Dictionary<int, PlayerWinLoseData> winloseDatas = new Dictionary<int, PlayerWinLoseData>();
    public FirebaseUserRevenue revenue;
    public FirebaseQuitReason quitReason;
}

[System.Serializable]
public class FirebaseSessionData
{
    public string id;
    public string installer;
    public float totalTime;
    public int skippedInter;
    public float secondPerAds;
    public float secondPerInter;
    public string startTime;
    public float sessionTime;
    public int totalPlayDay;
    public int retentionType;
    public string language;
    public List<string> actions;
    public FirebaseQuitReason quitReason;
}

[System.Serializable]
public class FirebaseExceptionData
{
    public int sessionId;
    public string userId;
    public string detail;
    public string version;
    public string time;
    public string platform;
    public string deviceModel;
    public string deviceName;
    public int systemMemorySize;
    public float batteryLevel;
    public BatteryStatus batteryStatus;
    public int graphicsMemorySize;
}

public class FirebaseManager : MonoBehaviour
{
    public static string SLACK_WEBHOOK = "https://hooks.slack.com/services/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";
    public const string FIREBASE_USER_DATA_FILE_NAME = "firebase_userdata";

    public static FirebaseManager Instance;
    public static FirebaseUserData firebaseUserData;
    public static int minsSecondsBetweenIntertistial;
    public static int minLevelToShowAds;
    public static int minSecondAnalytics;
    public static bool enableSlackAnalytics = true;

    public static string requestResult;
    public static string logString;
    public static bool remoteConfigLoaded = false;
    public static bool runContinueOnMainThread = false;

    List<string> actions;
    //DatabaseReference databaseRef;
    bool isFirebaseReady;
    float lastAnalytics;
    float previousPlayTime;
    float lastStartPlayTime;
    string appVersion;
    string startTime;
    int loseCount;

    //track watch ads time
    float startTimeAppOpen;
    float startTimeIntertistial;
    float startTimeRewarded;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            actions = new List<string>();
            startTime = DateTime.UtcNow.ToString();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadUserData();

        minSecondAnalytics = FirebaseConstants.MIN_SECONDS_ANALYTICS_RATE;
        minLevelToShowAds = FirebaseConstants.MIN_LEVEL_TO_SHOW_ADS;
        minsSecondsBetweenIntertistial = FirebaseConstants.MIN_SECOND_BETWEEN_INTERTISTIAL;
        enableSlackAnalytics = true;

        //Firebase.FirebaseApp.LogLevel = LogLevel.Verbose;
        appVersion = Application.version.Replace(".", "_");
        Application.logMessageReceived += LogException;
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            Debug.Log("check dependency complete, result = " + task.Result);
            Debug.Log("firebase defulat database instance = ");
            //Debug.Log(FirebaseDatabase.DefaultInstance);
            Debug.Log("firebase root reference = ");
            //Debug.Log(FirebaseDatabase.DefaultInstance.RootReference);

            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                if (string.IsNullOrEmpty(firebaseUserData.originalAppVersion))
                {
                    firebaseUserData.originalAppVersion = Application.version;
                }
                //clear data when update
                if (firebaseUserData.originalAppVersion != Application.version)
                {
                    string originalVersion = firebaseUserData.originalAppVersion;
                    firebaseUserData = new FirebaseUserData();
                    firebaseUserData.originalAppVersion = originalVersion;
                    firebaseUserData.revenue = new FirebaseUserRevenue();
                    firebaseUserData.startPlayDay = new TimeSpan(DateTime.UtcNow.Ticks).TotalDays;
                }
                firebaseUserData.appVersion = Application.version;
                firebaseUserData.language = Application.systemLanguage.ToString();
                firebaseUserData.sessionCount++;
                firebaseUserData.installerName = Application.installerName;
                if (firebaseUserData.revenue == null) firebaseUserData.revenue = new FirebaseUserRevenue();
                if (firebaseUserData.winloseDatas == null) firebaseUserData.winloseDatas = new Dictionary<int, PlayerWinLoseData>();
                if (firebaseUserData.actionChecker == null) firebaseUserData.actionChecker = new List<string>();
                firebaseUserData.quitReason = new FirebaseQuitReason();
                firebaseUserData.quitReason.type = FirebaseQuitReasonType.Loading.ToString();
                previousPlayTime = firebaseUserData.totalPlayTime;

                Debug.Log("finish firebase user data");
                isFirebaseReady = true;
                Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
                Debug.Log("get default insstance firebase app done");
                //databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("get database done!");
                //Debug.Log(databaseRef);

                TrackRetention();
                AnalyticsUserProperty();
                SetUpRemoteConfig();
                SaveLocal();
                SaveServer();
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }

    private void Update()
    {
        if (firebaseUserData == null) return;
        if (Time.realtimeSinceStartup - lastAnalytics > minSecondAnalytics)
        {
            Debug.Log($"Save data to server with id = {firebaseUserData.id}, app version = {Application.version}");
            lastAnalytics = Time.realtimeSinceStartup;
            firebaseUserData.totalPlayTime = previousPlayTime + Time.realtimeSinceStartup;
            SaveLocal();
            SaveServer();
            SaveActions();
        }
    }

    #region LOG_LEVEL_STATUS
    public void LogLevelStart(int level, bool isRestart)
    {
        if (isRestart) actions.Add("restart level " + level);
        else actions.Add("play level " + level);
        if (isFirebaseReady == false) return;
        var param1 = new Parameter("level", level);
        var param2 = new Parameter("restart", isRestart ? "true" : "false");
        FirebaseAnalytics.LogEvent("level_start", param1, param2);
        lastStartPlayTime = Time.time;
        firebaseUserData.level = level;
    }

    public void LogLevelPassed(int level)
    {
        float timeSpent = Time.time - lastStartPlayTime;
        actions.Add("pass level " + level + " time = " + (int)(timeSpent));
        if (isFirebaseReady == false) return;
        var param1 = new Parameter("level", level);
        var param2 = new Parameter("time_spent", timeSpent);
        FirebaseAnalytics.LogEvent("level_complete", param1, param2);
        CountWinLose(level, true, timeSpent);
        loseCount = 0;
        LogCheckPoint("pass_level_" + level);
    }

    public void LogLevelFail(int level)
    {
        float timeSpent = Time.time - lastStartPlayTime;
        actions.Add("fail level " + level + " time = " + (int)(timeSpent));
        if (isFirebaseReady == false) return;
        var param1 = new Parameter("level", level);
        var param2 = new Parameter("time_spent", timeSpent);
        var param3 = new Parameter("failcount", loseCount);
        loseCount++;
        FirebaseAnalytics.LogEvent("level_fail", param1, param2, param3);
        CountWinLose(level, false, timeSpent);
    }

    public void CountWinLose(int level, bool isWin, float timeSpent)
    {
        Debug.Log($"this is count win lose level = {level}, isWin = {isWin}, timeSpent = {timeSpent}");
        Debug.Log($"firebaseUserData = {firebaseUserData}, firebaseUserData is null = {firebaseUserData == null}");
        if (firebaseUserData.winloseDatas == null)
        {
            firebaseUserData.winloseDatas = new Dictionary<int, PlayerWinLoseData>();
        }
        if (firebaseUserData.winloseDatas.ContainsKey(level) == false)
        {
            firebaseUserData.winloseDatas.Add(level, new PlayerWinLoseData());
        }
        Debug.Log($"count win lose");
        if (isWin) firebaseUserData.winloseDatas[level].winCount++;
        else firebaseUserData.winloseDatas[level].loseCount++;
        Debug.Log($"count time spent");
        firebaseUserData.winloseDatas[level].timeSpent = (int)timeSpent;
        SaveLocal();
    }
    #endregion

    #region LOG_ADS
    public void LogIntertistialFailed()
    {
        bool no_internet = Application.internetReachability == NetworkReachability.NotReachable;
        actions.Add($"failed inter (internet = {!no_internet})");
        if (firebaseUserData.revenue == null) firebaseUserData.revenue = new FirebaseUserRevenue();
        firebaseUserData.revenue.interFailed++;
        SaveLocal();

        if (isFirebaseReady == false) return;
        string error = AdsErrorMessage.NoFill.ToString();
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            error = AdsErrorMessage.Offline.ToString();
        }
        var param1 = new Parameter("errormsg", error.ToString());
        FirebaseAnalytics.LogEvent("ad_inter_fail", param1);
    }

    public void LogIntertistialStart()
    {
        startTimeIntertistial = Time.realtimeSinceStartup;
        LogQuitReason(FirebaseQuitReasonType.IntertistialAds);
    }

    public void LogIntertistialSuccess(double revenue)
    {
        //int watchTime = (int)(Time.realtimeSinceStartup - startTimeIntertistial);
        actions.Add("success inter revenue = " + Math.Round(revenue, 4));
        if (firebaseUserData.revenue == null) firebaseUserData.revenue = new FirebaseUserRevenue();
        firebaseUserData.revenue.interSuccess++;
        firebaseUserData.revenue.interRevenue += revenue;
        SaveLocal();

        if (isFirebaseReady == false) return;
        FirebaseAnalytics.LogEvent("ad_inter_show");
    }

    public void LogRewardedFailed()
    {
        bool no_internet = Application.internetReachability == NetworkReachability.NotReachable;
        if (firebaseUserData.revenue == null) firebaseUserData.revenue = new FirebaseUserRevenue();
        firebaseUserData.revenue.rewardFailed++;
        SaveLocal();

        if (isFirebaseReady == false) return;
        string error = AdsErrorMessage.NoFill.ToString();
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            error = AdsErrorMessage.Offline.ToString();
        }
        var param1 = new Parameter("errormsg", error.ToString());
        FirebaseAnalytics.LogEvent("ads_reward_fail", param1);
    }

    public void LogRewardedStart()
    {
        startTimeRewarded = Time.realtimeSinceStartup;
        LogQuitReason(FirebaseQuitReasonType.RewardedAds);
    }

    public void LogRewardedSuccess(double revenue)
    {
        //int watchTime = (int)(Time.realtimeSinceStartup - startTimeRewarded);
        actions.Add("success reward revenue = " + Math.Round(revenue, 4));
        if (firebaseUserData.revenue == null) firebaseUserData.revenue = new FirebaseUserRevenue();
        firebaseUserData.revenue.rewardSuccess++;
        firebaseUserData.revenue.rewardRevenue += revenue;
        SaveLocal();

        if (isFirebaseReady == false) return;
        FirebaseAnalytics.LogEvent("ads_reward_show");
    }

    public void LogShowBanner(double revenue)
    {
        if (firebaseUserData.revenue == null) firebaseUserData.revenue = new FirebaseUserRevenue();
        firebaseUserData.revenue.bannerSuccess++;
        firebaseUserData.revenue.bannerRevenue += revenue;
        SaveLocal();

        if (isFirebaseReady == false) return;
        FirebaseAnalytics.LogEvent("show_banner_ads");
    }

    public void LogAppOpenStart()
    {
        startTimeAppOpen = Time.realtimeSinceStartup;
        LogQuitReason(FirebaseQuitReasonType.AppOpenAds);
    }

    public void LogAppOpenSuccess(double revenue)
    {
        int watchTime = (int)(Time.realtimeSinceStartup - startTimeAppOpen);
        if (firebaseUserData.revenue == null) firebaseUserData.revenue = new FirebaseUserRevenue();
        firebaseUserData.revenue.appOpenSuccess++;
        firebaseUserData.revenue.appOpenRevenue += revenue;
        SaveLocal();

        if (isFirebaseReady == false) return;
        FirebaseAnalytics.LogEvent("show_app_open_ads");
        actions.Add($"show app open revenue = " + Math.Round(revenue, 4) + " time = " + watchTime);
    }
    #endregion

    #region OTHER_LOGS
    public void LogPopupAppear(string popupName)
    {
        if (isFirebaseReady == false) return;
        var param1 = new Parameter("screen_name", SceneManager.GetActiveScene().name);
        var param2 = new Parameter("name", popupName);
        FirebaseAnalytics.LogEvent("ui_appear", param1, param2);
        actions.Add("popup " + popupName);
    }

    public void LogButtonClick(string buttonName)
    {
        if (isFirebaseReady == false) return;
        var param1 = new Parameter("screen_name", SceneManager.GetActiveScene().name);
        var param2 = new Parameter("name", buttonName);
        FirebaseAnalytics.LogEvent("button_click", param1, param2);
        actions.Add("button " + buttonName);
    }

    public void LogAction(string action)
    {
        if (isFirebaseReady == false) return;
        actions.Add(action);
    }

    public void LogEvent(string e)
    {
        if (isFirebaseReady == false) return;
        FirebaseAnalytics.LogEvent(e);
    }

    public void LogCheckPoint(string checkPoint)
    {
        if (isFirebaseReady == false) return;
        string key = "CHECKPOINT_" + checkPoint;
        if (PlayerPrefs.HasKey(key) == true) return;
        FirebaseAnalytics.LogEvent("checkpoint_" + checkPoint);
    }

    public void LogEarnMoney(string currency_name, long amount, string source)
    {
        if (isFirebaseReady == false) return;
        var param1 = new Parameter("virtual_currency_name", currency_name);
        var param2 = new Parameter("value", amount);
        var param3 = new Parameter("source", source);
        FirebaseAnalytics.LogEvent("earn_virtual_currency", param1, param2, param3);
    }

    public void LogSpendMoney(string currency_name, long amount, string item_name)
    {
        if (isFirebaseReady == false) return;
        var param1 = new Parameter("virtual_currency_name", currency_name);
        var param2 = new Parameter("value", amount);
        var param3 = new Parameter("item_name", item_name);
        FirebaseAnalytics.LogEvent("earn_virtual_currency", param1, param2, param3);
    }

    public void LogQuitReason(FirebaseQuitReasonType type, string detail = null)
    {
        if (!isFirebaseReady) return;
        FirebaseQuitReason reason = new FirebaseQuitReason();
        reason.type = type.ToString();
        reason.level = firebaseUserData.level;
        reason.sessionTime = Time.realtimeSinceStartup;
        reason.loseCount = loseCount;
        if (detail != null) reason.detail = detail;
        firebaseUserData.quitReason = reason;
        SaveLocal();
        SaveServer();
        SaveActions();
    }

    public void LogException(string condition, string stackTrace, LogType type)
    {
        if (isFirebaseReady == false) return;
        if (type == LogType.Exception)
        {
            actions.Add("exception " + condition + " " + stackTrace);
            FirebaseExceptionData exception = new FirebaseExceptionData();
            exception.sessionId = firebaseUserData.sessionCount;
            exception.version = Application.version;
            exception.userId = firebaseUserData.id;
            exception.detail = condition + " " + stackTrace;
            exception.time = DateTime.Now.ToString();
            exception.platform = Application.platform.ToString();
            exception.deviceModel = SystemInfo.deviceModel;
            exception.deviceName = SystemInfo.deviceName;
            exception.systemMemorySize = SystemInfo.systemMemorySize;
            exception.batteryLevel = SystemInfo.batteryLevel;
            exception.batteryStatus = SystemInfo.batteryStatus;
            exception.graphicsMemorySize = SystemInfo.graphicsMemorySize;

            //save server
            string json = JsonConvert.SerializeObject(exception);
            string key = Regex.Replace(condition, "[^0-9A-Za-z _-]", "");
            //databaseRef.Child("exception").Child(appVersion).Child(key).Child(GetRandomString(4)).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
            //{
            //    Debug.Log("Run finish, result = " + task.Status);
            //    Debug.Log("Exception = " + task.Exception);
            //});
            //SendSlackMessage(json);
        }
    }
    #endregion

    #region USER PROPERTY AND RETENTION
    public void SetUserProperty(UserProperty userProperty, string value)
    {
        if (isFirebaseReady == false) return;
        FirebaseAnalytics.SetUserProperty(userProperty.ToString(), value);
    }

    public void AnalyticsUserProperty()
    {
        //SetUserProperty(UserProperty.level_max, (GameSystem.userdata.maxLevel + 1).ToString());
        //SetUserProperty(UserProperty.last_level, (GameSystem.userdata.maxLevel + 1).ToString());
        //if (GameSystem.userdata != null)
        //{
        //    firebaseUserData.level = GameSystem.userdata.level;
        //    firebaseUserData.maxLevel = GameSystem.userdata.maxLevel;
        //    SaveLocal();
        //    SetUserProperty(UserProperty.level, (GameSystem.userdata.level).ToString());
        //}
        //SetUserProperty(UserProperty.retent_type, (firebaseUserData.retentionType).ToString());
        //SetUserProperty(UserProperty.days_played, (firebaseUserData.totalPlayDay).ToString()); 
    }

    public void TrackRetention()
    {
        double today = new TimeSpan(DateTime.Now.Ticks).TotalDays;
        if (firebaseUserData.startPlayDay == 0)
        {
            firebaseUserData.startPlayDay = today;
        }
        string lastPlayDay = PlayerPrefs.GetString("last_play_date", "");
        if (lastPlayDay == "") lastPlayDay = DateTime.Now.ToString("yyMMdd");
        string todayCode = DateTime.Now.ToString("yyMMdd");

        if (todayCode != lastPlayDay)
        {
            lastPlayDay = todayCode;
            PlayerPrefs.SetString("last_play_date", lastPlayDay);
            firebaseUserData.totalPlayDay++;
        }
        //if (firebaseUserData.lastPlayDay - today > 1)
        //{
        //    firebaseUserData.lastPlayDay = today;
        //    firebaseUserData.totalPlayDay++;
        //}
        firebaseUserData.retentionType = (int)(today - firebaseUserData.startPlayDay);
        SaveLocal();
    }
    #endregion

    #region REMOTE_CONFIG
    public void SetUpRemoteConfig()
    {
        //if (FirebaseConstants.TEST_MODE)
        //{
        //    ConfigSettings setting = FirebaseRemoteConfig.DefaultInstance.ConfigSettings;
        //    setting.MinimumFetchInternalInMilliseconds = 0;
        //}

        //Dictionary<string, object> defaults = new Dictionary<string, object>();

        //defaults.Add(RemoteConfigKey.min_level_to_show_ads.ToString(), FirebaseConstants.MIN_LEVEL_TO_SHOW_ADS);
        //defaults.Add(RemoteConfigKey.min_seconds_analytics.ToString(), FirebaseConstants.MIN_SECONDS_ANALYTICS_RATE);
        //defaults.Add(RemoteConfigKey.min_seconds_between_intertistial.ToString(), FirebaseConstants.MIN_SECOND_BETWEEN_INTERTISTIAL);

        //FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults).ContinueWithOnMainThread(task =>
        //{
        //    if (task.IsCompleted)
        //    {
        //        FetchDataAsync();
        //    }
        //    else
        //    {
        //        Debug.LogError("Failed to set default value of remote config");
        //    }
        //});
    }

    // Start a fetch request.
    // FetchAsync only fetches new data if the current data is older than the provided
    // timespan.  Otherwise it assumes the data is "recent enough", and does nothing.
    // By default the timespan is 12 hours, and for production apps, this is a good
    // number. For this example though, it's set to a timespan of zero, so that
    // changes in the console will always show up immediately.
    //public Task FetchDataAsync()
    //{
    //    Debug.Log("Fetching data...");
    //    Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(); //TimeSpan.Zero
    //    return fetchTask.ContinueWithOnMainThread(FetchComplete);
    //}

    private void FetchComplete(Task fetchTask)
    {
        //if (!fetchTask.IsCompleted)
        //{
        //    Debug.LogError("Retrieval hasn't finished.");
        //    return;
        //}

        //var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
        //var info = remoteConfig.Info;
        //if (info.LastFetchStatus != LastFetchStatus.Success)
        //{
        //    Debug.LogError($"{nameof(FetchComplete)} was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
        //    return;
        //}

        //// Fetch successful. Parameter values must be activated to use.
        //remoteConfig.ActivateAsync().ContinueWithOnMainThread(task => {
        //    remoteConfigLoaded = true;
        //    runContinueOnMainThread = true;
        //    Debug.Log($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}.");
        //    minsSecondsBetweenIntertistial = (int)remoteConfig.GetValue(RemoteConfigKey.min_seconds_between_intertistial.ToString()).LongValue;
        //    minSecondAnalytics = (int)remoteConfig.GetValue(RemoteConfigKey.min_seconds_analytics.ToString()).LongValue;
        //    minLevelToShowAds = (int)remoteConfig.GetValue(RemoteConfigKey.min_level_to_show_ads.ToString()).LongValue;

        //    logString = $"success, minSecondsAnalytics = {remoteConfig.GetValue(RemoteConfigKey.min_seconds_analytics.ToString()).LongValue}";
        //    Debug.Log($"Get value minSecondAnalytics = {minSecondAnalytics}");
        //    Debug.Log($"Get value enableSlackAnalytics = {enableSlackAnalytics}");
        //    if (minSecondAnalytics == 0) minSecondAnalytics = FirebaseConstants.MIN_SECONDS_ANALYTICS_RATE;
        //});
    }
    #endregion

    #region SAVE_LOAD_DATA
    public void SaveLocal()
    {
        //save local
        string json = JsonConvert.SerializeObject(firebaseUserData);
        string path = FileUtilities.GetWritablePath(FIREBASE_USER_DATA_FILE_NAME);
        FileUtilities.SaveFile(System.Text.Encoding.UTF8.GetBytes(json), path, true);
    }

    public void SaveServer()
    {
        //save server
        if (firebaseUserData == null || firebaseUserData.id == null || firebaseUserData.id == "") CreateNewUserData();

        string json = JsonConvert.SerializeObject(firebaseUserData);
        Debug.Log("save to server with json = " + json);

        //databaseRef.Child("userdata").Child(appVersion).Child(firebaseUserData.id).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        //{
        //    Debug.Log("Run finish, result = " + task.Status);
        //    Debug.Log("Exception = " + task.Exception);
        //});
    }

    public void SaveActions()
    {
        if (remoteConfigLoaded == false) return;
        if (enableSlackAnalytics == false) return;
        if (actions.Count > FirebaseConstants.MAX_ACTION_LOG)
        {
            List<string> newActions = new List<string>();
            newActions.Add("...");
            newActions.Add(actions[actions.Count - 2]);
            newActions.Add(actions[actions.Count - 1]);
            actions = newActions;
        }
        FirebaseSessionData session = new FirebaseSessionData();
        session.id = firebaseUserData.id;
        session.skippedInter = firebaseUserData.totalSkippedIntertistial;
        session.language = firebaseUserData.language;
        session.retentionType = firebaseUserData.retentionType;
        session.totalPlayDay = firebaseUserData.totalPlayDay;
        session.totalTime = firebaseUserData.totalPlayTime;
        session.sessionTime = Time.time;
        session.startTime = startTime;
        session.actions = actions;
        session.installer = Application.installerName;
        session.quitReason = firebaseUserData.quitReason;

        //save server
        string json = JsonConvert.SerializeObject(session);
        //databaseRef.Child("session").Child(appVersion).Child(firebaseUserData.id).Child(firebaseUserData.sessionCount.ToString()).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        //{
        //    Debug.Log("Run finish, result = " + task.Status);
        //    Debug.Log("Exception = " + task.Exception);
        //});
        //SendSlackMessage(json);
    }

    public void LoadUserData()
    {
        if (!FileUtilities.IsFileExist(FIREBASE_USER_DATA_FILE_NAME))
        {
            CreateNewUserData();
        }
        else
        {
            firebaseUserData = FileUtilities.DeserializeObjectFromFile<FirebaseUserData>(FIREBASE_USER_DATA_FILE_NAME);
            if (firebaseUserData == null || firebaseUserData.id == null || firebaseUserData.id == "")
            {
                CreateNewUserData();
            }
        }
    }

    void CreateNewUserData()
    {
        firebaseUserData = new FirebaseUserData();
        firebaseUserData.id = GetRandomUserKey();
        firebaseUserData.startPlayDay = new TimeSpan(DateTime.UtcNow.Ticks).TotalDays;
        Debug.Log("Create firebase user data with id = " + firebaseUserData.id);
        SaveLocal();
    }

    const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";

    public string GetRandomString(int length)
    {
        string rand = "";
        for (int i = 0; i < length; i++)
        {
            rand += alphabet[UnityEngine.Random.Range(0, alphabet.Length)];
        }
        return rand;
    }

    public string GetRandomUserKey()
    {
        string appVersion = Application.version.Replace(".", "_");
        string datetime = DateTime.Now.ToString("yyMMdd");
        string installerName = Application.installerName.Replace(".", "_");
        string key = datetime + "_" + appVersion + "_" + Application.systemLanguage + "_" + GetRandomString(4);
        if (installerName == "com.android.vending")
        {
            key += "_user";
        }
        return key;
    }

    public void SendSlackMessage(string message)
    {
        string content = "{\'text\':\'" + message + "\'}";
        SendRequest(SLACK_WEBHOOK, content, null);
    }

    public void SendRequest(string url, string json, Action doneAction, Action failAction = null)
    {
        StartCoroutine(SendRequestCoroutine(url, json, doneAction, failAction, System.Environment.StackTrace));
    }

    IEnumerator SendRequestCoroutine(string url, string json, Action doneAction, Action failAction, string stackTrace)
    {
        UnityWebRequest www = UnityWebRequest.Put(url, json);
        www.SetRequestHeader("Content-Type", "application/json");

        float startRequest = Time.time;

        yield return www.SendWebRequest();

        requestResult = www.downloadHandler.text;

        if (FirebaseConstants.TEST_MODE)
        {
            Debug.Log(url + " : " + requestResult);
        }

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError(requestResult);

            if (FirebaseConstants.TEST_MODE)
            {
                Debug.LogError("StackTrace detail " + stackTrace);
            }

            if (failAction != null)
            {
                failAction.Invoke();
            }
        }
        else
        {
            if (doneAction != null)
            {
                doneAction.Invoke();
            }

            try
            {
                Hashtable hash = JsonConvert.DeserializeObject<Hashtable>(requestResult);
                if (hash["result"].Equals("fail"))
                {
                    Debug.LogError(hash["detail"].ToString());

                    if (FirebaseConstants.TEST_MODE)
                    {
                        Debug.LogError("StackTrace detail " + stackTrace);
                    }
                }
            }
            catch { }
        }
        www.Dispose();
    }
    #endregion
}