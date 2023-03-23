using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using System;
using System.Threading.Tasks;
using Firebase;

public class FirebaseFirestoreManager
{
    private static FirebaseFirestoreManager instance;
    public static FirebaseFirestoreManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new FirebaseFirestoreManager();
                instance.Init();
            }

            return instance;
        }
    }

    private FirebaseFirestore _userStore = null;
    FirebaseFirestore _gameDataStore = null;

    string _userInfo = "userInfo";
    string _enemyInfo = "enemyInfo";

    public void Init()
    {
        _userStore = FirebaseFirestore.DefaultInstance;
        AppOptions app;
        FirebaseApp fapp;
        // 다른 파이어베이스 프로젝트를 가져오기 위한 앱 옵션
        app = new AppOptions
        {
            ProjectId = "unity2dgamedata",
            StorageBucket = "unity2dgamedata.appspot.com"
        };
        // 방금의 앱 옵션으로 파이어베이스 앱을 만듦(입력 데이터를 통해 파이어베이스에서 가져오는 것
        fapp = FirebaseApp.Create(app, "Unity2DGameData");
        // 파이어베이스 앱을 통해 파이어베이스 스토어를 가져옴
        _gameDataStore = FirebaseFirestore.GetInstance(fapp);
    }

    public void CreateUser(string userEmail, UserInfo userInfo)
    {
        _userStore.Collection(_userInfo).Document(userEmail).GetSnapshotAsync().ContinueWith(task =>
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
                //Dictionary<string, object> data = new Dictionary<string, object>
                //{
                //    {"name", userInfo.Name},
                //    {"lv", userInfo.Lv},
                //    {"hp", userInfo.Hp},
                //    {"mp", userInfo.Mp},
                //    {"exp", userInfo.Exp},
                //    {"moveSpeed", userInfo.MoveSpeed},
                //    {"atk", userInfo.Atk},
                //    {"def", userInfo.Def},
                //    {"str", userInfo.Str},
                //    {"dex", userInfo.Dex},
                //    {"int", userInfo.Int},
                //    {"luk", userInfo.Luk}
                //};
                //_userStore.Collection(_userInfo).Document(userEmail).SetAsync(data);
                _userStore.Collection(_userInfo).Document(userEmail).SetAsync(userInfo);
                //Dictionary<string, object> equipDic = new Dictionary<string, object>
                //{
                //    {"head", "a" },
                //    {"body", "b" }
                //};
                //Dictionary<string, object> invenDic = new Dictionary<string, object>
                //{
                //    {"1", "a" },
                //    {"2", "b" }
                //};
                //_userStore.Collection(_userInfo).Document(userEmail).Collection("Item").Document("Equipment").SetAsync(equipDic);
                //_userStore.Collection(_userInfo).Document(userEmail).Collection("Item").Document("Inventory").SetAsync(invenDic);
                Debug.Log("New user added.");
            }
        });
    }

    public async Task<bool> CreateUnit(UnitInfo unitInfo)
    {
        try
        {
            var result = await _gameDataStore.Collection(_enemyInfo).Document(unitInfo.Name).GetSnapshotAsync();

            if (result.Exists)
            {
                Debug.Log($"{unitInfo.Name}이(가) 이미 존재합니다.");
                return false;
            }

            await _gameDataStore.Collection(_enemyInfo).Document(unitInfo.Name).SetAsync(unitInfo);
            return true;
        }
        catch (FirestoreException e)
        {
            Debug.Log(e.Message);
            return false;
        }
    }

    public void UpdateUserInfo(FirebaseUser user, UserInfo userInfo)
    {
        _userStore.Collection(_userInfo).Document(user.Email).GetSnapshotAsync().ContinueWith(task =>
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
                    string key = user.Email;
                    //Dictionary<string, object> data = new Dictionary<string, object>
                    //{
                    //{"name", userInfo.Name},
                    //{"lv", userInfo.Lv},
                    //{"hp", userInfo.Hp},
                    //{"mp", userInfo.Mp},
                    //{"exp", userInfo.Exp},
                    //{"moveSpeed", userInfo.MoveSpeed},
                    //{"atk", userInfo.Atk},
                    //{"def", userInfo.Def},
                    //{"str", userInfo.Str},
                    //{"dex", userInfo.Dex},
                    //{"int", userInfo.Int},
                    //{"luk", userInfo.Luk}
                    //};
                    //_userStore.Collection(_userInfo).Document(key).SetAsync(data);
                    _userStore.Collection(_userInfo).Document(key).SetAsync(userInfo);
                    Debug.Log("User Data Updated");
                }
            }
        });
    }

    public void LoadUserInfo(FirebaseUser user, Action<UserInfo> onComplete)
    {
        _userStore.Collection(_userInfo).Document(user.Email).GetSnapshotAsync().ContinueWith(task =>
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
                    //Dictionary<string, object> d = snapshot.ToDictionary();
                    //string name = (string)d["name"];
                    //int lv = Convert.ToInt32(d["lv"]);
                    //int hp = Convert.ToInt32(d["hp"]);
                    //int mp = Convert.ToInt32(d["mp"]);
                    //float moveSpeed = Convert.ToSingle(d["moveSpeed"]);
                    //int atk = Convert.ToInt32(d["atk"]);
                    //int def = Convert.ToInt32(d["def"]);
                    //int str = Convert.ToInt32(d["str"]);
                    //int dex = Convert.ToInt32(d["dex"]);
                    //int _int = Convert.ToInt32(d["int"]);
                    //int luk = Convert.ToInt32(d["luk"]);
                    //int exp = Convert.ToInt32(d["exp"]);
                    //UserInfo userInfo = new UserInfo(name, lv, hp, mp, moveSpeed, atk, def, str, dex, _int, luk, exp);

                    UserInfo userInfo = snapshot.ConvertTo<UserInfo>();

                    onComplete(userInfo);
                }
                else
                    onComplete(null);
            }
        });
    }

    public async Task<UnitInfo> LoadUnitInfo(string name)
    {
        try
        {
            var result = await _gameDataStore.Collection(_enemyInfo).WhereEqualTo("name", name).GetSnapshotAsync();

            if (result.Count > 0)
            {
                UnitInfo unitInfo = null;
                foreach (DocumentSnapshot ds in result)
                    unitInfo = ds.ConvertTo<UnitInfo>();

                return unitInfo;
            }
            else
            {
                Debug.Log($"{name}이(가) 존재하지 않습니다.");
                return null;
            }
        }
        catch (FirestoreException e)
        {
            Debug.Log($"유닛 데이터 로드 실패 : {e.Message}");
            return null;
        }
    }

    public async Task<UserInfo> LoadUserInfo(string email)
    {
        try
        {
            var result = await _userStore.Collection(_userInfo).Document(email).GetSnapshotAsync();
            if (result.Exists)
            {
                //Dictionary<string, object> d = result.ToDictionary();
                //string name = (string)d["name"];
                //int lv = Convert.ToInt32(d["lv"]);
                //int hp = Convert.ToInt32(d["hp"]);
                //int mp = Convert.ToInt32(d["mp"]);
                //float moveSpeed = Convert.ToSingle(d["moveSpeed"]);
                //int atk = Convert.ToInt32(d["atk"]);
                //int def = Convert.ToInt32(d["def"]);
                //int str = Convert.ToInt32(d["str"]);
                //int dex = Convert.ToInt32(d["dex"]);
                //int _int = Convert.ToInt32(d["int"]);
                //int luk = Convert.ToInt32(d["luk"]);
                //int exp = Convert.ToInt32(d["exp"]);
                //UserInfo userInfo = new UserInfo(name, lv, hp, mp, moveSpeed, atk, def, str, dex, _int, luk, exp);

                UserInfo userInfo = result.ConvertTo<UserInfo>();

                return userInfo;
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
