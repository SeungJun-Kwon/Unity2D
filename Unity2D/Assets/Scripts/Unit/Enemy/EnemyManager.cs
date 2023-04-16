using Firebase.Auth;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : UnitManager
{
    public EnemyInfo _enemyInfo;

    List<ItemSO> _dropItems = new List<ItemSO>();

    string _itemdataPath = "ScriptableObject/ItemData/";

    public void SetEnemyInfo(EnemyInfo enemyInfo)
    {
        _enemyInfo = enemyInfo;

        if (_enemyInfo.DropItems.Count > 0)
        {
            foreach (var item in _enemyInfo.DropItems)
            {
                ItemSO result = Resources.Load<EquipmentItemSO>(_itemdataPath + "Equipment/" + item);
                if(result != null)
                {
                    _dropItems.Add(result);
                    continue;
                }

                result = Resources.Load<ConsumptionItemSO>(_itemdataPath + "Consumption/" + item);
                if (result != null)
                {
                    _dropItems.Add(result);
                    continue;
                }

                result = Resources.Load<ItemSO>(_itemdataPath + "Normal&Material/" + item);
                _dropItems.Add(result);
            }
        }
    }

    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}
