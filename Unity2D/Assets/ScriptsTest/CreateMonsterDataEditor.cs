using Firebase.Firestore;
using UnityEditor;
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

    [MenuItem("Window/CreateGameData/Monster")]
    public static void ShoWindow()
    {
        EditorWindow.GetWindow(typeof(CreateMonsterDataEditor));
    }

    private void OnGUI()
    {
        name = EditorGUILayout.TextField("Name", name);
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
        EditorGUILayout.Space();
        if (GUILayout.Button("Create") && CheckInputFields())
        {
            UnitInfo unitInfo = new UnitInfo(name, lv, hp, mp, moveSpeed, atk, def);
            CreateUnit(unitInfo);

        }
    }

    async void CreateUnit(UnitInfo unitInfo)
    {
        if(await FirebaseFirestoreManager.Instance.CreateUnit(unitInfo))
        {
            Debug.Log($"{unitInfo.Name}이(가) 추가되었습니다.");
            Close();
        }
    }

    bool CheckInputFields()
    {
        if (name == "" || lv == 0 || hp == 0 || atk == 0 || def == 0 || moveSpeed == 0)
        {
            Debug.Log("비어있는 칸 혹은 값이 0인 칸이 있습니다.");
            return false;
        }
        else if (name.Length < 2 || name.Length > 12)
        {
            Debug.Log("이름의 길이가 너무 짧거나 깁니다.(2자 이상 12자 이하)");
            return false;
        }
        else if (lv > 999)
        {
            Debug.Log("레벨 값이 너무 큽니다.(999 이하)");
            return false;
        }
        else if (moveSpeed < 50 || moveSpeed > 200)
        {
            Debug.Log("이동속도가 너무 느리거나 빠릅니다.(50 이상 200 이하)");
            return false;
        }
        return true;
    }
}
