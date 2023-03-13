using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using System;
using System.Threading.Tasks;

public class UnitInfo
{
    public string name;
    public int lv;
    public float hp;
    public float mp;

    public UnitInfo(string name)
    {
        this.name = name;
        lv = 1;
        hp = 50;
        mp = 20;
    }

    public UnitInfo(string name, int lv, float hp, float mp)
    {
        this.name = name;
        this.lv = lv;
        this.hp = hp;
        this.mp = mp;
    }
}

public class FirebaseFirestoreManager
{
    private static FirebaseFirestoreManager instance;
    public static FirebaseFirestoreManager Instance
    {
        get
        {
            if (instance == null)
                instance = new FirebaseFirestoreManager();

            return instance;
        }
    }

    private FirebaseFirestore _userStore = null;

    public void Init()
    {
        _userStore = FirebaseFirestore.DefaultInstance;
    }

    public void CreateUserInfo(string userId, UnitInfo userInfo)
    {
        _userStore.Collection("users").Document(userId).GetSnapshotAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to read data: " + task.Exception);
                return;
            }

            DocumentSnapshot snapshot = task.Result;

            // 해당 ID를 사용하는 사용자가 이미 존재하는 경우
            if (snapshot.Exists)
            {
                Debug.LogWarning("UnitInfo ID already exists.");
            }
            // 해당 ID를 사용하는 사용자가 존재하지 않는 경우
            else
            {
                // 새로운 사용자 데이터 생성 및 저장
                Dictionary<string, object> data = new Dictionary<string, object>
                {
                    {"userName", userInfo.name},
                    {"userLv", userInfo.lv},
                    {"userHp", userInfo.hp},
                    {"userMp", userInfo.mp}
                };
                _userStore.Collection("users").Document(userId).SetAsync(data);
                Debug.Log("New user added.");
            }
        });
    }

    public void UpdateUserInfo(FirebaseUser user, UnitInfo userInfo)
    {
        _userStore.Collection("users").Document(user.UserId).GetSnapshotAsync().ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log("Failed to save user : " + task.Exception);
                return;
            }

            if (task.IsCompleted)
            {
                DocumentSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    string key = user.UserId;
                    Dictionary<string, object> data = new Dictionary<string, object>
                    {
                        {"userName", userInfo.name},
                        {"userLv", userInfo.lv},
                        {"userHp", userInfo.hp},
                        {"userMp", userInfo.mp}
                    };
                    _userStore.Collection("users").Document(key).SetAsync(data);
                    Debug.Log("User Data Updated");
                }
            }
        });
    }

    public void LoadUserInfo(FirebaseUser user, System.Action<UnitInfo> onComplete)
    {
        _userStore.Collection("users").Document(user.UserId).GetSnapshotAsync().ContinueWith(task =>
        {
            if(task.IsCanceled || task.IsFaulted)
            {
                Debug.Log("Failed to load user : " + task.Exception);
                return;
            }

            if (task.IsCompleted)
            {

                DocumentSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    string userName = snapshot.GetValue<string>("userName");
                    int userLv = snapshot.GetValue<int>("userLv");
                    float userHp = snapshot.GetValue<float>("userHp");
                    float userMp = snapshot.GetValue<float>("userMp");

                    UnitInfo userInfo = new UnitInfo(userName, userLv, userHp, userMp);
                    onComplete(userInfo);
                }
                else
                    onComplete(null);
            }
        });
    }

    public async Task<UnitInfo> LoadUserInfo(FirebaseUser user)
    {
        try
        {
            var result = await _userStore.Collection("users").Document(user.UserId).GetSnapshotAsync();
            if (result.Exists)
            {
                string userName = result.GetValue<string>("userName");
                int userLv = result.GetValue<int>("userLv");
                float userHp = result.GetValue<float>("userHp");
                float userMp = result.GetValue<float>("userMp");

                return new UnitInfo(userName, userLv, userHp, userMp);
            }
            else
                return null;
        }
        catch(FirestoreException e)
        {
            Debug.Log($"유저 데이터 로드 실패 : {e.Message}");
            return null;
        }
    }
}
