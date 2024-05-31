using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DarkcupGames;
using DG.Tweening;
using TMPro;

[RequireComponent(typeof(Image))]
public class FrameAnimatorUI : FramesAnimator
{
    Image img;
    public override void Start() {
        base.Start();
        img = GetComponent<Image>();
    }
    public override void ApplySprite(Sprite sprite) {
        img.sprite = sprite;
    }
}
