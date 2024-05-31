using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using DarkcupGames;

public class GameSystem : MonoBehaviour
{
    public static GameSystem Instance;
    public static UserData  userdata;
    public const string USER_DATA_FILE_NAME = "user_data";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad (gameObject);
        } else
        {
            Destroy (gameObject);
            return;
        }

        LoadUserData();
        if (userdata == null)
        {
            userdata = new UserData();
        }
        userdata.CheckValid();
    }

    public static Vector3 GoToTargetVector(Vector3 current, Vector3 target, float speed, bool isFlying = false)
    {
        float distanceToTarget = Vector3.Distance(current, target);
        if (distanceToTarget < 0.1f)
            return new Vector3(0, 0);

        Vector3 vectorToTarget = target - current;

        vectorToTarget = vectorToTarget * speed / distanceToTarget;

        return vectorToTarget;
    }

    public IEnumerator IEIncreaseNumber(TextMeshProUGUI txtNumber, long startGold, long endGold, float effectTime, string endText = "")
    {
        int increase = (int)((endGold - startGold) / (effectTime / Time.deltaTime));
        if (increase == 0)
        {
            increase = endGold > startGold ? 1 : -1;
        }
        long gold = startGold;
        bool loop = true;
        while (loop)
        {
            gold += increase;

            if (startGold < endGold)
            {
                loop = gold < endGold;
            }
            else
            {
                loop = gold > endGold;
            }

            txtNumber.text = gold.ToString() + endText;

            yield return new WaitForEndOfFrame();
        }
    }

    public static void SaveUserDataToLocal()
    {
        string json = JsonConvert.SerializeObject(GameSystem.userdata);
        string path = FileUtilities.GetWritablePath(GameSystem.USER_DATA_FILE_NAME);
        FileUtilities.SaveFile(System.Text.Encoding.UTF8.GetBytes(json), path, true);
    }

    public static void LoadUserData()
    {
        if (!FileUtilities.IsFileExist(GameSystem.USER_DATA_FILE_NAME))
        {
            GameSystem.userdata = new UserData();
            GameSystem.SaveUserDataToLocal();
        }
        else
        {
            GameSystem.userdata = FileUtilities.DeserializeObjectFromFile<UserData>(GameSystem.USER_DATA_FILE_NAME);
            if (userdata == null) userdata = new UserData();
        }
    }

    public static void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void CheckLevelLib()
    {
        for (int i = 0; i < AddresableProperty.Instance.maxLevelIndex; i++)
        {
            var key = $"Level {i + 1}";
            if (!userdata.levelLib.ContainsKey (key))
            {
                userdata.levelLib.Add (key, LevelState.Locked);
            }
        }
        userdata.levelLib["Level 1"] = LevelState.Playing;
        SaveUserDataToLocal ();
    }
}