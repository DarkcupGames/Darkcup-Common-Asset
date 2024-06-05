using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class CountDown : MonoBehaviour
{
    public const int COUNT_DOWN_TIME = 3;
    public const int RUN_TOTAL_SECOUNDS = 600;

    public static CountDown Instance;

    public PopupCountDown popupCountDown;
    public TextCountdown txtCountDown;
    public MazeCharacterMovement movement;
    public MazeManager mazeManager;

    private void Awake()
    {
        Instance = this;
        movement.enabled = false;
    }

    public void CountDownAndStartGame()
    {
        StartCountDown(() =>
        {
            movement.enabled = true;
            mazeManager.StartGame();
        }, ()=> {
            Debug.Log("Countdown finished");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }

    public void StartCountDown(Action onStartCountDown, Action onFinishCountDown)
    {
        popupCountDown.gameObject.SetActive(true);
        popupCountDown.CountDown(COUNT_DOWN_TIME, () =>
        {
            txtCountDown.gameObject.SetActive(true);
            var canvasGroup = txtCountDown.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            LeanTween.value(0f, 1f, 1f).setOnUpdate((float f) =>
            {
                canvasGroup.alpha = f;
            });
            txtCountDown.CountDown(RUN_TOTAL_SECOUNDS, () =>
            {
                LeanTween.value(1f, 0f, 1f).setOnUpdate((float f) =>
                {
                    canvasGroup.alpha = f;
                });
                onFinishCountDown?.Invoke();
            });
            onStartCountDown?.Invoke();
        });
    }
}