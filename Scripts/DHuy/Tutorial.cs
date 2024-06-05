using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tutorial : MonoBehaviour
{
    public Transform dotParent;
    public SpriteRenderer hand;

    public float speed = 0.2f;

    private void Start()
    {
        if (hand == null)
        {
            foreach (Transform child in transform)
            {
                if (child.name == "Hand")
                {
                    hand = child.GetComponent<SpriteRenderer>();
                }
            }
        }
        DoAnimation();
    }

    public void DoAnimation()
    {
        hand.transform.position = dotParent.transform.GetChild(0).transform.position;
        hand.color = Color.white;

        Sequence sequence = DOTween.Sequence();
        
        for (int i = 1; i < dotParent.childCount; i++)
        {
            Transform trans = dotParent.transform.GetChild(i);
            sequence.Append(hand.transform.DOMove(trans.position, speed).SetEase(Ease.Linear));
        }

        sequence.Append(hand.DOFade(0f, 1f));
        sequence.AppendCallback(() =>
        {
            DoAnimation();
        });
    }

    //private void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        gameObject.SetActive(false);
    //    }
    //}
}
