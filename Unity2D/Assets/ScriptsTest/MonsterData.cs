using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "MonsterData")]
public class MonsterData : ScriptableObject
{
    public int _level;
    public string _name;
    public int _hp;
    public int _mp;
    public int[] _damage = new int[2] {0, 0};
    public string _objectPath;
    public float _spawnTime;
}
