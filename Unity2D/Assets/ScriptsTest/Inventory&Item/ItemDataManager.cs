using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class ItemDataManager
{
    private static ItemDataManager _instance;
    public static ItemDataManager Instance
    {
        get
        {
            if(_instance == null)
                _instance = new ItemDataManager();
            return _instance;
        }
    }

    ItemData _itemData;
    public Dictionary<string, ItemSO> _items = new Dictionary<string, ItemSO>();
    public Dictionary<string, EquipmentItemSO> _equipmentItems = new Dictionary<string, EquipmentItemSO>();
    public Dictionary<string, ConsumptionItemSO> _consumptionItems = new Dictionary<string, ConsumptionItemSO>();

    public void Init()
    {
        _itemData = NewtonsoftJson.Instance.LoadJsonFile<ItemData>("Assets/Resources/Json", "ItemData");
    }

    public Task CheckItemData()
    {
        object[] objArr = null;
        objArr = Resources.LoadAll("ScriptableObject/ItemData/Normal&Material");
        List<ItemSO> itemSOArr = new List<ItemSO>();
        foreach (var obj in objArr)
        {
            ItemSO item = obj as ItemSO;
            itemSOArr.Add(item);
        }

        objArr = Resources.LoadAll("ScriptableObject/ItemData/Equipment");
        List<EquipmentItemSO> equipmentItemSOArr = new List<EquipmentItemSO>();
        foreach (var obj in objArr)
        {
            EquipmentItemSO item = obj as EquipmentItemSO;
            equipmentItemSOArr.Add(item);
        }

        objArr = Resources.LoadAll("ScriptableObject/ItemData/Consumption");
        List<ConsumptionItemSO> consumptionItemSOArr = new List<ConsumptionItemSO>();
        foreach(var obj in objArr)
        {
            ConsumptionItemSO item = obj as ConsumptionItemSO;
            consumptionItemSOArr.Add(item);
        }

        if (itemSOArr != null && itemSOArr.Count > 0)
        {
            foreach (var item in itemSOArr)
                _items[item._name] = item;
        }
        if (equipmentItemSOArr != null && equipmentItemSOArr.Count > 0)
        {
            foreach (var item in equipmentItemSOArr)
                _equipmentItems[item._name] = item;
        }
        if (consumptionItemSOArr != null && consumptionItemSOArr.Count > 0)
        {
            foreach (var item in consumptionItemSOArr)
                _consumptionItems[item._name] = item;
        }

        foreach (var item in _itemData._itemArr)
        {
            if (!_items.TryGetValue(item.Name, out _))
            {
                ItemSO itemSO = ScriptableObject.CreateInstance<ItemSO>();
                itemSO.Init(item);
                AssetDatabase.CreateAsset(itemSO, "Assets/Resources/ScriptableObject/ItemData/Normal&Material/" + item.Name + ".asset");
                _items[item.Name] = itemSO;
            }
        }
        foreach(var item in _itemData._equipmentItemArr)
        {
            if(!_equipmentItems.TryGetValue(item.Name, out _))
            {
                EquipmentItemSO itemSO = ScriptableObject.CreateInstance<EquipmentItemSO>();
                itemSO.Init(item);
                AssetDatabase.CreateAsset(itemSO, "Assets/Resources/ScriptableObject/ItemData/Equipment/" + item.Name + ".asset");
                _items[item.Name] = itemSO;
            }
        }
        foreach(var item in _itemData._consumptionItemArr)
        {
            if(!_consumptionItems.TryGetValue(item.Name, out _))
            {
                ConsumptionItemSO itemSO = ScriptableObject.CreateInstance<ConsumptionItemSO>();
                itemSO.Init(item);
                AssetDatabase.CreateAsset(itemSO, "Assets/Resources/ScriptableObject/ItemData/Consumption/" + item.Name + ".asset");
                _items[item.Name] = itemSO;
            }
        }

        Debug.Log("ItemData 로드 및 저장 완료");
        return Task.CompletedTask;
    }
}
