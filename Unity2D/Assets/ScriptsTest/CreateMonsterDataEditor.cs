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
            Debug.Log($"{unitInfo.Name}��(��) �߰��Ǿ����ϴ�.");
            Close();
        }
    }

    bool CheckInputFields()
    {
        if (name == "" || lv == 0 || hp == 0 || atk == 0 || def == 0 || moveSpeed == 0)
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
        else if (moveSpeed < 50 || moveSpeed > 200)
        {
            Debug.Log("�̵��ӵ��� �ʹ� �����ų� �����ϴ�.(50 �̻� 200 ����)");
            return false;
        }
        return true;
    }
}
