using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MonsterSpawnData
{
    public MonsterData _monster;
    public Vector2 _position;
}

[CreateAssetMenu(fileName = "MapData", menuName = "MapData")]
public class MapData : ScriptableObject
{
    public string _name;
    public List<MonsterSpawnData> _monsterSpawn = new List<MonsterSpawnData>();
}
