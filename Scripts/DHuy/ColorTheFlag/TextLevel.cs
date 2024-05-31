using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextLevel : MonoBehaviour
{
    private TextMeshProUGUI txtLevel;
    private int currentLevel;
    private void Awake()
    {
        txtLevel = GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        txtLevel.text = "LEVEL " + (GameSystem.userdata.level + 1);
    }
    private void Update()
    {
        if (currentLevel != GameSystem.userdata.level)
        {
            currentLevel = GameSystem.userdata.level;
            txtLevel.text = "LEVEL " + (currentLevel + 1);
        }
    }
}
