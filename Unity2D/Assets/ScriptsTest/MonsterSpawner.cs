using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviourPunCallbacks
{
    public MapData _curMap;

    List<MonsterSpawnData> _monsterData;
    List<GameObject> _objects = new List<GameObject>();

    bool _isFirstSpawn = false;

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient && !_isFirstSpawn)
        {
            photonView.RPC("SetFirstSpawn", RpcTarget.OthersBuffered);
            photonView.RPC("SpawnMonsters", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void SetFirstSpawn() => _isFirstSpawn = true;

    [PunRPC]
    public void SpawnMonsters()
    {
        if (!_isFirstSpawn)
        {
            _monsterData = _curMap._monsterSpawn;

            for (int i = 0; i < _monsterData.Count; i++)
            {
                MonsterData monsterData = _monsterData[i]._monster;
                GameObject monsterObject = PhotonNetwork.Instantiate(monsterData._objectPath, _monsterData[i]._position, Quaternion.identity);
                monsterObject.transform.SetParent(transform);
                monsterObject.transform.name = monsterData.name + (i + 1).ToString();
                _objects.Add(monsterObject);
            }
        }
    }
}
