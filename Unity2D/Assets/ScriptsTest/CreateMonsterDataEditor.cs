using Firebase.Firestore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class CreateMonsterDataEditor : EditorWindow
{
    public new string name;
    public int lv;
    public int hp;
    public int mp;
    public int atk;
    public int def;
    public float moveSpeed;
    public int gold;
    public int exp;
    string[] options = null;
    List<ItemInfo> items = new List<ItemInfo>();
    List<int> selectedItems = new List<int>();

    static EditorWindow window;

    [MenuItem("Window/CreateGameData/Monster")]
    public static void ShoWindow()
    {
        window = EditorWindow.GetWindow(typeof(CreateMonsterDataEditor));
        window.minSize = new Vector2(300f, 250f);
        window.maxSize = new Vector2(300f, 500f);
    }

    void Init()
    {
        name = "";
        lv = 0;
        hp = 0;
        mp = 0;
        atk = 0;
        def = 0;
        moveSpeed = 0f;
        gold = 0;
        exp = 0;
        items = new List<ItemInfo>();
        selectedItems = new List<int>();
    }

    private void OnGUI()
    {
        LoadItemData();

        if (options != null)
        {
            GUILayout.BeginVertical();
            name = EditorGUILayout.TextField("Name", name);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Search Enemy") && name != "")
            {
                SearchEnemy(name);
            }
            if (GUILayout.Button("Clear"))
            {
                Init();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            EditorGUILayout.Space();
            lv = EditorGUILayout.IntField("Level", lv);
            EditorGUILayout.Space();
            hp = EditorGUILayout.IntField("Hp", hp);
            EditorGUILayout.Space();
            mp = EditorGUILayout.IntField("Mp", mp);
            EditorGUILayout.Space();
            atk = EditorGUILayout.IntField("Atk", atk);
            EditorGUILayout.Space();
            def = EditorGUILayout.IntField("Def", def);
            EditorGUILayout.Space();
            moveSpeed = EditorGUILayout.FloatField("Move Speed", moveSpeed);
            EditorGUILayout.Space();
            gold = EditorGUILayout.IntField("Drop Gold", gold);
            EditorGUILayout.Space();
            exp = EditorGUILayout.IntField("Drop Exp", exp);
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
                selectedItems.Add(0);
            if (GUILayout.Button("-") && selectedItems.Count > 0)
                selectedItems.RemoveAt(selectedItems.Count - 1);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            for (int i = 0; i < selectedItems.Count; i++)
                selectedItems[i] = EditorGUILayout.Popup("Item", selectedItems[i], options);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (GUILayout.Button("Create") && CheckInputFields())
            {
                List<string> items = new List<string>();
                foreach (var index in selectedItems)
                    items.Add(options[index]);
                EnemyInfo enemyInfo = new EnemyInfo(name, lv, hp, mp, atk, def, moveSpeed, gold, exp, items);
                CreateEnemy(enemyInfo);

            }
        }
    }

    async void CreateEnemy(EnemyInfo enemyInfo)
    {
        if(await FirebaseFirestoreManager.Instance.CreateEnemy(enemyInfo))
        {
            Debug.Log($"{enemyInfo.Name}��(��) �߰��Ǿ����ϴ�.");
            Close();
        }
    }

    async void SearchEnemy(string name)
    {
        var result = await FirebaseFirestoreManager.Instance.LoadEnemyInfo(name);

        if (result == null)
        {
            Debug.Log($"{name} ���͸� ã�� �� �����ϴ�.");
            return;
        }

        lv = result.Lv;
        hp = result.Hp;
        mp = result.Mp;
        atk = result.Atk;
        def = result.Def;
        moveSpeed = result.MoveSpeed;
        gold = result.Gold;
        exp = result.Exp;
        foreach (var item in result.DropItems)
            selectedItems.Add(Array.IndexOf(options, item));
    }

    bool CheckInputFields()
    {
        if (name == "" || lv == 0 || hp == 0 || atk == 0)
        {
            Debug.Log("����ִ� ĭ Ȥ�� ���� 0�� ĭ�� �ֽ��ϴ�.");
            return false;
        }
        else if (name.Length < 2 || name.Length > 12)
        {
            Debug.Log("�̸��� ���̰� �ʹ� ª�ų� ��ϴ�.(2�� �̻� 12�� ����)");
            return false;
        }
        else if (lv > 999)
        {
            Debug.Log("���� ���� �ʹ� Ů�ϴ�.(999 ����)");
            return false;
        }
        else if (moveSpeed < -50 || moveSpeed > 200)
        {
            Debug.Log("�̵��ӵ��� �ʹ� �����ų� �����ϴ�.(-50 �̻� 200 ����)");
            return false;
        }
        return true;
    }

    async void LoadItemData()
    {
        items = await FirebaseFirestoreManager.Instance.LoadAllItemInfo();
        options = new string[items.Count];
        for (int i = 0; i < items.Count; i++)
            options[i] = items[i].Name;
    }
}
