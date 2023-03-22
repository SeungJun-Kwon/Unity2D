using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestItem : MonoBehaviour
{
    public ItemSO _item;

    SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        TryGetComponent(out _spriteRenderer);
    }

    private void Start()
    {
        _spriteRenderer.sprite = _item._icon;
    }

    private void OnMouseDown()
    {
        InventoryManager.Instance.AddItem(_item);
    }
}
