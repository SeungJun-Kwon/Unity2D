using Firebase.Auth;
using Firebase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using System;

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

    public void CreateUserInfo(FirebaseUser user)
    {
        _userStore.Collection("users").Document(user.UserId).GetSnapshotAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to read data: " + task.Exception);
                return;
            }

            DocumentSnapshot snapshot = task.Result;

            // �ش� ID�� ����ϴ� ����ڰ� �̹� �����ϴ� ���
            if (snapshot.Exists)
            {
                Debug.LogWarning("UserInfo ID already exists.");
            }
            // �ش� ID�� ����ϴ� ����ڰ� �������� �ʴ� ���
            else
            {
                // ���ο� ����� ������ ���� �� ����
                string key = user.UserId;
                UserInfo userInfo = new UserInfo(user.Email);
                Dictionary<string, object> data = new Dictionary<string, object>
                {
                    {"userName", userInfo.userName},
                    {"userLv", userInfo.userLv},
                    {"userHp", userInfo.userHp},
                    {"userMp", userInfo.userMp}
                };
                _userStore.Collection("users").Document(key).SetAsync(data);
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
}
