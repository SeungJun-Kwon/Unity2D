using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UN = UnityEngine.Networking;

public class GoogleSheetManager : MonoBehaviour
{
    // export?format=tsv : ���� �������� ��Ʈ���� �ٿ�ε�(������ ���е� ��, tsv) ��ũ�� ����.
    // gid=���� : �ּ�â �� gid�� ���� ���ϴ� ��Ʈ�� ������ �� ����
    // range=���� : ���� ��Ʈ���� �ش� ������ŭ �������� ���� �� ���
    // const string URL = "https://docs.google.com/spreadsheets/d/1IHvwJ6izpC1nKNnle1Ha1Jwl2ZQj0ypUUsDFbwgaFp8/export?format=tsv&gid=1797689908&range=A2:B";

    // �̷��� �ϸ� �������� ��Ʈ�� �����͸� ���� �������� ���� �ǹǷ� �������� ��Ʈ�� �۽� ��ũ��Ʈ�� ���� �Լ��� �����͸� Get, Post �ϴ� ���� ����
    const string URL = "https://script.google.com/macros/s/AKfycbw2EsLFNhRLCBFdL4JbOXYNppiucQey8KftydlyTYMbZ-y2ZLfXvrYa6LGoD17oOcTZ/exec";

    private IEnumerator Start()
    {
        // �����͸� Post�� �� �����ִ� �Լ�. Key, Value�� �̷���� �ִ�.
        WWWForm form = new WWWForm();
        form.AddField("value", "��");

        UnityWebRequest www = UnityWebRequest.Post(URL, form);
        yield return www.SendWebRequest();

        string data = www.downloadHandler.text;
        print(data);
    }
}
