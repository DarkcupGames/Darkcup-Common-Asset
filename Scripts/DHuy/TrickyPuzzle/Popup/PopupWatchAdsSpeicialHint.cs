using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkcupGames;

public class PopupWatchAdsSpeicialHint : MonoBehaviour
{
    public void OnWatchAdsComplete()
    {
        FindObjectOfType<ButtonAdsHint>().AddHint(5);
        ShowHideChangeSceneLogic.HidePopup(gameObject);
    }
}