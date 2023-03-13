using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FirebaseReatimeDatabaseManager
{
    private static FirebaseReatimeDatabaseManager instance;
    public static FirebaseReatimeDatabaseManager Instance
    {
        get
        {
            if(instance == null)
                instance = new FirebaseReatimeDatabaseManager();

            return instance;
        }
    }

    private DatabaseReference _userDBRef = null;

    static string _userDBUrl = "https://userdb-6f753-default-rtdb.firebaseio.com/";

    public void Init()
    {
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new System.Uri(_userDBUrl);
        _userDBRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void CreateUserInfo(FirebaseUser user)
    {
        _userDBRef.Child("users").OrderByChild("userID").EqualTo(user.Email).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to read data: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;

            // �ش� ID�� ����ϴ� ����ڰ� �̹� �����ϴ� ���
            if (snapshot.ChildrenCount > 0)
            {
                Debug.LogWarning("UnitInfo ID already exists.");
            }
            // �ش� ID�� ����ϴ� ����ڰ� �������� �ʴ� ���
            else
            {
                // ���ο� ����� ������ ���� �� ����
                string key = user.UserId;
                UnitInfo userInfo = new UnitInfo(user.Email);
                string json = JsonUtility.ToJson(userInfo);
                _userDBRef.Child("users").Child(key).SetRawJsonValueAsync(json);
                Debug.Log("New user added.");
            }
        });
    }

    public void UpdateUserInfo(FirebaseUser user, UnitInfo userInfo)
    {
        _userDBRef.Child("users").OrderByChild("userID").EqualTo(user.Email).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log("Failed to save user : " + task.Exception);
                return;
            }

            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if(snapshot != null && snapshot.Value != null)
                {
                    string key = user.UserId;
                    string json = JsonUtility.ToJson(userInfo);
                    _userDBRef.Child("users").Child(key).SetRawJsonValueAsync(json);
                    Debug.Log("User Data Updated");
                }
            }
        });
    }

    public void LoadUserInfo(FirebaseUser user, System.Action<UnitInfo> onComplete)
    {
        _userDBRef.Child("users").Child(user.UserId).GetValueAsync().ContinueWith(task =>
        {
            if(task.IsFaulted || task.IsCanceled)
            {
                Debug.Log("Failed to load user : " + task.Exception);
                return;
            }

            if(task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot != null && snapshot.Value != null)
                {
                    string json = snapshot.GetRawJsonValue();
                    UnitInfo userInfo = JsonUtility.FromJson<UnitInfo>(json);
                    onComplete(userInfo);
                }
                else
                    onComplete(null);
            }
        });
    }
}
