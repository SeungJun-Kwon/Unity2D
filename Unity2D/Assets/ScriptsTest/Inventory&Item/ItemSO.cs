using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item")]
[Serializable]
public class ItemSO : ScriptableObject
{
    [JsonIgnore]
    public Sprite _icon;

    public ItemType _type;
    public string _name;
    public string _description;
    public int _requiredLv;

    public virtual void Init(ItemInfo info)
    {
        _type = Enum.Parse<ItemType>(info.Type);
        _name = info.Name;
        _description = info.Description;
        _requiredLv = info.RequiredLv;

        Sprite image = Resources.Load<Sprite>("Sprites/ItemSprites/" + info.Image);
        _icon = image;
    }

    public virtual void Print()
    {
        Debug.Log($"Type : {_type.ToString()}\nName : {_name}\nDescription : {_description}\nRequiredLv : {_requiredLv}");
    }
}