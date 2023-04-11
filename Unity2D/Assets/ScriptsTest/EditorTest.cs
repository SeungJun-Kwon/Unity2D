using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorTest : EditorWindow
{
    static EditorWindow window;

    public new string name = "";
    public int lv = 0;
    public int hp = 0;
    public int mp = 0;
    public int atk = 0;
    public int def = 0;
    public float moveSpeed = 0;
    public float attackSpeed = 0;
    public int gold = 0;
    public int exp = 0;
    public List<string> dropItems = new List<string>();

    [MenuItem("Window/CreateGameData/Test")]
    public static void ShowWindow()
    {
        window = EditorWindow.GetWindow(typeof(EditorTest));
        window.minSize = new Vector2(300f, 250f);
        window.maxSize = new Vector2(300f, 500f);
    }

    private void OnGUI()
    {
        name = EditorGUILayout.TextField("Name", name);

        if (GUILayout.Button("Search"))
        {
            LoadEnemyInfo();
        }

        lv = EditorGUILayout.IntField("Level", lv);
        hp = EditorGUILayout.IntField("Hp", hp);
        mp = EditorGUILayout.IntField("Mp", mp);
        atk = EditorGUILayout.IntField("Atk", atk);
        def = EditorGUILayout.IntField("Def", def);
        moveSpeed = EditorGUILayout.FloatField("MoveSpeed", moveSpeed);
        gold = EditorGUILayout.IntField("Drop Gold", gold);
        exp = EditorGUILayout.IntField("Drop Exp", exp);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Drop Items");
        if (dropItems.Count > 0)
        {
            for (int i = 0; i < dropItems.Count; i++)
                EditorGUILayout.LabelField((i + 1).ToString(), dropItems[i]);
        }
        else
            EditorGUILayout.LabelField("Null");
        EditorGUILayout.Space();
    }

    async void LoadEnemyInfo()
    {
        var result = await FirebaseFirestoreManager.Instance.LoadEnemyInfo(name);

        if (result == null)
        {
            Debug.Log($"{name} 몬스터를 찾을 수 없습니다.");
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
        dropItems = result.DropItems;
    }
}
