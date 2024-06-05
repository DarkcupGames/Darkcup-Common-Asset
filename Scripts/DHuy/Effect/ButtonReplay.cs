using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DarkcupGames;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ButtonReplay : MonoBehaviour
{
    public FadeInOut fadeInOut;

    public void Replay()
    {
        fadeInOut.FadeOut(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }
}