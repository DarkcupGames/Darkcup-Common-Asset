using UnityEngine;

namespace DarkcupGames
{
    public class PaintableMesh : PaintableObject
    {
        private MeshRenderer meshRenderer;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        public override void ApplyTexture(Texture2D texture2D)
        {
            meshRenderer.material.mainTexture = texture2D;
        }

        public override Texture2D GetSourceTexture()
        {
            return (Texture2D)meshRenderer.material.mainTexture;
        }
    }
}