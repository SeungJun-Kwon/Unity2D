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
        // �ٸ� ���̾�̽� ������Ʈ�� �������� ���� �� �ɼ�
        app = new AppOptions
        {
            ProjectId = "unity2dgamedata",
            StorageBucket = "unity2dgamedata.appspot.com"
        };
        // ����� �� �ɼ����� ���̾�̽� ���� ����(�Է� �����͸� ���� ���̾�̽����� �������� ��)
        fapp = FirebaseApp.Create(app, "Unity2DGameData");
        // ���̾�̽� ���� ���� ���̾�̽� ���� ������
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

        // ������ �ٿ�ε��� ��ġ�� �����մϴ�.
        string folderPath = Application.dataPath + "/DownTest";
        string filePath = Path.Combine(folderPath, "Download.jpg");

        // ������ �ٿ�ε��մϴ�.
        using (UnityWebRequest webRequest = UnityWebRequest.Get(task.Result))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // �ٿ�ε��� ������ �����մϴ�.
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