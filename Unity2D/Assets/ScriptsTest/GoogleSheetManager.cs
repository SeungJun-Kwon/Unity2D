using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UN = UnityEngine.Networking;

public class GoogleSheetManager : MonoBehaviour
{
    // export?format=tsv : 구글 스프레드 시트에서 다운로드(탭으로 구분된 값, tsv) 링크와 같음.
    // gid=숫자 : 주소창 끝 gid를 통해 원하는 시트를 가져올 수 있음
    // range=범위 : 구글 시트에서 해당 범위만큼 가져오고 싶을 때 사용
    // const string URL = "https://docs.google.com/spreadsheets/d/1IHvwJ6izpC1nKNnle1Ha1Jwl2ZQj0ypUUsDFbwgaFp8/export?format=tsv&gid=1797689908&range=A2:B";

    // 이렇게 하면 스프레드 시트의 데이터를 직접 가져오는 것이 되므로 스프레드 시트의 앱스 스크립트를 통해 함수로 데이터를 Get, Post 하는 것이 좋다
    const string URL = "https://script.google.com/macros/s/AKfycbw2EsLFNhRLCBFdL4JbOXYNppiucQey8KftydlyTYMbZ-y2ZLfXvrYa6LGoD17oOcTZ/exec";

    private IEnumerator Start()
    {
        // 데이터를 Post할 때 도와주는 함수. Key, Value로 이루어져 있다.
        WWWForm form = new WWWForm();
        form.AddField("value", "값");

        UnityWebRequest www = UnityWebRequest.Post(URL, form);
        yield return www.SendWebRequest();

        string data = www.downloadHandler.text;
        print(data);
    }
}
