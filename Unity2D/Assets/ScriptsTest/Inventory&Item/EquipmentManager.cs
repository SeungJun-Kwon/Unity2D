using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance;

    [SerializeField] ItemSlot _head, _body, _hand, _foot, _weapon;
    [SerializeField] TMP_Text _nameText, _lvText, _hpText, _mpText, _atkText, _defText, _moveSpeedText, _attackSpeedText, _expText;

    PlayerManager _playerManager = null;

    string _name;
    int _lv, _hp, _mp, _atk, _def, _exp;
    float _moveSpeed, _attackSpeed;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        if(_playerManager == null)
            GameObject.FindGameObjectWithTag("Player").TryGetComponent(out _playerManager);

        SetValues();
    }

    private void OnEnable()
    {
        if(_playerManager != null)
            SetValues();
    }

    public void SetValues()
    {
        if (_playerManager._equipment.Head != "null")
            _head.Item = _playerManager._equipment.HeadItem;
        else
            _head.Item = null;

        if (_playerManager._equipment.Body != "null")
            _body.Item = _playerManager._equipment.BodyItem;
        else
            _body.Item = null;

        if (_playerManager._equipment.Hand != "null")
            _hand.Item = _playerManager._equipment.HandItem;
        else
            _hand.Item = null;

        if (_playerManager._equipment.Foot != "null")
            _foot.Item = _playerManager._equipment.FootItem;
        else
            _foot.Item = null;

        if (_playerManager._equipment.Weapon != "null")
            _weapon.Item = _playerManager._equipment.WeaponItem;
        else
            _weapon.Item = null;

        _nameText.text = _playerManager._userInfo.Name;
        _lvText.text = _playerManager._userInfo.Lv.ToString();
        _hpText.text = _playerManager._userInfo.Hp.ToString();
        _mpText.text = _playerManager._userInfo.Mp.ToString();
        _atkText.text = _playerManager._userInfo.Atk.ToString();
        _defText.text = _playerManager._userInfo.Def.ToString();
        _moveSpeedText.text = _playerManager._userInfo.MoveSpeed.ToString();
        _attackSpeedText.text = _playerManager._userInfo.AttackSpeed.ToString();
        _expText.text = _playerManager._userInfo.Exp.ToString();
    }

    public bool IsEquip(ItemSO item)
    {
        if (item == null)
            return false;

        if (_head.Item._name == item._name || _body.Item._name == item._name || _hand.Item._name == item._name || _foot.Item._name == item._name || _weapon.Item._name == item._name)
            return true;

        return false;
    }

    public async void UnEquipItem(EquipmentItemSO item)
    {
        Debug.Log("EquipmentManager -> UnEquipItem");
        if (!_playerManager._inventory.AddItem(item))
            return;

        switch (item._part)
        {
            case EquipmentPart.Head:
                _playerManager._equipment.HeadItem = null;
                break;
            case EquipmentPart.Body:
                _playerManager._equipment.BodyItem = null;
                break;
            case EquipmentPart.Hand:
                _playerManager._equipment.HandItem = null;
                break;
            case EquipmentPart.Foot:
                _playerManager._equipment.FootItem = null;
                break;
            case EquipmentPart.Weapon:
                _playerManager._equipment.WeaponItem = null;
                break;
        }

        _playerManager._userInfo.UnequipItem(item);
        await _playerManager.SaveItems();

        SetValues();
    }
}
