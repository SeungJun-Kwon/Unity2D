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
}