using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkcupGames
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class PaintableSpriteMask : PaintableObject
    {
        public SpriteMask spriteMask;

        public override void Start()
        {
            spriteMask = GetComponent<SpriteMask>();
            base.Start();
        }

        public override void ApplyTexture(Texture2D texture2D)
        {
            spriteMask.sprite = Sprite.Create(texture, spriteMask.sprite.rect, new Vector2(0.5f, 0.5f), spriteMask.sprite.pixelsPerUnit);
        }

        public override Texture2D GetSourceTexture()
        {
            return spriteMask.sprite.texture;
        }
    }
}