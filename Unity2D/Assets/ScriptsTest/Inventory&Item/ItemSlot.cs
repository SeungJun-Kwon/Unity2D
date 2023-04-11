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

    private void OnEnable()
    {
        _outline.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (EquipmentManager.Instance != null && EquipmentManager.Instance.gameObject.activeSelf && !IsEmpty)
        {
            if (IsEmpty)
                return;

            // ��� â���� �������� ����Ŭ�� ���� ��
            // -> ������ ����
            if (eventData.clickCount == 2)
            {
                EquipmentManager.Instance.UnEquipItem(Item as EquipmentItemSO);
                _outline.enabled = false;
                return;
            }
            return;
        }

        if (InventoryManager.Instance != null && InventoryManager.Instance.gameObject.activeSelf)
        {
            // �κ��丮 â���� �������� ����Ŭ�� ���� ��
            // -> ������ ���(����, �Ҹ�ǰ ��)
            if (!IsEmpty && eventData.clickCount >= 2)
            {
                InventoryManager.Instance.UseItem(this);
                _outline.enabled = false;
                return;
            }

            if (!IsEmpty && InventoryManager.Instance.CurIndex == -1)
            {
                InventoryManager.Instance.SelectItem(this);
            }
            else
            {
                InventoryManager.Instance.MoveItem(this);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _outline.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (EquipmentManager.Instance != null && EquipmentManager.Instance.gameObject.activeSelf == true)
        {
            _outline.enabled = false;
            return;
        }

        if(InventoryManager.Instance != null && InventoryManager.Instance.gameObject.activeSelf == true)
        {
            if (InventoryManager.Instance.CurIndex != -1 && InventoryManager.Instance._slots[InventoryManager.Instance.CurIndex] != this)
                _outline.enabled = false;
            else if (InventoryManager.Instance.CurIndex == -1)
                _outline.enabled = false;
        }
    }
}
