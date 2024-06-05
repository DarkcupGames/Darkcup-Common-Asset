using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

namespace DarkcupGames
{
    public class PopupConfirm : MonoBehaviour
    {
        [SerializeField] private GameObject popupBody;
        [SerializeField] private Button btnYes;
        [SerializeField] private Button btnNo;
        [SerializeField] private Button btnOK;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI mess;
        [SerializeField] private TextMeshProUGUI txtYes;
        [SerializeField] private TextMeshProUGUI txtNo;
        [SerializeField] private TextMeshProUGUI txtOk;
        private Action yesAction;
        private Action noAction;
        private Action okAction;

        public void ShowOK(string title, string message)
        {
            this.popupBody.SetActive(true);
            EasyEffect.Appear(popupBody, 0f, 1f);
            this.title.text = title;
            this.mess.text = message;
            this.btnYes.gameObject.SetActive(false);
            this.btnNo.gameObject.SetActive(false);
            this.btnOK.gameObject.SetActive(true);
        }

        public void ShowYesNo(string title, string message, string yes, string no, Action yesAction)
        {
            this.popupBody.SetActive(true);
            EasyEffect.Appear(popupBody, 0.7f, 1f, speed: 0.1f);
            this.title.text = title;
            this.mess.text = message;
            this.txtYes.text = yes;
            this.txtNo.text = no;
            this.yesAction = yesAction;
            this.btnYes.gameObject.SetActive(true);
            this.btnNo.gameObject.SetActive(true);
            this.btnOK.gameObject.SetActive(false);
        }

        public void Close()
        {
            AudioSystem.Instance.PlayButtonSound();
            EasyEffect.Appear(popupBody, 1f, 0f);
        }

        public void OnYes()
        {
            if (yesAction != null) yesAction.Invoke();
            Close();
        }
    }
}