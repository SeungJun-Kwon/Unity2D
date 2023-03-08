using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using System;
using System.Threading.Tasks;

public class UserInfo
{
    public string userName;
    public int userLv;
    public float userHp;
    public float userMp;

    public UserInfo(string name)
    {
        userName = name;
        userLv = 1;
        userHp = 50;
        userMp = 20;
    }

    public UserInfo(string userName, int userLv, float userHp, float userMp)
    {
        this.userName = userName;
        this.userLv = userLv;
        this.userHp = userHp;
        this.userMp = userMp;
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

    public void CreateUserInfo(string userId, UserInfo userInfo)
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
                Debug.LogWarning("UserInfo ID already exists.");
            }
            // 해당 ID를 사용하는 사용자가 존재하지 않는 경우
            else
            {
                // 새로운 사용자 데이터 생성 및 저장
                Dictionary<string, object> data = new Dictionary<string, object>
                {
                    {"userName", userInfo.userName},
                    {"userLv", userInfo.userLv},
                    {"userHp", userInfo.userHp},
                    {"userMp", userInfo.userMp}
                };
                _userStore.Collection("users").Document(userId).SetAsync(data);
                Debug.Log("New user added.");
            }
        });
    }

    public void UpdateUserInfo(FirebaseUser user, UserInfo userInfo)
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
                        {"userName", userInfo.userName},
                        {"userLv", userInfo.userLv},
                        {"userHp", userInfo.userHp},
                        {"userMp", userInfo.userMp}
                    };
                    _userStore.Collection("users").Document(key).SetAsync(data);
                    Debug.Log("User Data Updated");
                }
            }
        });
    }

    public void LoadUserInfo(FirebaseUser user, System.Action<UserInfo> onComplete)
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

                    UserInfo userInfo = new UserInfo(userName, userLv, userHp, userMp);
                    onComplete(userInfo);
                }
                else
                    onComplete(null);
            }
        });
    }

    public async Task<UserInfo> LoadUserInfo(FirebaseUser user)
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

                return new UserInfo(userName, userLv, userHp, userMp);
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
