using Firebase.Firestore;
using System;
using WebSocketSharp;

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

    [FirestoreProperty("moveSpeed")]
    public float MoveSpeed { get; set; }

    [FirestoreProperty("atk")]
    public int Atk { get; set; }

    [FirestoreProperty("def")]
    public int Def { get; set; }

    public UnitInfo()
    {
        Name = "";
        Lv = 0;
        Hp = 0;
        Mp = 0;
        MoveSpeed = 0;
        Atk = 0;
        Def = 0;
    }

    public UnitInfo(string name)
    {
        Name = name;
        Lv = 1;
        Hp = 50;
        Mp = 20;
        MoveSpeed = 100;
        Atk = 1;
        Def = 1;
    }

    public UnitInfo(string name, float moveSpeed, int atk, int def) : this(name)
    {
        MoveSpeed = moveSpeed;
        Atk = atk;
        Def = def;
    }

    public UnitInfo(string name, int lv, int hp, int mp, float moveSpeed, int atk, int def)
    {
        Name = name;
        Lv = lv;
        Hp = hp;
        Mp = mp;
        MoveSpeed = moveSpeed;
        Atk = atk;
        Def = def;
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
        return $"Name: {Name}, Lv: {Lv}, Hp: {Hp}, Mp: {Mp}, MoveSpeed: {MoveSpeed}, Atk: {Atk}, Def: {Def}";
    }
}


[Serializable]
[FirestoreData]
public class UserInfo : UnitInfo
{
    [FirestoreProperty("str")]
    public int Str { get; set; }

    [FirestoreProperty("dex")]
    public int Dex { get; set; }

    [FirestoreProperty("int")]
    public int Int { get; set; }

    [FirestoreProperty("luk")]
    public int Luk { get; set; }

    [FirestoreProperty("exp")]
    public int Exp { get; set; }

    public UserInfo() : base()
    {
        Str = 0;
        Dex = 0;
        Int = 0;
        Luk = 0;
        Exp = 0;
    }

    public UserInfo(string name) : base(name)
    {
        Str = 1;
        Dex = 1;
        Int = 1;
        Luk = 1;
        Exp = 0;
    }

    public UserInfo(string name, int lv, int hp, int mp, float moveSpeed, int atk, int def, int str, int dex, int _int, int luk, int exp)
        : base(name, lv, hp, mp, moveSpeed, atk, def)
    {
        Str = str;
        Dex = dex;
        Int = _int;
        Luk = luk;
        Exp = exp;
    }

    public override string ToString()
    {
        return $"Name: {Name}, Lv: {Lv}, Hp: {Hp}, Mp: {Mp}, MoveSpeed: {MoveSpeed}, Atk: {Atk}, Def: {Def}, Str: {Str}, Dex: {Dex}, Int: {Int}, Luk: {Luk}, Exp: {Exp}";
    }
}
