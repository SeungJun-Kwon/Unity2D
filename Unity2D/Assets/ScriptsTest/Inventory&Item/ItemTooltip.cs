using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    public TMP_Text _name;
    public Image _icon;
    public TMP_Text _desc;
    public TMP_Text _type;
    public TMP_Text _effect;

    ItemSlot _curSlot = null;
    ItemSO _item = null;
    public ItemSO Item
    {
        get { return _item; }
        set
        {
            _item = value;
            _name.text = value._name;
            _icon.sprite = value._icon;
            _desc.text = value._description;
            string effectText = "ȿ�� = ";

            if (_item._type == ItemType.Equipment)
            {
                _type.text = "��� ������";
                EquipmentItemSO item = _item as EquipmentItemSO;

                if (item._part == EquipmentPart.Head)
                {
                    _type.text += "(����)";
                    effectText += $"HP {item._hp}, ���� {item._def} ����";
                }
                else if (item._part == EquipmentPart.Body)
                {
                    _type.text += "(����)";
                    effectText += $"MP {item._mp}, ���� {item._def} ����";
                }
                else if (item._part == EquipmentPart.Hand)
                {
                    _type.text += "(�尩)";
                    effectText += $"HP {item._hp}, ���ݷ� {item._atk}, ���� {item._def} ����";
                }
                else if (item._part == EquipmentPart.Foot)
                {
                    _type.text += "(�Ź�)";
                    effectText += $"MP {item._mp}, ���� {item._def}, �̵��ӵ� {item._moveSpeed}% ����";
                }
                else if (item._part == EquipmentPart.Weapon)
                {
                    _type.text += "(����)";
                    effectText += $"���ݷ� {item._atk}, ���� {item._def}, ���ݼӵ� {item._attackSpeed}% ����";
                }
            }
            else if(_item._type == ItemType.Consumption)
            {
                _type.text = "�Һ� ������";
            }
            else if(_item._type == ItemType.Material)
            {
                _type.text = "��Ÿ ������";
                effectText = "";
            }

            _effect.text = effectText;
        }
    }

    RectTransform _transform;

    private void Awake()
    {
        _transform = GetComponent<RectTransform>();
    }

    public void VisibleTooltip(ItemSlot slot, RectTransform rect)
    {
        _transform.localPosition = rect.localPosition + new Vector3(50f, 0);
        _curSlot = slot;
        Item = _curSlot.Item;
    }
}
