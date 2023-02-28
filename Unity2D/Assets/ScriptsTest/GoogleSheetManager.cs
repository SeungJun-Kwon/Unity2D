using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UN = UnityEngine.Networking;

public class GoogleSheetManager : MonoBehaviour
{
    public static GoogleSheetManager instance;

    // export?format=tsv : ���� �������� ��Ʈ���� �ٿ�ε�(������ ���е� ��, tsv) ��ũ�� ����.
    // gid=���� : �ּ�â �� gid�� ���� ���ϴ� ��Ʈ�� ������ �� ����
    // range=���� : ���� ��Ʈ���� �ش� ������ŭ �������� ���� �� ���
    // const string URL = "https://docs.google.com/spreadsheets/d/1IHvwJ6izpC1nKNnle1Ha1Jwl2ZQj0ypUUsDFbwgaFp8/export?format=tsv&gid=1797689908&range=A2:B";

    // �̷��� �ϸ� �������� ��Ʈ�� �����͸� ���� �������� ���� �ǹǷ� �������� ��Ʈ�� �۽� ��ũ��Ʈ�� ���� �Լ��� �����͸� Get, Post �ϴ� ���� ����
    const string URL = "https://script.google.com/macros/s/AKfycbw2EsLFNhRLCBFdL4JbOXYNppiucQey8KftydlyTYMbZ-y2ZLfXvrYa6LGoD17oOcTZ/exec";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
            Destroy(gameObject);
    }

    public IEnumerator Post(WWWForm form)
    {
        // using�� ����ϴ� ������ �̰� ������ ������ �ƿ� ����� �� �� ���� �ִٰ� �Ѵ�.
        using (UnityWebRequest www = UnityWebRequest.Post(URL, form))
        {
            yield return www.SendWebRequest();

            if (www.isDone)
                print(www.downloadHandler.text);
            else
                print("���� ������ �����ϴ�.");

            www.Dispose();
        }
    }

    private void OnApplicationQuit()
    {
        WWWForm form = new WWWForm();
        form.AddField("order", "signout");

        StartCoroutine(Post(form));
    }
}
