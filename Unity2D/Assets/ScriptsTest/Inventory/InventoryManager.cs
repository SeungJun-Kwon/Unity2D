using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Equitment
{
    public string _head;
    public string _body;
    public string _hand;
    public string _foot;
    public string _weapon;

    public Equitment()
    {
        _head = null;
        _body = null;
        _hand = null;
        _foot = null;
        _weapon = null;
    }

    public Equitment(string head, string body, string hand, string foot, string weapon)
    {
        _head = head;
        _body = body;
        _hand = hand;
        _foot = foot;
        _weapon = weapon;
    }
}

public class Inventory
{
    public List<string> _itemArr = null;

    public Inventory()
    {
        _itemArr = new List<string>();
    }

    public Inventory(List<string> itemArr)
    {
        _itemArr = itemArr;
    }
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public ItemSlot _itemPrefab;

    public GameObject _slotPanel;
    public List<ItemSlot> _slots = new List<ItemSlot>();

    [SerializeField] int _curIndex;
    public int CurIndex
    {
        get { return _curIndex; }
        set { _curIndex = value; }
    }

    [SerializeField] List<ItemSO> cacheTest = new List<ItemSO>();
    Dictionary<string, ItemSO> _itemCache = new Dictionary<string, ItemSO>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(this);

        CurIndex = -1;
    }

    public void AddItem(ItemSO item) {
        for(int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i].IsEmpty)
            {
                _slots[i].Item = item;
                return;
            }
        }
    }

    public void RemoveItem(ItemSlot slot)
    {
        int index = _slots.IndexOf(slot);
        _slots[index].Item = null;
    }

    public void MoveItem(ItemSlot slot)
    {
        _slots[CurIndex]._outline.enabled = false;
        slot._outline.enabled = false;

        if (_slots[CurIndex] == slot)
        {
            CurIndex = -1;
            return;
        }

        if(slot.IsEmpty)
        {
            slot.Item = _slots[CurIndex].Item;
            _slots[CurIndex].Item = null;
        }
        else
        {
            ItemSO item = slot.Item;
            slot.Item = _slots[CurIndex].Item;
            _slots[CurIndex].Item = item;
        }

        CurIndex = -1;
    }

    public void SelectItem(ItemSlot slot)
    {
        CurIndex = _slots.IndexOf(slot);
        _slots[CurIndex]._outline.enabled = true;
    }

    public void SaveSlots()
    {
        Inventory inventory = new Inventory();

        foreach(ItemSlot slot in _slots)
        {
            if(slot.Item != null)
                inventory._itemArr.Add(slot.Item._name);
            else
                inventory._itemArr.Add("null");
        }

        string json = NewtonsoftJson.Instance.ObjectToJson(inventory);
        NewtonsoftJson.Instance.SaveJsonFile("Assets/Resources/Json/", "Inventory", json);
    }

    public void LoadSlots()
    {
        Inventory inventory = NewtonsoftJson.Instance.LoadJsonFile<Inventory>("Assets/Resources/Json/", "Inventory");

        for (int i = 0; i < _slots.Count; i++)
        {
            if (inventory._itemArr[i] == "null")
                _slots[i].Item = null;
            else
            {
                if (_itemCache.TryGetValue(inventory._itemArr[i], out var value))
                    _slots[i].Item = value;
                else
                {
                    ItemSO itemSO = Resources.Load("ScriptableObject/ItemData/" + inventory._itemArr[i]) as ItemSO;
                    _itemCache[inventory._itemArr[i]] = itemSO;
                    cacheTest.Add(itemSO);
                    _slots[i].Item = itemSO;
                }
            }
        }
    }

    public void DeleteAll()
    {
        foreach (var item in _slots)
            item.Item = null;
    }

    public void SortSlots()
    {

    }
}
