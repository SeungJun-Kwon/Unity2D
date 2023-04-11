using Firebase.Firestore;
using Mono.Cecil.Cil;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class CreateItemDataEditor : EditorWindow
{
    public ItemType type;
    public new string name;
    public string description;
    public int requiredLv;
    public Sprite image;

    public EquipmentPart part;
    public int hp;
    public int mp;
    public int atk;
    public int def;
    public float moveSpeed;
    public float attackSpeed;

    static EditorWindow window;

    [MenuItem("Window/CreateGameData/Item")]
    public static void ShoWindow()
    {
        window = EditorWindow.GetWindow(typeof(CreateItemDataEditor));
        window.minSize = new Vector2(300f, 250f);
        window.maxSize = new Vector2(300f, 500f);
    }

    void Init()
    {
        name = "";
        type = ItemType.Null;
        description = "";
        requiredLv = 0;
        image = null;
        part = EquipmentPart.Weapon;
        hp = 0;
        mp = 0;
        atk = 0;
        def = 0;
        moveSpeed = 0f;
        attackSpeed = 0f;
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        name = EditorGUILayout.TextField("Name", name);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Search Item") && name != "")
        {
            SearchItem(name);
        }
        if(GUILayout.Button("Clear"))
        {
            Init();
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        EditorGUILayout.Space();
        type = (ItemType)EditorGUILayout.EnumPopup("ItemType : ", type);
        EditorGUILayout.Space();
        description = EditorGUILayout.TextField("Description", description);
        EditorGUILayout.Space();
        requiredLv = EditorGUILayout.IntField("Required Lv", requiredLv);
        EditorGUILayout.Space();
        image = (Sprite)EditorGUILayout.ObjectField("Image : ", image, typeof(Sprite), false);
        EditorGUILayout.Space();

        if(type == ItemType.Equipment)
        {
            part = (EquipmentPart)EditorGUILayout.EnumPopup("EquipmentPart : ", part);
            EditorGUILayout.Space();
            hp = EditorGUILayout.IntField("HP", hp);
            EditorGUILayout.Space();
            mp = EditorGUILayout.IntField("MP", mp);
            EditorGUILayout.Space();
            atk = EditorGUILayout.IntField("ATK", atk);
            EditorGUILayout.Space();
            def = EditorGUILayout.IntField("DEF", def);
            EditorGUILayout.Space();
            moveSpeed = EditorGUILayout.FloatField("MoveSpeed", moveSpeed);
            EditorGUILayout.Space();
            attackSpeed = EditorGUILayout.FloatField("AttackSpeed", attackSpeed);
            EditorGUILayout.Space();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        if (GUILayout.Button("Create") && CheckInputFields())
        {
            if (type == ItemType.Null || type == ItemType.Material)
            {
                ItemInfo itemInfo = new ItemInfo(type.ToString(), name, image.name, description, requiredLv);
                CreateItem(itemInfo);
            }
            else if(type == ItemType.Equipment)
            {
                EquipmentItemInfo itemInfo = new EquipmentItemInfo(type.ToString(), name, image.name, description, requiredLv, part.ToString(), hp, mp, atk, def, moveSpeed, attackSpeed);
                CreateItem(itemInfo);
            }
            else if(type == ItemType.Consumption)
            {
                ConsumptionItemInfo itemInfo = new ConsumptionItemInfo(type.ToString(), name, image.name, description, requiredLv);
                CreateItem(itemInfo);
            }
        }
    }

    async void CreateItem<T>(T itemInfo)
    {
        if(await FirebaseFirestoreManager.Instance.CreateItem(itemInfo, name))
        {
            Debug.Log($"{name}��(��) �߰��Ǿ����ϴ�.");
            Close();
        }
    }

    async void SearchItem(string name)
    {
        var result = await FirebaseFirestoreManager.Instance.LoadItemInfo(name);
        if(result == null)
        {
            Debug.Log($"{name} �������� �������� �ʽ��ϴ�.");
            return;
        }

        type = (ItemType)Enum.Parse(typeof(ItemType), result.Type);
        description = result.Description;
        requiredLv = result.RequiredLv;

        image = Resources.Load<Sprite>("Sprites/ItemSprites/" + result.Image);
        //image.name = result.Image;

        if(type == ItemType.Equipment)
        {
            EquipmentItemInfo itemInfo = result as EquipmentItemInfo;
            part = (EquipmentPart)Enum.Parse(typeof(EquipmentPart), itemInfo.Part);
            hp = itemInfo.Hp;
            mp = itemInfo.Mp;
            atk = itemInfo.Atk;
            def = itemInfo.Def;
            moveSpeed = itemInfo.MoveSpeed;
            attackSpeed = itemInfo.AttackSpeed;
        }
    }

    bool CheckInputFields()
    {
        if (name == "" || description == "" || requiredLv == 0 || image == null)
        {
            Debug.Log("����ִ� ĭ Ȥ�� ���� 0�� ĭ�� �ֽ��ϴ�.");
            return false;
        }
        else if (name.Length < 2 || name.Length > 12)
        {
            Debug.Log("�̸��� ���̰� �ʹ� ª�ų� ��ϴ�.(2�� �̻� 12�� ����)");
            return false;
        }
        else if (requiredLv > 999)
        {
            Debug.Log("���� ���� �ʹ� Ů�ϴ�.(999 ����)");
            return false;
        }
        if(type == ItemType.Equipment)
        {
            if(moveSpeed >= 500 || attackSpeed >= 500)
            {
                Debug.Log("�ӵ� ���� �ʹ� Ů�ϴ�.(500 �̸�)");
                return false;
            }
        }

        return true;
    }
}
