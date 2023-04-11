using Firebase.Firestore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

[Serializable]
// 클래스를 파이어스토어에 매핑하기 위해 사용한다
[FirestoreData]
public class UnitInfo
{
    // 변수를 파이어스토어에 매핑하기 위해 사용한다
    // 파이어스토어의 필드명과 실제 변수명이 다를 경우 매핑할 때 에러가 발생하므로 파이어스토어의 필드 명을 입력해준다
    [FirestoreProperty("name")]
    public string Name { get; set; }

    [FirestoreProperty("lv")]
    public int Lv { get; set; }

    [FirestoreProperty("hp")]
    public int Hp { get; set; }

    [FirestoreProperty("mp")]
    public int Mp { get; set; }

    [FirestoreProperty("atk")]
    public int Atk { get; set; }

    [FirestoreProperty("def")]
    public int Def { get; set; }

    [FirestoreProperty("moveSpeed")]
    public float MoveSpeed { get; set; }

    public UnitInfo()
    {
        Name = "";
        Lv = 0;
        Hp = 0;
        Mp = 0;
        Atk = 0;
        Def = 0;
        MoveSpeed = 0;
    }

    public UnitInfo(string name)
    {
        Name = name;
        Lv = 1;
        Hp = 50;
        Mp = 20;
        Atk = 1;
        Def = 1;
        MoveSpeed = 100;
    }

    public UnitInfo(string name, float moveSpeed, int atk, int def) : this(name)
    {
        MoveSpeed = moveSpeed;
        Atk = atk;
        Def = def;
    }

    public UnitInfo(string name, int lv, int hp, int mp, int atk, int def, float moveSpeed)
    {
        Name = name;
        Lv = lv;
        Hp = hp;
        Mp = mp;
        Atk = atk;
        Def = def;
        MoveSpeed = moveSpeed;
    }

    public void ModifyValue(string fieldName, float value)
    {
        var field = this.GetType().GetField(fieldName);
        if (field != null)
        {
            Type fieldType = field.FieldType;
            if (fieldType == typeof(float))
            {
                float currentValue = (float)field.GetValue(this);
                field.SetValue(this, currentValue + value);
            }
            else if (fieldType == typeof(int))
            {
                int currentValue = (int)field.GetValue(this);
                field.SetValue(this, currentValue + (int)value);
            }
        }
    }

    public override string ToString()
    {
        return $"Name: {Name}, Lv: {Lv}, Hp: {Hp}, Mp: {Mp}, Atk: {Atk}, Def: {Def}, MoveSpeed: {MoveSpeed}";
    }
}

[Serializable]
[FirestoreData]
public class UserInfo : UnitInfo
{
    [FirestoreProperty("attakSpeed")]
    public float AttackSpeed { get; set; }

    [FirestoreProperty("exp")]
    public int Exp { get; set; }

    public UserInfo() : base()
    {
        AttackSpeed = 100;
        Exp = 0;
    }

    public UserInfo(string name) : base(name)
    {
        AttackSpeed = 100;
        Exp = 0;
    }

    public UserInfo(string name, int lv, int hp, int mp, int atk, int def, float moveSpeed, float attackSpeed, int exp)
        : base(name, lv, hp, mp, atk, def, moveSpeed)
    {
        AttackSpeed = attackSpeed;
        Exp = exp;
    }

    public void EquipItem(EquipmentItemSO item)
    {
        if (item == null)
            return;

        Hp += item._hp;
        Mp += item._mp;
        Atk += item._atk;
        Def += item._def;
        MoveSpeed += item._moveSpeed;
        AttackSpeed += item._attackSpeed;
    }

    public void UnequipItem(EquipmentItemSO item)
    {
        if (item == null)
            return;

        Hp -= item._hp;
        Mp -= item._mp;
        Atk -= item._atk;
        Def -= item._def;
        MoveSpeed -= item._moveSpeed;
        AttackSpeed -= item._attackSpeed;
    }

    public override string ToString()
    {
        return $"Name: {Name}, Lv: {Lv}, Hp: {Hp}, Mp: {Mp}, Atk: {Atk}, Def: {Def}, MoveSpeed: {MoveSpeed}, AttackSpeed: {AttackSpeed}, Exp: {Exp}";
    }
}

[Serializable]
[FirestoreData]
public class EnemyInfo : UnitInfo
{
    [FirestoreProperty("gold")]
    public int Gold { get; set; }

    [FirestoreProperty("exp")]
    public int Exp { get; set; }

    [FirestoreProperty("dropItems")]
    public List<string> DropItems { get; set; }

    public EnemyInfo() : base()
    {
        Gold = 0;
        Exp = 0;
        DropItems = new List<string>() { "null" };
    }

    public EnemyInfo(string name) : base(name)
    {
        Gold = 0;
        Exp = 0;
        DropItems = new List<string>() { "null" };
    }

    public EnemyInfo(string name, float moveSpeed, int atk, int def) : base(name, moveSpeed, atk, def)
    {
        Gold = 0;
        Exp = 0;
        DropItems = new List<string>() { "null" };
    }

    public EnemyInfo(string name, int lv, int hp, int mp, int atk, int def, float moveSpeed) : base(name, lv, hp, mp, atk, def, moveSpeed)
    {
        Gold = 0;
        Exp = 0;
        DropItems = new List<string>() { "null" };
    }

    public EnemyInfo(string name, int lv, int hp, int mp, int atk, int def, float moveSpeed, int gold, int exp, List<string> dropItems) : base(name, lv, hp, mp, atk, def, moveSpeed)
    {
        Gold = gold;
        Exp = exp;
        DropItems = dropItems;
    }
}