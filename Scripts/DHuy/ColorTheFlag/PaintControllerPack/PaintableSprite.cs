using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DarkcupGames;
using UnityEngine.SceneManagement;

namespace DarkcupGames
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class PaintableSprite : PaintableObject
    {
        SpriteRenderer spriteRenderer;

        public override void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            base.Start();
        }

        public override void ApplyTexture(Texture2D texture2D)
        {
            spriteRenderer.sprite = Sprite.Create(texture, spriteRenderer.sprite.rect, new Vector2(0.5f, 0.5f), spriteRenderer.sprite.pixelsPerUnit);
        }

        public override Texture2D GetSourceTexture()
        {
            return spriteRenderer.sprite.texture;
        }
    }
}