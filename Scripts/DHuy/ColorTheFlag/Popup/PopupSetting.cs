using UnityEngine;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class PopupSetting : MonoBehaviour
{
    public GameObject settingBoard;
    public Vector3 popupScale;
    public float doSpeed;

    private Vector3 firstScale;

    private void OnEnable()
    {
        firstScale = transform.localScale;
        settingBoard.transform.DOScale(popupScale, doSpeed).SetEase(Ease.OutBack).SetUpdate(true);
        FirebaseManager.analytics.LogUIAppear(SceneManager.GetActiveScene().name, "popup_setting");
    }
    
    public void TurnOff()
    {
        settingBoard.transform.DOScale(firstScale, doSpeed).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
        {
            gameObject.SetActive(false);
            Time.timeScale = 1;
        });
    }
}