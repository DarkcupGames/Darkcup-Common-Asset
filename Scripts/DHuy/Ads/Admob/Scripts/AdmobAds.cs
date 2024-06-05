using UnityEngine;

public abstract class AdmobAds : MonoBehaviour
{
    [SerializeField] protected bool showDebug;
    public abstract void Init();
    public abstract void LoadAds();
    public abstract bool ShowAds(System.Action onShowAdsComplete);
    public abstract bool IsAdsAvailable();
    public abstract bool IsShowingAds();
}