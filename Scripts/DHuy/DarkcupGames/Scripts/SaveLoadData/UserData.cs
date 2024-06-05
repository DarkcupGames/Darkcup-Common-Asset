using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SettingKey
{
    Sound, Music, Vibration
}

public enum LevelState
{
    Locked, Finish, Playing
}

[System.Serializable]
public class UserData
{
    public int level;
    public int maxLevel;
    public Dictionary<string, LevelState> levelLib = new Dictionary<string, LevelState> ();
    public Dictionary<SettingKey, bool> dicSetting = new Dictionary<SettingKey, bool> ();
    public List<string> boughtItems;

    public UserData ()
    {
        boughtItems = new List<string> ();
        levelLib = new Dictionary<string, LevelState> ();
    }
    public void CheckValid ()
    {
        if (boughtItems == null) boughtItems = new List<string> ();
        if (dicSetting == null) dicSetting = new Dictionary<SettingKey, bool> ();
        if (dicSetting.ContainsKey (SettingKey.Sound) == false) dicSetting.Add (SettingKey.Sound, true);
        if (dicSetting.ContainsKey (SettingKey.Music) == false) dicSetting.Add (SettingKey.Music, true);
        if (dicSetting.ContainsKey (SettingKey.Vibration) == false) dicSetting.Add (SettingKey.Vibration, true);
    }
}