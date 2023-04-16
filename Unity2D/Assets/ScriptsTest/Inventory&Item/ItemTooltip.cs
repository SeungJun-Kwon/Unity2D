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
            string effectText = "효과 = ";

            if (_item._type == ItemType.Equipment)
            {
                _type.text = "장비 아이템";
                EquipmentItemSO item = _item as EquipmentItemSO;

                if (item._part == EquipmentPart.Head)
                {
                    _type.text += "(투구)";
                    effectText += $"HP {item._hp}, 방어력 {item._def} 증가";
                }
                else if (item._part == EquipmentPart.Body)
                {
                    _type.text += "(갑옷)";
                    effectText += $"MP {item._mp}, 방어력 {item._def} 증가";
                }
                else if (item._part == EquipmentPart.Hand)
                {
                    _type.text += "(장갑)";
                    effectText += $"HP {item._hp}, 공격력 {item._atk}, 방어력 {item._def} 증가";
                }
                else if (item._part == EquipmentPart.Foot)
                {
                    _type.text += "(신발)";
                    effectText += $"MP {item._mp}, 방어력 {item._def}, 이동속도 {item._moveSpeed}% 증가";
                }
                else if (item._part == EquipmentPart.Weapon)
                {
                    _type.text += "(무기)";
                    effectText += $"공격력 {item._atk}, 방어력 {item._def}, 공격속도 {item._attackSpeed}% 증가";
                }
            }
            else if(_item._type == ItemType.Consumption)
            {
                _type.text = "소비 아이템";
            }
            else if(_item._type == ItemType.Material)
            {
                _type.text = "기타 아이템";
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
