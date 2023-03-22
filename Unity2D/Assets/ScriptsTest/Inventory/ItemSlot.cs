using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ItemSO _item;
    public ItemSO Item
    {
        get { return _item; }
        set
        {
            _item = value;
            if (value != null)
                _slotImg.sprite = value._icon;
            else
                _slotImg.sprite = _emptyImg;
        }
    }

    public Image _slotImg;
    public Sprite _emptyImg;
    public Outline _outline;

    public bool IsEmpty
    {
        get { return Item == null; }
    }

    private void Start()
    {
        _outline.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount >= 2)
        {
            if (!IsEmpty)
                Debug.Log($"{Item._name} : {Item._description}");
        }

        if (InventoryManager.Instance.CurIndex == -1)
        {
            if (IsEmpty)
                return;
            InventoryManager.Instance.SelectItem(this);
        }
        else
        {
            InventoryManager.Instance.MoveItem(this);
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _outline.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _outline.enabled = false;
    }
}
