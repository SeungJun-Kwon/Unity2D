using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public bool IsCurSlotEmpty() => _slots[CurIndex].IsEmpty;

    public void SaveSlots()
    {

    }

    public void LoadSlots()
    {

    }

    public void SortSlots()
    {

    }
}
