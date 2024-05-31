//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;
//using com.adjust.sdk;
//using UnityEngine.SceneManagement;
//using GoogleMobileAds.Common;

//public class IronsourceManager : MonoBehaviour
//{
//	public static IronsourceManager Instance;

//    public const float MIN_SECONDS_FOR_LOAD_NEW_ADS = 5;

//#if !UNITY_IOS
//    public const string appKey = "13c37c001"; //real id from commandoo 
//    //public const string appKey = "182ca7905"; //test id darkcupgames
//#endif

//#if UNITY_IOS
//	//real id from commandoo
//	public const string appKey = "1739d8a4d"; 
//#endif

//	public static Action rewardAction;
//	public static Action intertistialCloseAction;
//	public bool showAds = true;

//	public float lastShowIntertistial;
//	public float lastShowReward;
//	public float lastLoadReward;
//	public float lastLoadIntertistial;
//	public bool isShowingIntertistial;
//	public bool isShowingReward;

//	PlacementIntertistial currentInter;
//	PlacementRewarded currentReward;
//	PlacementBanner currentBanner = PlacementBanner.banner_bottom;

//	float lastClickAdsTime;
//	float multiClickPreventTime;

//    private void Awake() {
//        if (Instance == null)
//        {
//            Instance = this;
//			DontDestroyOnLoad(gameObject);
//		}
//        else
//        {
//			Destroy(gameObject);
//        }
//    }

//	void Start() {
//		Debug.Log("unity-script: MyAppStart Start called");

//		//Dynamic config example
//		IronSourceConfig.Instance.setClientSideCallbacks(true);

//		string id = IronSource.Agent.getAdvertiserId();
//		Debug.Log("unity-script: IronSource.Agent.getAdvertiserId : " + id);

//		Debug.Log("unity-script: IronSource.Agent.validateIntegration");
//		IronSource.Agent.validateIntegration();

//		Debug.Log("unity-script: unity version" + IronSource.unityVersion());

//		// Add Banner Events
//		IronSourceEvents.onBannerAdLoadedEvent += BannerAdLoadedEvent;
//		IronSourceEvents.onBannerAdLoadFailedEvent += BannerAdLoadFailedEvent;
//		IronSourceEvents.onBannerAdClickedEvent += BannerAdClickedEvent;
//		IronSourceEvents.onBannerAdScreenPresentedEvent += BannerAdScreenPresentedEvent;
//		IronSourceEvents.onBannerAdScreenDismissedEvent += BannerAdScreenDismissedEvent;
//		IronSourceEvents.onBannerAdLeftApplicationEvent += BannerAdLeftApplicationEvent;

//		// Add Reward event
//		IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
//		IronSourceEvents.onRewardedVideoAdClickedEvent += RewardedVideoAdClickedEvent;
//		IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
//		IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
//		IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
//		IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
//		IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
//		IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
//		IronSourceEvents.onRewardedVideoAdReadyEvent += RewardVideoReadyEvent;

//		//track Adjust revenue
//		IronSourceEvents.onImpressionDataReadyEvent += ImpressionDataReadyEvent;

//		// SDK init
//		Debug.Log("unity-script: IronSource.Agent.init");
//		IronSource.Agent.init(appKey, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL, IronSourceAdUnits.OFFERWALL, IronSourceAdUnits.BANNER);

//		// Load Banner example
//		if (showAds) {
//			IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
//			IronSource.Agent.displayBanner();

//			IronSource.Agent.loadInterstitial();
//			IronSource.Agent.loadRewardedVideo();
//		}
//	}
	
//	private void ImpressionDataReadyEvent(IronSourceImpressionData impressionData)
//	{
//		AdjustAdRevenue adjustAdRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceIronSource);
//		double revenue = impressionData.revenue.GetValueOrDefault();
//		adjustAdRevenue.setRevenue(revenue, "USD");
//		// optional fields
//		adjustAdRevenue.setAdRevenueNetwork(impressionData.adNetwork);
//		adjustAdRevenue.setAdRevenueUnit(impressionData.adUnit);
//		adjustAdRevenue.setAdRevenuePlacement(impressionData.placement);
//		// track Adjust ad revenue
//		Adjust.trackAdRevenue(adjustAdRevenue);
//	}

//	public bool CanShowAppOpen()
//    {
//        Debug.Log($"Check can show app open");
//        Debug.Log($"isShowingReward = {isShowingReward}, isShowingIntertistial = {isShowingIntertistial}");
//        Debug.Log($"lastShowIntertistial = {lastShowIntertistial}, lastShowReward = {lastShowReward}");
//        Debug.Log($"time since last inter = {Time.realtimeSinceStartup - lastShowIntertistial}, time since last reward = {Time.realtimeSinceStartup - lastShowReward}");
//        if (isShowingReward || isShowingIntertistial)
//        {
//			Debug.Log("Can show app open = false, reason = isShowingReward || isShowingIntertistial");
//			return false;
//		}
//		if (Time.realtimeSinceStartup - lastShowIntertistial < Constants.MIN_SECONDS_AFTER_ADS_TO_SHOW_APP_OPEN) {
//			Debug.Log("Can show app open = false, reason = Time.realtimeSinceStartup - lastShowIntertistial < Constants.MIN_SECONDS_AFTER_ADS_TO_SHOW_APP_OPEN");
//			return false;
//		} 
//		if (Time.realtimeSinceStartup - lastShowReward < Constants.MIN_SECONDS_AFTER_ADS_TO_SHOW_APP_OPEN)
//        {
//			Debug.Log("Can show app open = false, reason = Time.realtimeSinceStartup - lastShowReward < Constants.MIN_SECONDS_AFTER_ADS_TO_SHOW_APP_OPEN");
//			return false;
//        }
//		Debug.Log("Can show app open = true");
//		return true;
//    }

//	void OnEnable() {
//		IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
//		IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
//		IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
//		IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent;
//		IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
//		IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
//		IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;
//	}

//	void OnApplicationPause(bool isPaused) {
//		Debug.Log("unity-script: OnApplicationPause = " + isPaused);
//		IronSource.Agent.onApplicationPause(isPaused);
//	}

//	public void ShowInterstitial(PlacementIntertistial placement, Action intertistialCloseAction = null) {
//		DebugCustom.Log("calling show intertistial");
//		DebugCustom.Log($"intertistialCloseAction is null = {intertistialCloseAction == null}");
//		IronsourceManager.intertistialCloseAction = intertistialCloseAction;
//		if (!IronSource.Agent.isInterstitialReady())
//		{
//			DebugCustom.Log("inter not ready yet");
//			if (Time.realtimeSinceStartup - lastLoadIntertistial > MIN_SECONDS_FOR_LOAD_NEW_ADS)
//			{
//				DebugCustom.Log("request new intertistial");
//				IronSource.Agent.loadInterstitial();
//				lastLoadIntertistial = Time.realtimeSinceStartup;
//			}
//			//AnalyticManager.Log("inter fail");
//			FirebaseManager.Instance.LogFailedShowIntertistial(placement);
//			MainThreadManager.Instance.ExecuteInUpdate(() => {
//				DebugCustom.Log("invoke intertistial close action");
//				intertistialCloseAction?.Invoke();
//			});
//			return;
//		}
//		if (!showAds)
//        {
//			DebugCustom.Log("skip by not show ads");
//			intertistialCloseAction?.Invoke();
//			return;
//		}
//		if (ShouldShowIntertistial() == false) {
//			intertistialCloseAction?.Invoke();
//			return;
//		}
//		DebugCustom.Log("showing intertistial");
//		IronSource.Agent.showInterstitial();
//		//AnalyticManager.Log("inter show");
//		//lastLoadIntertistial = Time.realtimeSinceStartup;
//		currentInter = placement;
//		FirebaseManager.Instance.LogShowIntertistial(placement);
//		PlayerPrefs.SetInt("show_inter_ads", 1);
//	}

//	public bool ShouldShowIntertistial()
//    {
//		//Nếu là người chơi vip thì không hiện ads
//		if (GameSystem.userdata.boughtItems == null) GameSystem.userdata.boughtItems = new List<string>();
//		if (GameSystem.userdata.boughtItems.Contains(IAP_ID.no_ads.ToString())) return false;
//		//Không hiện nếu chưa đủ level trong remote config
//		if (GameSystem.userdata.level < FirebaseManager.minLevelToShowAds) return false;
//		//Không hiện nếu khoảng cách giữa 2 lần chưa đủ trong remote config
//		float time = Time.realtimeSinceStartup - lastShowIntertistial;
//		Debug.Log($"Check show intertistial, time = {time}, canshow = {time < FirebaseManager.minsSecondsBetweenIntertistial}");
//		if (time < FirebaseManager.minsSecondsBetweenIntertistial)
//        {
//			FirebaseManager.Instance.LogAction($"inter fail: time capped");
//			DebugCustom.Log($"inter fail: time capped");
//			return false;
//		}
//		//Không hiện nếu click nhiều lần lần
//		if (Time.realtimeSinceStartup < multiClickPreventTime) {
//			FirebaseManager.Instance.LogAction($"inter fail: multi click");
//			DebugCustom.Log($"inter fail: multi click");
//			return false;
//		}
//		//Không hiện nếu user đang chơi thua liên tục
//		//if (Gameplay.loseCount >= Constants.MULTI_LOSE_COUNT_DETECT)
//  //      {
//		//	FirebaseManager.Instance.LogAction($"inter fail: lose 3 times");
//		//	DebugCustom.Log($"inter fail: lose 3 times");
//		//	return false;
//		//}
//		return true;
//    }

//	public void ShowRewardAds() {
//		if (!showAds) {
//			//action.Invoke();
//			return;
//		}
//		PlacementRewarded placement = PlacementRewarded.reward_all;
//#if UNITY_EDITOR
//		AdManager.Instance.HandleEarnReward();
//		return;
//#endif

//		//rewardAction = action;
//		bool available = IronSource.Agent.isRewardedVideoAvailable();

//		if (!available) {
//			if (Time.realtimeSinceStartup - lastLoadReward > MIN_SECONDS_FOR_LOAD_NEW_ADS)
//            {
//				IronSource.Agent.loadRewardedVideo();
//				lastLoadReward = Time.realtimeSinceStartup;
//			}
//			FirebaseManager.Instance.LogFailedShowReward(placement);
//			//AnalyticManager.Log("reward fail");
//        } else {
//			IronSource.Agent.showRewardedVideo();
//			//AnalyticManager.Log("reward show");
//			IronSource.Agent.loadRewardedVideo();
//			lastLoadReward = Time.realtimeSinceStartup;
//			currentReward = placement;
//			FirebaseManager.Instance.LogShowRewarded(placement);
//		}
//	}

//	#region INTERSTITIAL

//	// Invoked when the initialization process has failed.
//	// @param description - string - contains information about the failure.
//	void InterstitialAdLoadFailedEvent(IronSourceError error) {
//	}
//	// Invoked when the ad fails to show.
//	// @param description - string - contains information about the failure.
//	void InterstitialAdShowFailedEvent(IronSourceError error) {
//	}
//	// Invoked when end user clicked on the interstitial ad
//	void InterstitialAdClickedEvent() {
//		//multi click detection
//		if (Time.realtimeSinceStartup - lastClickAdsTime < Constants.MULTI_ADS_CLICK_DETECTION)
//        {
//			multiClickPreventTime = Time.realtimeSinceStartup + Constants.PAUSE_ADS_AFTER_MULTI_CLICK;
//        }
//		lastClickAdsTime = Time.realtimeSinceStartup;

//		//int temp = PlayerPrefs.GetInt(AnalyticManager.AD_CLICK_INTERSTITIAL_COUNT, 0);
//		//temp++;
//		//PlayerPrefs.SetInt(AnalyticManager.AD_CLICK_INTERSTITIAL_COUNT, temp);

//		//int all = PlayerPrefs.GetInt(AnalyticManager.AD_CLICK_COUNT, 0);
//		//all++;
//		//PlayerPrefs.SetInt(AnalyticManager.AD_CLICK_COUNT, all);
//		//AnalyticManager.Log("inter click");
//		FirebaseManager.Instance.LogIntertistialClick(currentInter);
//	}

//	// Invoked when the interstitial ad closed and the user goes back to the application screen.
//	void InterstitialAdClosedEvent() {
//		isShowingIntertistial = false;
//		lastShowIntertistial = Time.realtimeSinceStartup;
//		IronSource.Agent.loadInterstitial();
//		lastLoadIntertistial = Time.realtimeSinceStartup;
//		FirebaseManager.Instance.LogIntertistialSuccess(currentInter);
//		intertistialCloseAction?.Invoke();
//	}
//	// Invoked when the Interstitial is Ready to shown after load function is called
//	void InterstitialAdReadyEvent() {
//		FirebaseManager.Instance.LogIntertistialReady();
//	}
//	// Invoked when the Interstitial Ad Unit has opened
//	void InterstitialAdOpenedEvent() {
//		isShowingIntertistial = true;
//	}
//	// Invoked right before the Interstitial screen is about to open.
//	// NOTE - This event is available only for some of the networks. 
//	// You should treat this event as an interstitial impression, but rather use InterstitialAdOpenedEvent
//	void InterstitialAdShowSucceededEvent() {
//	}

//	#endregion

//	#region BANNER
//	//Banner Events
//	void BannerAdLoadedEvent() {
//		Debug.Log("unity-script: I got BannerAdLoadedEvent");
//	}

//	void BannerAdLoadFailedEvent(IronSourceError error) {
//		Debug.Log("unity-script: I got BannerAdLoadFailedEvent, code: " + error.getCode() + ", description : " + error.getDescription());
//	}

//	void BannerAdClickedEvent() {
//		//Debug.Log("unity-script: I got BannerAdClickedEvent");
//		FirebaseManager.Instance.LogBannerClick(currentBanner);
//	}

//	void BannerAdScreenPresentedEvent() {
//		Debug.Log("unity-script: I got BannerAdScreenPresentedEvent");
//		FirebaseManager.Instance.LogShowBanner(currentBanner);
//	}

//	void BannerAdScreenDismissedEvent() {
//		Debug.Log("unity-script: I got BannerAdScreenDismissedEvent");
//	}

//	void BannerAdLeftApplicationEvent() {
//		Debug.Log("unity-script: I got BannerAdLeftApplicationEvent");
//	}
//	#endregion

//	#region REWARD
//	//Invoked when the RewardedVideo ad view has opened.
//	//Your Activity will lose focus. Please avoid performing heavy 
//	//tasks till the video ad will be closed.
//	void RewardedVideoAdOpenedEvent() {
//		isShowingReward = true;
//	}
//	//Invoked when the RewardedVideo ad view is about to be closed.
//	//Your activity will now regain its focus.
//	void RewardedVideoAdClosedEvent() {
//		isShowingReward = false;
//		lastShowReward = Time.realtimeSinceStartup;
//		FirebaseManager.Instance.LogRewardedSuccess(currentReward);
//	}
//	//Invoked when there is a change in the ad availability status.
//	//@param - available - value will change to true when rewarded videos are available. 
//	//You can then show the video by calling showRewardedVideo().
//	//Value will change to false when no videos are available.
//	void RewardedVideoAvailabilityChangedEvent(bool available) {
//		//Change the in-app 'Traffic Driver' state according to availability.
//		bool rewardedVideoAvailability = available;
//	}

//	//Invoked when the user completed the video and should be rewarded. 
//	//If using server-to-server callbacks you may ignore this events and wait for 
//	// the callback from the  ironSource server.
//	//@param - placement - placement object which contains the reward data
//	void RewardedVideoAdRewardedEvent(IronSourcePlacement placement) {
//		//rewardAction.Invoke();
//		AdManager.Instance.HandleEarnReward();
//	}
//	//Invoked when the Rewarded Video failed to show
//	//@param description - string - contains information about the failure.
//	void RewardedVideoAdShowFailedEvent(IronSourceError error) {
//	}

//	// ----------------------------------------------------------------------------------------
//	// Note: the events below are not available for all supported rewarded video ad networks. 
//	// Check which events are available per ad network you choose to include in your build. 
//	// We recommend only using events which register to ALL ad networks you include in your build. 
//	// ----------------------------------------------------------------------------------------

//	//Invoked when the video ad starts playing. 
//	void RewardedVideoAdStartedEvent() {
//	}
//	//Invoked when the video ad finishes playing. 
//	void RewardedVideoAdEndedEvent() {
//	}
//	void RewardVideoReadyEvent()
//    {
//		FirebaseManager.Instance.LogRewardedReady();
//    }
//	//Invoked when the video ad is clicked. 
//	void RewardedVideoAdClickedEvent(IronSourcePlacement placement) {
//		FirebaseManager.Instance.LogRewardedClick(currentReward);
//	}
//	#endregion
//}
