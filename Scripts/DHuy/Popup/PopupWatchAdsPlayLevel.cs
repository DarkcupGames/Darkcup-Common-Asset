using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkcupGames;
using UnityEngine.SceneManagement;

public class PopupWatchAdsPlayLevel : MonoBehaviour
{
    int level;

    public void ShowWatchAds(int level)
    {
        this.level = level;
        ShowHideChangeSceneLogic.ShowPopup(gameObject);        
    }

    public void OnWatchAdsComplete()
    {
        GameSystem.userdata.level = level;
        GameSystem.SaveUserDataToLocal();

        //save locked level
        string key = PopupLevelLocked.GetUnlockKey(level, out int step);
        PlayerPrefs.SetInt(key, 1);
        SceneManager.LoadScene(Constants.SCENE_GAMEPLAY);
    }
}