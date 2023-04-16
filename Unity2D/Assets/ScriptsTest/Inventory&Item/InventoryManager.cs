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

    public GrabbedSlot _grabbedSlot;

    public GameObject _tooltipPanel;
    ItemTooltip _itemTooltip;
    public ItemTooltip ItemTooltip
    {
        get
        {
            if (_itemTooltip == null)
                _tooltipPanel.TryGetComponent(out _itemTooltip);

            return _itemTooltip;
        }
    }

    public RectTransform _rect;

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

        TryGetComponent(out _rect);

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

        SavePlayerInventory();
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

        SavePlayerInventory();
    }

    public void SwapItem(ItemSlot slot1, ItemSlot slot2)
    {
        int index1 = _slots.IndexOf(slot1);
        int index2 = _slots.IndexOf(slot2);

        // 아이템 슬롯 두 개가 같은 경우
        if (index1 == index2)
            return;

        if (CurTab == (int)SelectedTab.Equipment)
        {
            string tmp = _equipments[index2];
            _equipments[index2] = _equipments[index1];
            _equipments[index1] = tmp;
        }
        else if (CurTab == (int)SelectedTab.Comsumption)
        {
            string tmp = _consumptions[index2];
            _consumptions[index2] = _consumptions[index1];
            _consumptions[index1] = tmp;
        }
        else if (CurTab == (int)SelectedTab.Material)
        {
            string tmp = _materials[index2];
            _materials[index2] = _materials[index1];
            _materials[index1] = tmp;
        }

        ItemSO tmpItem = null;
        if (slot2.Item != null)
            tmpItem = slot2.Item;
        slot2.Item = slot1.Item;
        slot1.Item = tmpItem;

        SavePlayerInventory();
    }


    public void SelectItem(ItemSlot slot)
    {
        CurIndex = _slots.IndexOf(slot);
        _slots[CurIndex]._outline.enabled = true;
    }

    public void SavePlayerInventory() => _playerManager._inventory = new Inventory(_equipments, _consumptions, _materials);

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
            _playerManager._inventory._equipmentItemArr = _equipments;
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
