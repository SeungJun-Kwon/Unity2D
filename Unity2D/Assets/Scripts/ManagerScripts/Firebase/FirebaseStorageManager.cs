using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Storage;
using Firebase;
using System.IO;
using UnityEngine.UI;
using System.Threading.Tasks;

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
}