using UnityEngine;
using TMPro;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class TextCountdown : MonoBehaviour
{
    public float timeRemaining = 10f; 
    private TextMeshProUGUI countdownText;
    private Action onCountdownComplete;
    private CanvasGroup canvasGroup;

    private bool timerIsRunning = false;
    private void Awake()
    {
        countdownText = GetComponent<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }
    public void CountDown(int seconds, Action onComplete)
    {
        this.onCountdownComplete = onComplete;
        timeRemaining = seconds;
        timerIsRunning = true;
    }
    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateCountdownText(timeRemaining);
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;
                onCountdownComplete?.Invoke();
                UpdateCountdownText(timeRemaining);
            }
        }
    }

    void UpdateCountdownText(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}