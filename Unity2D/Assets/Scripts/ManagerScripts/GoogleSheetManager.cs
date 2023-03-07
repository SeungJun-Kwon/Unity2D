using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UN = UnityEngine.Networking;

public class GoogleSheetManager : MonoBehaviour
{
    public static GoogleSheetManager instance;

    // export?format=tsv : 구글 스프레드 시트에서 다운로드(탭으로 구분된 값, tsv) 링크와 같음.
    // gid=숫자 : 주소창 끝 gid를 통해 원하는 시트를 가져올 수 있음
    // range=범위 : 구글 시트에서 해당 범위만큼 가져오고 싶을 때 사용
    // const string URL = "https://docs.google.com/spreadsheets/d/1IHvwJ6izpC1nKNnle1Ha1Jwl2ZQj0ypUUsDFbwgaFp8/export?format=tsv&gid=1797689908&range=A2:B";

    // 이렇게 하면 스프레드 시트의 데이터를 직접 가져오는 것이 되므로 스프레드 시트의 앱스 스크립트를 통해 함수로 데이터를 Get, Post 하는 것이 좋다
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
        // using을 사용하는 이유는 이걸 해주지 않으면 아예 통신이 안 될 때가 있다고 한다.
        using (UnityWebRequest www = UnityWebRequest.Post(URL, form))
        {
            yield return www.SendWebRequest();

            if (www.isDone)
                print(www.downloadHandler.text);
            else
                print("서버 응답이 없습니다.");

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
