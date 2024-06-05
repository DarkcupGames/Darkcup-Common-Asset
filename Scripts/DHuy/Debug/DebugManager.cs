using AYellowpaper.SerializedCollections;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour
{
    public const bool TEST_MODE = false;
    public const int CLICK_LIMIT = 30;
    public static DebugManager Instance;
    [SerializeField] private GameObject debugConsole;
    [SerializeField] private GameObject debugPanel;
    [SerializeField] private GameObject textDebugGroup;
    [SerializeField] private GameObject debugButtonGroup;
    [SerializeField] private Button buttonShowDebugPanel;
    [SerializeField] private CanvasGroup canvasGroupGameplay;

    private int _clickCount = 0;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        debugConsole.gameObject.SetActive(false);
        textDebugGroup.gameObject.SetActive(false);
        if (TEST_MODE == false)
        {
            canvasGroupGameplay.enabled = false;
            debugPanel.gameObject.SetActive(false);
            buttonShowDebugPanel.gameObject.SetActive(false);
            debugButtonGroup.gameObject.SetActive(false);
            return;
        }
        canvasGroupGameplay.enabled = true;
        int show = PlayerPrefs.GetInt("debugPanel", 1);
        debugPanel.gameObject.SetActive(show == 1);
    }
    public void ClickActiveDebug()
    {
        _clickCount++;
        if (_clickCount > CLICK_LIMIT)
        {
            debugConsole.gameObject.SetActive(true);
            textDebugGroup.gameObject.SetActive(true);
        }
    }
    public void AddDebug(string value)
    {
        Debug.Log(value);
    }

    public void ShowDebugPanel()
    {
        debugPanel.gameObject.SetActive(true);
        PlayerPrefs.SetInt("debugPanel", 1);
    }

    public void HideDebugPanel()
    {
        debugPanel.gameObject.SetActive(false);
        PlayerPrefs.SetInt("debugPanel", 0);
    }
}