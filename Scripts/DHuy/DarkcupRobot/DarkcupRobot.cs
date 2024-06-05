#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DarkcupRobot : MonoBehaviour
{
    public int index;

    public void Next()
    {
        index++;
        ShowPrefab(index);
    }

    public void Previous()
    {
        index--;
        ShowPrefab(index);
    }

    public void ShowPrefab(int index)
    {
        LevelManager level = Resources.Load<LevelManager>("Levels/Level" + index);
        string assetPath = AssetDatabase.GetAssetPath(level.gameObject);
        AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath(assetPath, typeof(LevelManager)));
        //PrefabUtility.LoadPrefabContents(assetPath);
        
    }
}
#endif