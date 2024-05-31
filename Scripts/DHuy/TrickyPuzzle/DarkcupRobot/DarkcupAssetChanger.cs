using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DarkcupAssetChanger : MonoBehaviour
{
    public List<Sprite> sprites;

    [ContextMenu("Change Asset")]
    public void ChangeAsset()
    {
        var children = gameObject.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < children.Length; i++)
        {
            var image = children[i].GetComponent<Image>();
            if (image != null) TryChangeAsset(image);
            var renderer = children[i].GetComponent<SpriteRenderer>();
            if (renderer != null) TryChangeAsset(renderer);
        }
    }

    public void TryChangeAsset(Image image)
    {
        for (int i = 0; i < sprites.Count; i++)
        {
            if (image.name != sprites[i].name) continue;
            image.sprite = sprites[i];
        }
    }
    public void TryChangeAsset(SpriteRenderer renderer)
    {
        for (int i = 0; i < sprites.Count; i++)
        {
            if (renderer.name != sprites[i].name) continue;
            renderer.sprite = sprites[i];
        }
    }
}
