using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace DarkcupGames
{
    public enum PaintPosition
    {
        AnyWhere, VisibleArea, InvisibleArea
    }

    public abstract class PaintableObject : MonoBehaviour
    {
        public const float TIME_PER_UPDATE = 0;

        public static PaintableObject currentPaint = null;
        public PaintPosition paintPosition = PaintPosition.AnyWhere;
        public Color paintColor = Color.clear;
        [SerializeField] private int size = 20;

        public Vector2Int lastPos { get; private set; }
        public Vector2 lastWorldPos { get; private set; }
        public List<Vector2> drawPoints { get; private set; }
        public bool isDrawing { get; private set; } = false;

        protected Texture2D texture;
        [SerializeField] protected BoxCollider2D drawBoundCollider;
        protected Color[] originalColors;
        protected Color[] m_Colors;
        private List<Vector2Int> offsets = new List<Vector2Int>();
        private bool canDraw = false;
        private bool changed;
        private float percent;
        private Camera mainCam;
        private Vector2 minPoint, maxPoint;
        private float nextUpdate;

        public int Size
        {
            get { return size; }
            set
            {
                size = value;
                CreateListOffset();
            }
        }
        public float Percent => percent;

        public virtual void Start()
        {
            mainCam = Camera.main;
            CreateListOffset();
            Init();
        }

        public void Init()
        {
            var source = GetSourceTexture();
            texture = new Texture2D(source.width, source.height, TextureFormat.ARGB32, false);
            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;
            m_Colors = source.GetPixels();
            originalColors = source.GetPixels();
            texture.SetPixels(m_Colors);
            texture.Apply();
            drawPoints = new List<Vector2>();
            if (drawBoundCollider == null) drawBoundCollider = GetComponent<BoxCollider2D>();
            minPoint = drawBoundCollider.bounds.min;
            maxPoint = drawBoundCollider.bounds.max;
            ApplyTexture(texture);

            Debug.Log($"this is mipmap count: " + texture.mipmapCount);
        }
        Vector2 pos;
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                canDraw = true;

                pos = mainCam.ScreenToWorldPoint(Input.mousePosition);
                bool inside = drawBoundCollider.OverlapPoint(pos);
                if (inside)
                {
                    Vibration.Vibrate();
                    Debug.Log("Virrrrrrrr");
                }
            }

            if (!canDraw)
            {
                isDrawing = false;
                return;
            }

            if (Input.GetMouseButton(0))
            {
                if (Time.time > nextUpdate)
                {
                    nextUpdate = Time.time + TIME_PER_UPDATE;
                    pos = mainCam.ScreenToWorldPoint(Input.mousePosition);
                    bool inside = pos.x < maxPoint.x && pos.x > minPoint.x && pos.y > minPoint.y && pos.y < maxPoint.y;

                    if (inside && currentPaint == null)
                    {
                        currentPaint = this;
                    }

                    if (inside && currentPaint == this)
                    {
                        UpdateTexture(pos);
                        isDrawing = true;
                        return;
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (currentPaint == this)
                {
                    currentPaint = null;
                }
            }

            isDrawing = false;
        }
        public abstract Texture2D GetSourceTexture();

        public abstract void ApplyTexture(Texture2D texture2D);

        private void CreateListOffset()
        {
            Vector2 center = Vector2.zero;
            offsets = new List<Vector2Int>();
            for (int i = -size; i < size; i++)
            {
                for (int j = -size; j < size; j++)
                {
                    Vector2Int pos = new Vector2Int(i, j);
                    if (Vector2.Distance(pos, center) <= size)
                    {
                        offsets.Add(pos);
                    }
                }
            }
        }

        public void UpdateTexture(Vector2 pos)
        {
            if (size == 0)
            {
                Debug.LogError("size is 0");
                return;
            }
            changed = true;

            lastWorldPos = pos;
            drawPoints.Add(pos);

            var mousePos = pos - (Vector2)drawBoundCollider.bounds.min;
            mousePos.x *= texture.width / drawBoundCollider.bounds.size.x;
            mousePos.y *= texture.height / drawBoundCollider.bounds.size.y;
            Vector2Int center = new Vector2Int((int)mousePos.x, (int)mousePos.y);
            Vector2Int checkPos = Vector2Int.zero;
            Vector2 dir = lastPos - mousePos;


            for (int i = 0; i < offsets.Count; i++)
            {
                checkPos = center + offsets[i];
                checkPos.x = (int)Mathf.Clamp(checkPos.x, 0, texture.width);
                checkPos.y = (int)Mathf.Clamp(checkPos.y, 0, texture.height);
                int index = checkPos.x + checkPos.y * texture.width;
                index = Mathf.Clamp(index, 0, m_Colors.Length - 1);
                if (paintPosition == PaintPosition.VisibleArea && m_Colors[index].a == 0) continue;
                if (paintPosition == PaintPosition.InvisibleArea && m_Colors[index].a != 0) continue;
                texture.SetPixel(checkPos.x, checkPos.y, paintColor);
            }

            for (int i = 0; i < offsets.Count; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Vector2 offset1 = Vector2.Lerp(lastPos, center, (j / 5));
                    checkPos = center + offsets[i] + new Vector2Int((int)offset1.x, (int)offset1.y) * j;
                    checkPos.x = (int)Mathf.Clamp(checkPos.x, 0, texture.width);
                    checkPos.y = (int)Mathf.Clamp(checkPos.y, 0, texture.height);
                    int index = checkPos.x + checkPos.y * texture.width;
                    index = Mathf.Clamp(index, 0, m_Colors.Length - 1);
                    if (paintPosition == PaintPosition.VisibleArea && m_Colors[index].a == 0) continue;
                    if (paintPosition == PaintPosition.InvisibleArea && m_Colors[index].a != 0) continue;
                    texture.SetPixel(checkPos.x, checkPos.y, paintColor);
                }
            }

            lastPos = center;
            texture.Apply();
            ApplyTexture(texture);
        }

        public float GetDrawPercent()
        {
            if (m_Colors == null) return 0;
            float count = 0;
            for (int i = 0; i < m_Colors.Length; i++)
            {
                if (m_Colors[i] == paintColor)
                {
                    count++;
                }
            }
            return count / m_Colors.Length;
        }

        public void ClearDraw()
        {
            if (!changed) return;
            changed = false;

            texture.SetPixels(originalColors);
            texture.Apply();
            ApplyTexture(texture);

            Init();

            if (currentPaint == this)
            {
                currentPaint = null;
            }
        }
    }
}