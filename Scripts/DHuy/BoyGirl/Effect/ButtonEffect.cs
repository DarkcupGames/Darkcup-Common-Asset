using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DarkcupGames;
using DG.Tweening;

public class ButtonEffect : MonoBehaviour
{
    public readonly Vector2 MAX_SIZE = new Vector2(3f, 3f);
    public const float ANIMATION_TIME = 1f;

    public SpriteRenderer spriteRenderer;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        transform.localScale = new Vector3(1, 1);
        transform.DOScale(MAX_SIZE, ANIMATION_TIME);
        spriteRenderer.color = Color.white;
        spriteRenderer.DOFade(0f, ANIMATION_TIME).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}