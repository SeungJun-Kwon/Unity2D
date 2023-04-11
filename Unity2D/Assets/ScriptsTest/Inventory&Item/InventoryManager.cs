using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

enum SelectedTab { Equipment = 0, Comsumption, Material }
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public GameObject _itemPrefab;

    public GameObject _slotPanel;
    public List<ItemSlot> _slots = null;

    [SerializeField] int _curIndex;
    public int CurIndex
    {
        get { return _curIndex; }
        set { _curIndex = value; }
    }

    List<string> _equipments = new List<string>();
    List<string> _consumptions = new List<string>();
    List<string> _materials = new List<string>();

    PlayerManager _playerManager;

    SelectedTab _curTab;
    public int CurTab
    {
        get
        {
            return (int)_curTab;
        }
        set
        {
            _curTab = (SelectedTab)value;
            LoadSlots();
        }
    }

    string _jsonPath = "Assets/Resources/Json";

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        Inventory inventory = NewtonsoftJson.Instance.LoadJsonFile<Inventory>(_jsonPath, "Inventory");

        _equipments = inventory._equipmentItemArr;
        _consumptions = inventory._consumptionItemArr;
        _materials = inventory._itemArr;

        _itemPrefab = Resources.Load("Prefabs/UI/ItemSlot") as GameObject;
        _slots = new List<ItemSlot>();
        for (int i = 0; i < inventory._capacity; i++)
        {
            var item = Instantiate(_itemPrefab, transform.Find("SlotPanel"));
            item.TryGetComponent(out ItemSlot itemSlot);
            _slots.Add(itemSlot);
        }

        CurIndex = -1;
    }

    private void Start()
    {
        CurTab = (int)SelectedTab.Equipment;

        if(_playerManager == null)
            GameObject.FindGameObjectWithTag("Player").TryGetComponent(out _playerManager);
    }

    public void AddItem(ItemSO item) {
        for(int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i].IsEmpty)
            {
                _slots[i].Item = item;
                if (item._type == ItemType.Equipment)
                    _equipments[i] = item._name;
                else if (item._type == ItemType.Consumption)
                    _consumptions[i] = item._name;
                else if (item._type == ItemType.Material)
                    _materials[i] = item._name;
                return;
            }
        }

        SaveInventoryForPlayer();
    }

    public void RemoveItem(ItemSlot slot)
    {
        int index = _slots.IndexOf(slot);
        if (slot.Item._type == ItemType.Equipment)
            _equipments[index] = "null";
        else if (slot.Item._type == ItemType.Consumption)
            _consumptions[index] = "null";
        else if (slot.Item._type == ItemType.Material)
            _materials[index] = "null";
        _slots[index].Item = null;

        SaveInventoryForPlayer();
    }

    public void MoveItem(ItemSlot slot)
    {
        if (CurIndex == -1)
            return;

        int selectedIndex = _slots.IndexOf(slot);

        _slots[CurIndex]._outline.enabled = false;
        slot._outline.enabled = false;

        if (_slots[CurIndex] == slot)
        {
            CurIndex = -1;
            return;
        }

        if(slot.IsEmpty)
        {
            if(CurTab == (int)SelectedTab.Equipment)
            {
                _equipments[selectedIndex] = _equipments[CurIndex];
                _equipments[CurIndex] = "null";
            }
            else if(CurTab == (int)SelectedTab.Comsumption)
            {
                _consumptions[selectedIndex] = _consumptions[CurIndex];
                _consumptions[CurIndex] = "null";
            }
            else if(CurTab == (int)SelectedTab.Material)
            {
                _materials[selectedIndex] = _materials[CurIndex];
                _materials[CurIndex] = "null";
            }
            slot.Item = _slots[CurIndex].Item;
            _slots[CurIndex].Item = null;
        }
        else
        {
            if (CurTab == (int)SelectedTab.Equipment)
            {
                string tmp = _equipments[selectedIndex];
                _equipments[selectedIndex] = _equipments[CurIndex];
                _equipments[CurIndex] = tmp;
            }
            else if (CurTab == (int)SelectedTab.Comsumption)
            {
                string tmp = _consumptions[selectedIndex];
                _consumptions[selectedIndex] = _consumptions[CurIndex];
                _consumptions[CurIndex] = tmp;
            }
            else if (CurTab == (int)SelectedTab.Material)
            {
                string tmp = _materials[selectedIndex];
                _materials[selectedIndex] = _materials[CurIndex];
                _materials[CurIndex] = tmp;
            }
            ItemSO item = slot.Item;
            slot.Item = _slots[CurIndex].Item;
            _slots[CurIndex].Item = item;
        }
        SaveInventoryForPlayer();

        CurIndex = -1;
    }

    public void SelectItem(ItemSlot slot)
    {
        CurIndex = _slots.IndexOf(slot);
        _slots[CurIndex]._outline.enabled = true;
    }

    public void SaveInventoryForPlayer() => _playerManager._inventory = new Inventory(_equipments, _consumptions, _materials);

    public Task SaveSlots()
    {
        Inventory inventory = new Inventory(_equipments, _consumptions, _materials);

        string json = NewtonsoftJson.Instance.ObjectToJson(inventory);
        NewtonsoftJson.Instance.SaveJsonFile(_jsonPath, "Inventory", json);

        return Task.CompletedTask;
    }

    public Task LoadSlots()
    {
        Inventory inventory = NewtonsoftJson.Instance.LoadJsonFile<Inventory>(_jsonPath, "Inventory");
        _equipments = inventory._equipmentItemArr;
        _consumptions = inventory._consumptionItemArr;
        _materials = inventory._itemArr;

        switch(CurTab)
        {
            case (int)SelectedTab.Equipment:
                for (int i = 0; i < _slots.Count; i++)
                {
                    if (_equipments[i] == "null")
                        _slots[i].Item = null;
                    else
                        _slots[i].Item = ItemDataManager.Instance._equipmentItems[_equipments[i]];
                }
                break;
            case (int)SelectedTab.Comsumption:
                for (int i = 0; i < _slots.Count; i++)
                {
                    if (_consumptions[i] == "null")
                        _slots[i].Item = null;
                    else
                        _slots[i].Item = ItemDataManager.Instance._consumptionItems[_consumptions[i]];
                }
                break;
            case (int)SelectedTab.Material:
                for (int i = 0; i < _slots.Count; i++)
                {
                    if (_materials[i] == "null")
                        _slots[i].Item = null;
                    else
                        _slots[i].Item = ItemDataManager.Instance._items[_materials[i]];
                }
                break;
        }

        return Task.CompletedTask;
    }

    public void DeleteAll()
    {
        foreach (var item in _slots)
            item.Item = null;
    }

    public async void UseItem(ItemSlot slot)
    {
        int index = _slots.IndexOf(slot);

        if (slot.Item._type == ItemType.Equipment)
        {
            EquipmentItemSO item = slot.Item as EquipmentItemSO;
            slot.Item = null;

            switch (item._part)
            {
                case EquipmentPart.Head:
                    if (_playerManager._equipment.HeadItem != null)
                        slot.Item = _playerManager._equipment.HeadItem;

                    _playerManager._equipment.HeadItem = item;
                    break;
                case EquipmentPart.Body:
                    if (_playerManager._equipment.BodyItem != null)
                        slot.Item = _playerManager._equipment.BodyItem;

                    _playerManager._equipment.BodyItem = item;
                    break;
                case EquipmentPart.Hand:
                    if (_playerManager._equipment.HandItem != null)
                        slot.Item = _playerManager._equipment.HandItem;

                    _playerManager._equipment.HandItem = item;
                    break;
                case EquipmentPart.Foot:
                    if (_playerManager._equipment.FootItem != null)
                        slot.Item = _playerManager._equipment.FootItem;

                    _playerManager._equipment.FootItem = item;
                    break;
                case EquipmentPart.Weapon:
                    if (_playerManager._equipment.WeaponItem != null)
                        slot.Item = _playerManager._equipment.WeaponItem;

                    _playerManager._equipment.WeaponItem = item;
                    break;
            }

            if(slot.IsEmpty)
                _equipments[index] = "null";
            else
                _equipments[index] = slot.Item._name;

            _playerManager._userInfo.UnequipItem(slot.Item as EquipmentItemSO);
            _playerManager._userInfo.EquipItem(item);
            await _playerManager.SaveItems();
        }

        CurIndex = -1;
    }

    public void SortSlots()
    {

    }

    private async void OnEnable()
    {
        await LoadSlots();
    }

    private async void OnDisable()
    {
        await SaveSlots();
    }
}
