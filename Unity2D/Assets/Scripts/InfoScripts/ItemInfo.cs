using Firebase.Firestore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { Null, Equipment, Consumption, Material }
public enum EquipmentPart { Weapon, Head, Body, Hand, Foot }

[Serializable]
[FirestoreData]
public class ItemInfo
{
    [SerializeField][JsonIgnore] private ItemType _type;
    [FirestoreProperty("type")]
    public string Type
    {
        get => _type.ToString();
        set => _type = (ItemType)Enum.Parse(typeof(ItemType), value);
    }

    [SerializeField][JsonIgnore] private string _name;
    [FirestoreProperty("name")]
    public string Name
    {
        get => _name;
        set => _name = value;
    }

    [SerializeField][JsonIgnore] private string _image;
    [FirestoreProperty("image")]
    public string Image
    {
        get => _image;
        set => _image = value;
    }

    [SerializeField][JsonIgnore] private string _description;
    [FirestoreProperty("description")]
    public string Description
    {
        get
        {
            return _description;
        }
        set
        {
            _description = value;
        }
    }

    [SerializeField][JsonIgnore] private int _requiredLv;
    [FirestoreProperty("requiredLv")]
    public int RequiredLv
    {
        get
        {
            return _requiredLv;
        }
        set
        {
            _requiredLv = value;
        }
    }

    public ItemInfo()
    {
        Type = ItemType.Null.ToString();
        Name = "null";
    }

    public ItemInfo(string type, string name, string image, string description, int requiredLv)
    {
        Type = type;
        Name = name;
        Image = image;
        Description = description;
        RequiredLv = requiredLv;
    }
}

[Serializable]
[FirestoreData]
public class EquipmentItemInfo : ItemInfo
{
    [SerializeField][JsonIgnore] private EquipmentPart _part;
    [FirestoreProperty("part")]
    public string Part
    {
        get => _part.ToString();
        set => _part = (EquipmentPart)Enum.Parse(typeof(EquipmentPart), value);
    }

    [SerializeField][JsonIgnore] private int _hp;
    [FirestoreProperty("hp")]
    public int Hp
    {
        get
        {
            return _hp;
        }
        set
        {
            _hp = value;
        }
    }

    [SerializeField][JsonIgnore] private int _mp;
    [FirestoreProperty("mp")]
    public int Mp
    {
        get
        {
            return _mp;
        }
        set
        {
            _mp = value;
        }
    }

    [SerializeField][JsonIgnore] private int _atk;
    [FirestoreProperty("atk")]
    public int Atk
    {
        get
        {
            return _atk;
        }
        set
        {
            _atk = value;
        }
    }

    [SerializeField][JsonIgnore] private int _def;
    [FirestoreProperty("def")]
    public int Def
    {
        get
        {
            return _def;
        }
        set
        {
            _def = value;
        }
    }

    [SerializeField][JsonIgnore] private float _moveSpeed;
    [FirestoreProperty("moveSpeed")]
    public float MoveSpeed
    {
        get
        {
            return _moveSpeed;
        }
        set
        {
            _moveSpeed = value;
        }
    }

    [SerializeField][JsonIgnore] private float _attackSpeed;
    [FirestoreProperty("attackSpeed")]
    public float AttackSpeed
    {
        get
        {
            return _attackSpeed;
        }
        set
        {
            _attackSpeed = value;
        }
    }

    public EquipmentItemInfo() : base()
    {

    }

    public EquipmentItemInfo(string type, string name, string image, string description, int requiredLv, string part, int hp, int mp, int atk, int def, float moveSpeed, float attackSpeed)
        : base(type, name, image, description, requiredLv)
    {
        Part = part;
        Hp = hp;
        Mp = mp;
        Atk = atk;
        Def = def;
        MoveSpeed = moveSpeed;
        AttackSpeed = attackSpeed;
    }
}

[Serializable]
[FirestoreData]
public class ConsumptionItemInfo : ItemInfo
{
    public ConsumptionItemInfo() : base()
    {

    }

    public ConsumptionItemInfo(string type, string name, string image, string description, int requiredLv) : base(type, name, image, description, requiredLv)
    {

    }
}

public class ItemData
{
    public List<ItemInfo> _itemArr = null;
    public List<EquipmentItemInfo> _equipmentItemArr = null;
    public List<ConsumptionItemInfo> _consumptionItemArr = null;

    public ItemData()
    {
        _itemArr = new List<ItemInfo>();
        _equipmentItemArr = new List<EquipmentItemInfo>();
        _consumptionItemArr = new List<ConsumptionItemInfo>();
    }

    public ItemData(List<ItemInfo> itemArr, List<EquipmentItemInfo> equipmentItemArr, List<ConsumptionItemInfo> consumptionItemArr)
    {
        if (itemArr == null || itemArr.Count == 0)
            itemArr = new List<ItemInfo>();
        _itemArr = itemArr;

        if (equipmentItemArr == null || equipmentItemArr.Count == 0)
            _equipmentItemArr = new List<EquipmentItemInfo>();
        _equipmentItemArr = equipmentItemArr;

        if (consumptionItemArr == null || consumptionItemArr.Count == 0)
            consumptionItemArr = new List<ConsumptionItemInfo>();
        _consumptionItemArr = consumptionItemArr;
    }
}

[Serializable]
[FirestoreData]
public class Equipment
{
    [FirestoreProperty]
    public string Head { get; set; }

    [FirestoreProperty]
    public string Body { get; set; }

    [FirestoreProperty]
    public string Hand { get; set; }

    [FirestoreProperty]
    public string Foot { get; set; }

    [FirestoreProperty]
    public string Weapon { get; set; }

    [JsonIgnore]
    EquipmentItemSO _headItem, _bodyItem, _handItem, _footItem, _weaponItem;

    [JsonIgnore]
    public EquipmentItemSO HeadItem
    {
        get
        {
            return _headItem;
        }
        set
        {
            _headItem = value;
            if (value == null)
                Head = "null";
            else
                Head = value._name;
        }
    }

    [JsonIgnore]
    public EquipmentItemSO BodyItem
    {
        get
        {
            return _bodyItem;
        }
        set
        {
            _bodyItem = value;
            if (value == null)
                Body = "null";
            else
                Body = value._name;
        }
    }

    [JsonIgnore]
    public EquipmentItemSO HandItem
    {
        get
        {
            return _handItem;
        }
        set
        {
            _handItem = value;
            if (value == null)
                Hand = "null";
            else
                Hand = value._name;
        }
    }

    [JsonIgnore]
    public EquipmentItemSO FootItem
    {
        get
        {
            return _footItem;
        }
        set
        {
            _footItem = value;
            if (value == null)
                Foot = "null";
            else
                Foot = value._name;
        }
    }

    [JsonIgnore]
    public EquipmentItemSO WeaponItem
    {
        get
        {
            return _weaponItem;
        }
        set
        {
            _weaponItem = value;
            if (value == null)
                Weapon = "null";
            else
                Weapon = value._name;
        }
    }

    public Equipment()
    {
        HeadItem = null;
        BodyItem = null;
        HandItem = null;
        FootItem = null;
        WeaponItem = null;
    }

    public Equipment(EquipmentItemSO head, EquipmentItemSO body, EquipmentItemSO hand, EquipmentItemSO foot, EquipmentItemSO weapon)
    {
        HeadItem = head;
        BodyItem = body;
        HandItem = hand;
        FootItem = foot;
        WeaponItem = weapon;
    }
}

public class Inventory
{
    public List<string> _equipmentItemArr = null;
    public List<string> _consumptionItemArr = null;
    public List<string> _itemArr = null;

    public int _capacity = 48;

    public Inventory()
    {
        _equipmentItemArr = new List<string>();
        _consumptionItemArr = new List<string>();
        _itemArr = new List<string>();
    }

    public Inventory(List<string> equipmentItemArr, List<string> consumptionItemArr, List<string> itemArr)
    {
        _equipmentItemArr = equipmentItemArr;
        _consumptionItemArr = consumptionItemArr;
        _itemArr = itemArr;
    }

    public bool AddItem<T>(T item)
    {
        int i = 0;
        if(item.GetType() == typeof(EquipmentItemSO))
        {
            for(i = 0; i < _capacity; i++)
            {
                if (_equipmentItemArr[i] != "null")
                    continue;                
                EquipmentItemSO equip = item as EquipmentItemSO;
                Debug.Log("ItemInfo -> AddItem");
                _equipmentItemArr[i] = equip._name;
                return true;
            }
        }
        else if(item.GetType() == typeof(ConsumptionItemSO))
        {
            for (i = 0; i < _capacity; i++)
            {
                if (_consumptionItemArr[i] != "" || _consumptionItemArr[i] != "null")
                    continue;

                ConsumptionItemSO consum = item as ConsumptionItemSO;
                _consumptionItemArr[i] = consum._name;
                return true;
            }
        }
        else
        {
            for (i = 0; i < _capacity; i++)
            {
                if (_itemArr[i] != "" || _itemArr[i] != "null")
                    continue;

                ItemSO material = item as ItemSO;
                _itemArr[i] = material._name;
                return true;
            }
        }

        return false;
    }
}