using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DarkcupGames;
using DG.Tweening;
using TMPro;

[RequireComponent(typeof(SpriteRenderer))]
public class FrameAnimatorSprite : FramesAnimator
{
    SpriteRenderer spriteRenderer;

    public override void Start() {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void ApplySprite(Sprite sprite) {
        spriteRenderer.sprite = sprite;
    }
}