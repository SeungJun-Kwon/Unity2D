using Firebase.Auth;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerManager : UnitManager
{
    public UserInfo _userInfo;
    public Inventory _inventory;
    public Equipment _equipment;

    string _jsonPath = "Assets/Resources/Json";

    [PunRPC]
    public async void LoadUserData(string email)
    {
        _info = await FirebaseFirestoreManager.Instance.LoadUserInfo(email);
        _userInfo = _info as UserInfo;

        CurHp = _userInfo.Hp;
        CurMp = _userInfo.Mp;

        _inventory = await FirebaseFirestoreManager.Instance.LoadUserInventory(email);
        string json = NewtonsoftJson.Instance.ObjectToJson(_inventory);
        NewtonsoftJson.Instance.SaveJsonFile(_jsonPath, "Inventory", json);

        _equipment = await FirebaseFirestoreManager.Instance.LoadUserEquipment(email);
        json = NewtonsoftJson.Instance.ObjectToJson(_equipment);
        NewtonsoftJson.Instance.SaveJsonFile(_jsonPath, "Equipment", json);

        if (ItemDataManager.Instance._equipmentItems.TryGetValue(_equipment.Head, out var head))
            _equipment.HeadItem = head;
        else
            _equipment.HeadItem = null;
        if (ItemDataManager.Instance._equipmentItems.TryGetValue(_equipment.Body, out var body))
            _equipment.BodyItem = body;
        else
            _equipment.BodyItem = null;
        if (ItemDataManager.Instance._equipmentItems.TryGetValue(_equipment.Hand, out var hand))
            _equipment.HandItem = hand;
        else
            _equipment.HandItem = null;
        if (ItemDataManager.Instance._equipmentItems.TryGetValue(_equipment.Foot, out var foot))
            _equipment.FootItem = foot;
        else
            _equipment.FootItem = null;
        if (ItemDataManager.Instance._equipmentItems.TryGetValue(_equipment.Weapon, out var weapon))
            _equipment.WeaponItem = weapon;
        else
            _equipment.WeaponItem = null;

        _userInfo.EquipItem(_equipment.HeadItem);
        _userInfo.EquipItem(_equipment.BodyItem);
        _userInfo.EquipItem(_equipment.HandItem);
        _userInfo.EquipItem(_equipment.FootItem);
        _userInfo.EquipItem(_equipment.WeaponItem);
    }

    public Task SaveItems()
    {
        string json = NewtonsoftJson.Instance.ObjectToJson(_equipment);
        NewtonsoftJson.Instance.SaveJsonFile(_jsonPath, "Equipment", json);
        json = NewtonsoftJson.Instance.ObjectToJson(_inventory);
        NewtonsoftJson.Instance.SaveJsonFile(_jsonPath, "Inventory", json);

        return Task.CompletedTask;
    }

    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {

        }
        else if(stream.IsReading)
        {

        }
    }

    private async void OnApplicationQuit()
    {
        _userInfo.UnequipItem(_equipment.HeadItem);
        _userInfo.UnequipItem(_equipment.BodyItem);
        _userInfo.UnequipItem(_equipment.HandItem);
        _userInfo.UnequipItem(_equipment.FootItem);
        _userInfo.UnequipItem(_equipment.WeaponItem);

        await SaveItems();
        FirebaseFirestoreManager.Instance.UpdateUserInfo(FirebaseAuthManager.Instance._user, _userInfo);
        FirebaseFirestoreManager.Instance.UpdateUserInventory(FirebaseAuthManager.Instance._user, _inventory);
        FirebaseFirestoreManager.Instance.UpdateUserEquipment(FirebaseAuthManager.Instance._user, _equipment);
    }
}
