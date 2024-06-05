using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;
    public PopupWatchAdsPlayLevel popupWatchAdsPlayLevel;

    private void Awake()
    {
        Instance = this;
    }
}