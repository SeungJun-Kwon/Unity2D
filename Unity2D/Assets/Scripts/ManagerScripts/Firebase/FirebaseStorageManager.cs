using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Storage;
using Firebase;
using Firebase.Extensions;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FirebaseStorageManager : MonoBehaviour
{
    public static FirebaseStorageManager Instance;

    FirebaseStorage _gameDataStorage;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if(Instance != this)
            Destroy(Instance);

        AppOptions app;
        FirebaseApp fapp;
        // 다른 파이어베이스 프로젝트를 가져오기 위한 앱 옵션
        app = new AppOptions
        {
            ProjectId = "unity2dgamedata",
            StorageBucket = "unity2dgamedata.appspot.com"
        };
        // 방금의 앱 옵션으로 파이어베이스 앱을 만듦(입력 데이터를 통해 파이어베이스에서 가져오는 것)
        fapp = FirebaseApp.Create(app, "Unity2DGameData");
        // 파이어베이스 앱을 통해 파이어베이스 스토어를 가져옴
        _gameDataStorage = FirebaseStorage.GetInstance(fapp, "gs://unity2dgamedata.appspot.com");
    }

    public IEnumerator Test()
    {
        var task = _gameDataStorage.RootReference.Child("Test.jpg").GetDownloadUrlAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted || task.IsCanceled)
        {
            Debug.Log(task.Exception);
            yield break;
        }

        // 파일을 다운로드할 위치를 지정합니다.
        string folderPath = Application.dataPath + "/DownTest";
        string filePath = Path.Combine(folderPath, "Download.jpg");

        // 파일을 다운로드합니다.
        using (UnityWebRequest webRequest = UnityWebRequest.Get(task.Result))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // 다운로드한 파일을 저장합니다.
                File.WriteAllBytes(filePath, webRequest.downloadHandler.data);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(webRequest.downloadHandler.data);

                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                TryGetComponent<Image>(out var image);
                image.sprite = sprite;

                Debug.Log("Downloaded file : " + filePath);
            }
            else
            {
                Debug.LogError("Failed to download file : " + webRequest.error);
            }
        }
    }

}