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
            var result = Resources.LoadAll<ItemSO>("ScriptableObject/ItemData");
            foreach (ItemSO item in result)
                Debug.Log($"{item._name} {item._description}");
        }
    }
}
