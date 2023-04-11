using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentItem", menuName = "Item/EquipmentItem")]
[Serializable]
public class EquipmentItemSO : ItemSO
{
    public EquipmentPart _part;
    public int _hp;
    public int _mp;
    public int _atk;
    public int _def;
    public float _moveSpeed;
    public float _attackSpeed;

    public override void Init(ItemInfo info)
    {
        base.Init(info);

        EquipmentItemInfo info1 = info as EquipmentItemInfo;

        _part = Enum.Parse<EquipmentPart>(info1.Part);
        _hp = info1.Hp;
        _mp = info1.Mp;
        _atk = info1.Atk;
        _def = info1.Def;
        _moveSpeed = info1.MoveSpeed;
        _attackSpeed = info1.AttackSpeed;
    }

    public override void Print()
    {
        base.Print();

        Debug.Log($"Part : {_part.ToString()}\nHp : {_hp}\nMp : {_mp}\nAtk : {_atk}\n Def : {_def}\nMoveSpeed : {_moveSpeed}\nAttackSpeed : {_attackSpeed}");
    }
}