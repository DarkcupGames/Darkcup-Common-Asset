//using Firebase.Analytics;
//using Newtonsoft.Json;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Networking;

//public class AnalyticManager : MonoBehaviour {
//    //public static string BASE_URL = "http://103.4.14.24";
//    //public static string URL_ANALYTICS_DATA = BASE_URL + "/save_analytics_data";
//    //public static string URL_ANALYTICS_EVENT = BASE_URL + "/save_analytics_event";

//    public static string SLACK_WEBHOOK = "https://hooks.slack.com/services/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";

//    public const string USER_ID = "analytic_id";
//    public const string TABLE_NAME = "table_name";

//    public const string AD_CLICK_COUNT = "ad_click_count";
//    public const string AD_CLICK_BANNER_COUNT = "banner_click_count";
//    public const string AD_CLICK_INTERSTITIAL_COUNT = "interstitial_click_count";
//    public const string AD_CLICK_REWARDED_COUNT = "reward_click_count";

//    public const string WATCH_AD_COUNT = "watch_ad_count";
//    public const string WATCH_INTERSTITIAL_COUNT = "watch_interstitial_count";
//    public const string WATCH_REWARDED_COUNT = "watch_reward_count";
//    public const string WATCH_REWARDED_COMPLETE = "watch_reward_complete";

//    public const string FIRST_PLAY_DATE = "first_play_date";
//    public const string PLAY_DAY_COUNT = "play_day_count";
//    public const string LAST_PLAY_DATE = "last_play_date";

//    public const string RATED = "rated";
//    public const string LAST_ASK_FOR_RATE = "last_ask_for_rate";
//    public const string CANCEL_RATE_COUNT = "cancel_rate_count";

//    public static bool DEBUG_MODE = true;

//    public static bool isFirebaseReady = false;

//    public static string result;
//    public static AnalyticManager Instance;

//    public string tableName = "watersort";

//    public List<string> events = new List<string>();

//    float nextAnalytics = 0f;

//    public static float MINUTE_TO_ANALYTICS = 0.5f;

//    private void Awake() {
//        if (Instance == null) {
//            DontDestroyOnLoad(gameObject);
//            Instance = this;

//            string lastPlayDate = PlayerPrefs.GetString(LAST_PLAY_DATE, "");

//            if (lastPlayDate == "") {
//                PlayerPrefs.SetString(USER_ID, GetRandomString());
//                PlayerPrefs.SetString(LAST_PLAY_DATE, DateTime.Now.ToString("yyyyMMdd"));
//                PlayerPrefs.SetString(FIRST_PLAY_DATE, DateTime.Now.ToString("yyyyMMdd"));
//                return;
//            }

//            if (lastPlayDate != DateTime.Now.ToString("yyyyMMdd")) {
//                int playDayCount = PlayerPrefs.GetInt(PLAY_DAY_COUNT, 1);
//                playDayCount++;
//                PlayerPrefs.SetInt(PLAY_DAY_COUNT, playDayCount);
//                PlayerPrefs.SetString(LAST_PLAY_DATE, DateTime.Now.ToString("yyyyMMdd"));
//            }
//        } else {
//            Destroy(gameObject);
//        }
//    }

//    public static void Log(string e) {
//        if (Instance.events.Count > 20) {
//            Instance.events = new List<string>();
//            Instance.events.Add("...");
//        }
//        Instance.events.Add(e);
//    }

//    public static bool ShouldAskForReview() {
//        int playDayCount = PlayerPrefs.GetInt(PLAY_DAY_COUNT, 1);
//        int rated = PlayerPrefs.GetInt(RATED, 0);

//        string lastAskForRate = PlayerPrefs.GetString(LAST_ASK_FOR_RATE, "");

//        if (rated == 1) return false;

//        if (lastAskForRate == DateTime.Now.ToString("yyyyMMdd")) { return false; }

//        int cancelCount = PlayerPrefs.GetInt(CANCEL_RATE_COUNT);
//        if (cancelCount >= 3) {
//            return false;
//        }

//        if (playDayCount >= 2 && Time.time > 120) {
//            return true;
//        }

//        if (playDayCount == 1 && Time.time > 30 * 60) {
//            return true;
//        }

//        return false;
//    }

//    public static void SetLastAskForReview() {
//        Log("ask rate");
//        PlayerPrefs.SetString(LAST_ASK_FOR_RATE, DateTime.Now.ToString("yyyyMMdd"));
//    }

//    public static void SetRateResult(bool acceptrate) { 
//        if (acceptrate) {
//            PlayerPrefs.SetInt(RATED, 1);
//        } else {
//            int cancelCount = PlayerPrefs.GetInt(CANCEL_RATE_COUNT);
//            cancelCount += 1;
//            PlayerPrefs.SetInt(CANCEL_RATE_COUNT, cancelCount);
//        }
//        PlayerPrefs.SetString(LAST_ASK_FOR_RATE, DateTime.Now.ToString("yyyyMMdd"));
//    }

//    public string GetPlayerString() {
//        return PlayerPrefs.GetInt(PLAY_DAY_COUNT) + "_" + Application.systemLanguage.ToString() + "_" + Application.version + "_" + PlayerPrefs.GetInt("currentLevel", 1) + "_" + Application.installerName;
//    }

//    public void SaveUser() {
//        Hashtable form = new Hashtable();
//        form.Add(USER_ID, PlayerPrefs.GetString(USER_ID));
//        form.Add(TABLE_NAME, tableName);

//        form.Add("time", DateTime.UtcNow.ToString());
//        form.Add("local_time", DateTime.Now.ToString());
//        form.Add("language", Application.systemLanguage.ToString());
//        form.Add("installer", Application.installerName);
//        form.Add("version", Application.version);
//        form.Add("genuine", Application.genuine.ToString());
//        form.Add("genuine_available", Application.genuineCheckAvailable.ToString());
//        form.Add("level", PlayerPrefs.GetInt("currentLevel", 1));

//        form.Add(AD_CLICK_COUNT, PlayerPrefs.GetInt(AD_CLICK_COUNT));

//        form.Add(AD_CLICK_BANNER_COUNT, PlayerPrefs.GetInt(AD_CLICK_BANNER_COUNT));
//        form.Add(AD_CLICK_INTERSTITIAL_COUNT, PlayerPrefs.GetInt(AD_CLICK_INTERSTITIAL_COUNT));
//        form.Add(AD_CLICK_REWARDED_COUNT, PlayerPrefs.GetInt(AD_CLICK_REWARDED_COUNT));

//        form.Add(WATCH_AD_COUNT, PlayerPrefs.GetInt(WATCH_AD_COUNT));
//        form.Add(WATCH_INTERSTITIAL_COUNT, PlayerPrefs.GetInt(WATCH_INTERSTITIAL_COUNT));
//        form.Add(WATCH_REWARDED_COUNT, PlayerPrefs.GetInt(WATCH_REWARDED_COUNT));
//        form.Add(WATCH_REWARDED_COMPLETE, PlayerPrefs.GetInt(WATCH_REWARDED_COMPLETE));

//        form.Add(FIRST_PLAY_DATE, PlayerPrefs.GetString(FIRST_PLAY_DATE));
//        form.Add(PLAY_DAY_COUNT, PlayerPrefs.GetInt(PLAY_DAY_COUNT));
//        form.Add(LAST_PLAY_DATE, PlayerPrefs.GetString(LAST_PLAY_DATE));

//        //SendRequest(URL_ANALYTICS_DATA, JsonConvert.SerializeObject(form), null);
//    }

//    public void SendAnalyticsToSlack() {
//        //Hashtable form = new Hashtable();
//        //form.Add(USER_ID, PlayerPrefs.GetString(USER_ID));
//        //form.Add(TABLE_NAME, tableName);

//        //form.Add("event", JsonConvert.SerializeObject(events));
//        //form.Add("play_time", Time.time);

//        string json = GetPlayerString() + "_" + JsonConvert.SerializeObject(events);
//        string content = "{\'text\':\'" + json + "\'}";

//        Debug.Log(content);

//        //SendRequest(URL_ANALYTICS_EVENT, JsonConvert.SerializeObject(form), null);
//        SendRequest(SLACK_WEBHOOK, content, null);
//    }

//    public void SendSlackMessage(string message) {
//        string content = "{\'text\':\'" + message + "\'}";
//        SendRequest(SLACK_WEBHOOK, content, null);
//    }

//    private void Start() {
//        SaveUser();
//        nextAnalytics = Time.time + MINUTE_TO_ANALYTICS * 60;
//    }

//    private void Update() {
//        //if (Time.time > nextAnalytics) {
//        //    nextAnalytics = Time.time + MINUTE_TO_ANALYTICS * 60;
//        //    SendAnalyticsToSlack();
//        //}
//    }

//    public string GetRandomString() {
//        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
//        var stringChars = new char[8];
//        var random = new System.Random();

//        for (int i = 0; i < stringChars.Length; i++) {
//            stringChars[i] = chars[random.Next(chars.Length)];
//        }

//        var finalString = new String(stringChars);
//        return finalString;
//    }

//    public void SendRequest(string url, string json, Action doneAction, Action failAction = null) {
//        StartCoroutine(SendRequestCoroutine(url, json, doneAction, failAction, System.Environment.StackTrace));
//    }

//    IEnumerator SendRequestCoroutine(string url, string json, Action doneAction, Action failAction, string stackTrace) {
//        UnityWebRequest www = UnityWebRequest.Put(url, json);
//        www.SetRequestHeader("Content-Type", "application/json");

//        float startRequest = Time.time;

//        yield return www.SendWebRequest();

//        result = www.downloadHandler.text;

//        if (DEBUG_MODE) {
//            Debug.Log(url + " : " + result);
//        }

//        if (www.isNetworkError || www.isHttpError) {
//            Debug.LogError(result);

//            if (DEBUG_MODE) {
//                Debug.LogError("StackTrace detail " + stackTrace);
//            }

//            if (failAction != null) {
//                failAction.Invoke();
//            }
//        } else {
//            if (doneAction != null) {
//                doneAction.Invoke();
//            }

//            try {
//                Hashtable hash = JsonConvert.DeserializeObject<Hashtable>(result);
//                if (hash["result"].Equals("fail")) {
//                    Debug.LogError(hash["detail"].ToString());

//                    if (DEBUG_MODE) {
//                        Debug.LogError("StackTrace detail " + stackTrace);
//                    }
//                }
//            } catch { }
//        }
//        www.Dispose();
//    }

//    public bool IsResponseOK() {
//        Hashtable hash = JsonConvert.DeserializeObject<Hashtable>(result);

//        if (hash != null && hash["result"] != null && hash["result"].ToString() == "success")
//            return true;
//        else
//            return false;
//    }

//    public string ReadData() {
//        Hashtable hash = JsonConvert.DeserializeObject<Hashtable>(result);

//        return hash["data"].ToString();
//    }

//    public string ResultDetail() {
//        Hashtable hash = JsonConvert.DeserializeObject<Hashtable>(result);

//        return hash["detail"].ToString();
//    }
//}
