using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item")]
[Serializable]
public class ItemSO : ScriptableObject
{
    public string _name;
    public string _description;
    [JsonIgnore]
    public Sprite _icon;
}
