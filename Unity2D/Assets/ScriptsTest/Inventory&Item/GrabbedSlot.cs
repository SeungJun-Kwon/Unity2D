using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GrabbedSlot : MonoBehaviour
{
    public static GrabbedSlot instance;

    public ItemSlot _curSlot;
    public Image _icon;

    public bool _isClicked;
    RectTransform _rect;

    private void Awake()
    {
        TryGetComponent(out _rect);
    }

    private void Start()
    {
        if(instance == null)
            instance = this;

        TryGetComponent<Image>(out var image);
        image.raycastTarget = false;
        _icon.raycastTarget = false;
    }

    private void LateUpdate()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(InventoryManager.Instance._rect, Input.mousePosition, Camera.main, out var pos);
        _rect.localPosition = pos;
    }

    public void SetItem(ItemSlot slot)
    {
        _curSlot = slot;
        _icon.sprite = _curSlot.Item._icon;
        _isClicked = true;
        StartCoroutine(IsClicked());
    }

    IEnumerator IsClicked()
    {
        yield return new WaitForSeconds(0.5f);

        _isClicked = false;
    }
}
