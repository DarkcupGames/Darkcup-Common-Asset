using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkcupGames
{
    public class ShowHidePopup : MonoBehaviour
    {
        public static void ShowPopup(GameObject obj)
        {
            EasyEffect.Appear(obj, 0.8f, 1f, speed: 0.15f);
        }

        public static void HidePopup(GameObject obj)
        {
            EasyEffect.Disappear(obj, 1f, 0.8f, speed: 0.15f);
        }

        public static void PauseGame(GameObject pausePanel)
        {
            pausePanel.SetActive(true);
            pausePanel.transform.SetAsLastSibling();
            Time.timeScale = 0f;
        }

        public static void ResumeGame(GameObject pausePanel)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}