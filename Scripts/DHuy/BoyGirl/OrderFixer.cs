using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderFixer : MonoBehaviour
{
    public int sortingOrder;

    public bool dynamic = false;

    SpriteRenderer sprite;
    MeshRenderer mesh;

    public virtual void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        mesh = GetComponent<MeshRenderer>();
        enabled = dynamic;
        Update();
    }

    private void Update()
    {
        int order = GetSortingOrder();
        if (sprite) sprite.sortingOrder = order;
        if (mesh) mesh.sortingOrder = order;
    }

    public int GetSortingOrder()
    {
        return -(int)(transform.position.y * 100) + sortingOrder;
    }
}
