using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkcupGames;
using UnityEngine.SceneManagement;
using TMPro;

public class PopupLevelLocked : MonoBehaviour
{
    public const string PLAYEREFS_UNLOCKED = "level_unlocked";
    public const int LOCKED_LEVEL_RANGE = 15;

    public TextMeshProUGUI txtInfo;

    public void CheckUnlocked()
    {
        string key = GetUnlockKey(GameSystem.userdata.level, out int step);
        bool unlocked = step == 0 || PlayerPrefs.GetInt(key, 0) == 1;
        if (unlocked == false)
        {
            ShowHideChangeSceneLogic.ShowPopup(gameObject);
        }
        txtInfo.text = "Unlock until level " + ((step+1) * LOCKED_LEVEL_RANGE);
    }

    public void OnWatchAdsCompleted()
    {
        string key = GetUnlockKey(GameSystem.userdata.level, out int step);
        PlayerPrefs.SetInt(key, 1);
        ShowHideChangeSceneLogic.HidePopup(gameObject);
    }

    public static string GetUnlockKey(int level, out int step)
    {
        //int level = GameSystem.userdata.level;
        step = level / LOCKED_LEVEL_RANGE;
        string key = PLAYEREFS_UNLOCKED + step;
        return key;
    }

    public void OnClose()
    {
        SceneManager.LoadScene(Constants.SCENE_HOME);
    }
}