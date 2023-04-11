using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TestItem : MonoBehaviour
{
    public TMP_Text _name;
    public TMP_Text _desc;
    public TMP_Text _lv;
    public TMP_InputField _input;
    public Image _image;

    public void Set()
    {
        string name = _input.text;
        if (name == null || name == "")
            return;

        ItemSO item = null;

        if(ItemDataManager.Instance._items.ContainsKey(name))
        {
            item = ItemDataManager.Instance._items[name];
        }
        else if(ItemDataManager.Instance._equipmentItems.ContainsKey(name))
        {
            item = ItemDataManager.Instance._equipmentItems[name];
        }
        else if(ItemDataManager.Instance._consumptionItems.ContainsKey(name))
        {
            item = ItemDataManager.Instance._consumptionItems[name];
        }

        _name.text = item._name;
        _desc.text = item._description;
        _lv.text = item._requiredLv.ToString();
        _image.sprite = item._icon;
    }
}
