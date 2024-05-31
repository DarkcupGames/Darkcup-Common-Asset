using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using DG.Tweening;

namespace DarkcupGames
{
    public class EasyEffect : MonoBehaviour
    {
        public static EasyEffect Instance;

        const float DEFAULT_BOUNCE_STRENGTH = 0.5f;

        private void Awake()
        {
            Instance = this;
        }

        public static void Appear(float timeDelay,GameObject obj,float startScale, float endScale, float speed = 0.1f, float maxScale = 1.2f)
        {
            Sequence sq = DOTween.Sequence();
            sq.AppendInterval(timeDelay).AppendCallback(() => 
            {
                obj.SetActive(true);
                obj.transform.localScale = new Vector3(startScale, startScale);
                LeanTween.scale(obj, new Vector3(maxScale, maxScale, maxScale) * endScale, speed).setOnComplete(() =>
                {
                    LeanTween.scale(obj, new Vector3(1f, 1f, 1f) * endScale, speed);
                });
            });
           
        }
        public static void Appear(GameObject obj, float startScale, float endScale, float speed = 0.1f, float maxScale = 1.2f)
        {
            obj.SetActive(true);
            obj.transform.localScale = new Vector3(startScale, startScale);
            LeanTween.scale(obj, new Vector3(maxScale, maxScale, maxScale) * endScale, speed).setOnComplete(() =>
                {
                    LeanTween.scale(obj, new Vector3(1f, 1f, 1f) * endScale, speed);
                });
           
        }

        public static void Disappear(GameObject obj, float startScale, float endScale, float speed = 0.1f, float maxScale = 1.2f, Action doneAction = null)
        {
            obj.transform.localScale = new Vector3(startScale, startScale);
            LeanTween.scale(obj, new Vector3(maxScale, maxScale, maxScale) * startScale, speed).setOnComplete(() =>
            {
                LeanTween.scale(obj, new Vector3(1f, 1f, 1f) * endScale, speed).setOnComplete(() =>
                {
                    doneAction?.Invoke();
                    obj.SetActive(false);
                });
            });
        }

        public static void Bounce(GameObject go, float time, float strength = DEFAULT_BOUNCE_STRENGTH)
        {
            float baseScale = go.transform.localScale.x;

            LeanTween.scale(go, new Vector3(1 + strength, 1 - strength) * baseScale, time).setOnComplete(() =>
            {
                LeanTween.scale(go, new Vector3(1 - strength, 1 + strength) * baseScale, time).setOnComplete(() =>
                {
                    LeanTween.scale(go, new Vector3(1f, 1f) * baseScale, time);
                });
            });
        }

        public static void UfoCatch(GameObject obj, float to, float time, Action doneAction = null)
        {
            LeanTween.scale(obj, Vector3.zero, time);
            LeanTween.moveY(obj, to, time).setOnComplete(() =>
            {
                obj.SetActive(false);
                doneAction?.Invoke();
            });
        }

        public static void RunTextNumber(TextMeshProUGUI txtNumber, long from, long to, float effectTime, string endText = "")
        {
            Instance.StartCoroutine(Instance.IEIncreaseNumber(txtNumber, from, to, effectTime, endText));
        }

        public IEnumerator IEIncreaseNumber(TextMeshProUGUI txtNumber, long startGold, long endGold, float effectTime, string endText = "")
        {
            long increase = (long)((endGold - startGold) / (effectTime / Time.deltaTime));
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
            txtNumber.text = endGold.ToString() + endText;
        }
    }
}