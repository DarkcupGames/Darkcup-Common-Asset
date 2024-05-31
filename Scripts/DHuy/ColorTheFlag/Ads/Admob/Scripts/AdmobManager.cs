using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdmobManager : MonoBehaviour
{
    public static AdmobManager Instance;
    [SerializeField] private bool showDebug;
    public static bool isReady;
    public bool showAds = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (showDebug) Debug.Log("starting consent request");
        ConsentRequestParameters request = new ConsentRequestParameters();
        ConsentInformation.Update(request, OnConsentInfoUpdated);
    }
    void OnConsentInfoUpdated(FormError consentError)
    {
        if (consentError != null)
        {
            if (showDebug) Debug.LogError(consentError);
            return;
        }
        ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) =>
        {
            if (showDebug) Debug.Log("LoadAndShowConsentFormIfRequired");
            if (formError != null)
            {
                UnityEngine.Debug.LogError(consentError);
                return;
            }
            if (showDebug) Debug.Log("can request ads " + ConsentInformation.CanRequestAds());
            if (ConsentInformation.CanRequestAds())
            {
                Init();
            }
        });
    }
    public void Init()
    {
        MobileAds.Initialize(initStatus =>
        {
            if (showDebug) Debug.Log("init finish with status = " + initStatus);
            var ads = GetComponentsInChildren<AdmobAds>();
            for (int i = 0; i < ads.Length; i++)
            {
                ads[i].Init();
                ads[i].LoadAds();
            }
            isReady = true;
        });
    }
}