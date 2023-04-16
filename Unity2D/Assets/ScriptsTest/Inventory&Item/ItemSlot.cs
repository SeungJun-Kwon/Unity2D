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
            {
                _slotImg.sprite = value._icon;
                _slotImg.color = new Color(255, 255, 255, 255);
            }
            else
                _slotImg.color = new Color(255, 255, 255, 0);
        }
    }

    public Image _slotImg;
    public Sprite _emptyImg;
    public Outline _outline;

    RectTransform _rect;

    public bool IsEmpty
    {
        get { return Item == null; }
    }

    private void Awake()
    {
        TryGetComponent(out _rect);
    }

    private void Start()
    {
        _slotImg.raycastTarget = false;
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
                Debug.Log("ItemSlot -> UnEquipItem");
                EquipmentManager.Instance.UnEquipItem(Item as EquipmentItemSO);
                _outline.enabled = false;
                return;
            }
            return;
        }

        if (InventoryManager.Instance != null && InventoryManager.Instance.gameObject.activeSelf)
        {
            if (InventoryManager.Instance._grabbedSlot.gameObject.activeSelf)
            {
                // �κ��丮 â���� ���� ������ ����Ŭ�� ���� ��
                // -> ������ ���(����, �Ҹ�ǰ ��)
                if (GrabbedSlot.instance._curSlot == this && GrabbedSlot.instance._isClicked)
                {
                    InventoryManager.Instance.UseItem(this);
                    GrabbedSlot.instance.gameObject.SetActive(false);
                    if (InventoryManager.Instance._tooltipPanel.activeSelf)
                        InventoryManager.Instance._tooltipPanel.SetActive(false);
                    return;
                }

                InventoryManager.Instance.SwapItem(GrabbedSlot.instance._curSlot, this);
                GrabbedSlot.instance.gameObject.SetActive(false);
            }
            else if (!IsEmpty)
            {
                InventoryManager.Instance._grabbedSlot.gameObject.SetActive(true);
                InventoryManager.Instance._grabbedSlot.SetItem(this);
            }

            if (!IsEmpty && InventoryManager.Instance.CurIndex == -1)
            {
                //InventoryManager.Instance.SelectItem(this);
            }
            else
            {
                //InventoryManager.Instance.MoveItem(this);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _outline.enabled = true;

        // �κ��丮�� �������� �� ���콺�� Enter�� ��� ���� ǥ��
        if (InventoryManager.Instance != null && InventoryManager.Instance.gameObject.activeSelf == true && !IsEmpty)
        {
            InventoryManager.Instance._tooltipPanel.SetActive(true);
            InventoryManager.Instance.ItemTooltip.VisibleTooltip(this, _rect);
        }
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
            if (!IsEmpty && InventoryManager.Instance._tooltipPanel.activeSelf)
                InventoryManager.Instance._tooltipPanel.SetActive(false);
            if (InventoryManager.Instance.CurIndex != -1 && InventoryManager.Instance._slots[InventoryManager.Instance.CurIndex] != this)
                _outline.enabled = false;
            else if (InventoryManager.Instance.CurIndex == -1)
                _outline.enabled = false;
        }
    }
}
