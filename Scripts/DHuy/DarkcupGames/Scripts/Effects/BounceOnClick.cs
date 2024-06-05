using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DarkcupGames
{
    public class BounceOnClick : MonoBehaviour
    {
        private Button button;
        private Vector2 localScale;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(Bounce);
            localScale = button.transform.localScale;
        }

        public void Bounce()
        {
            button.transform.localScale = localScale;
            EasyEffect.Bounce(gameObject, 0.1f, strength: 0.2f);
        }
    }
}