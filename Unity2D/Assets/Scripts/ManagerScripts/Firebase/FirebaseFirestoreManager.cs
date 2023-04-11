using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using System;
using System.Threading.Tasks;
using Firebase;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor.PackageManager;
using UnityEditor;
using JetBrains.Annotations;

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
    string _itemInfo = "itemInfo";

    int _inventoryCount = 0;

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
        // 방금의 앱 옵션으로 파이어베이스 앱을 만듦(입력 데이터를 통해 파이어베이스에서 가져오는 것)
        fapp = FirebaseApp.Create(app, "Unity2DGameData");
        // 파이어베이스 앱을 통해 파이어베이스 스토어를 가져옴
        _gameDataStore = FirebaseFirestore.GetInstance(fapp);

        _inventoryCount = new Inventory()._capacity;
    }

    #region User
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
                //    {"atk", userInfo.Atk},
                //    {"def", userInfo.Def},
                //    {"moveSpeed", userInfo.MoveSpeed},
                //    {"attackSpeed", userInfo.AttackSpeed},
                //};
                //_userStore.Collection(_userInfo).Document(userEmail).SetAsync(data);
                _userStore.Collection(_userInfo).Document(userEmail).SetAsync(userInfo);

                Dictionary<string, object> invenDic = new Dictionary<string, object>();
                for(int i = 0; i < _inventoryCount; i++)
                    invenDic.Add(i.ToString(), "null");
                Dictionary<string, object> equipmentDic = new Dictionary<string, object>();
                equipmentDic.Add("Head", "null");
                equipmentDic.Add("Body", "null");
                equipmentDic.Add("Hand", "null");
                equipmentDic.Add("Foot", "null");
                equipmentDic.Add("Weapon", "null");
                _userStore.Collection(_userInfo).Document(userEmail).Collection("Inventory").Document("Equipment").SetAsync(invenDic);
                _userStore.Collection(_userInfo).Document(userEmail).Collection("Inventory").Document("Consumption").SetAsync(invenDic);
                _userStore.Collection(_userInfo).Document(userEmail).Collection("Inventory").Document("Material").SetAsync(invenDic);
                _userStore.Collection(_userInfo).Document(userEmail).Collection("Equipment").Document("Item").SetAsync(equipmentDic);
                Debug.Log("New user added.");
            }
        });
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
                    _userStore.Collection(_userInfo).Document(key).SetAsync(userInfo);
                    Debug.Log("User Data Updated");
                }
            }
        });
    }

    public async Task<UserInfo> LoadUserInfo(string email)
    {
        try
        {
            var result = await _userStore.Collection(_userInfo).Document(email).GetSnapshotAsync();
            if (result.Exists)
            {
                UserInfo userInfo = result.ConvertTo<UserInfo>();

                return userInfo;
            }
            else
                return null;
        }
        catch (FirestoreException e)
        {
            Debug.Log($"유저 데이터 로드 실패 : {e.Message}");
            return null;
        }
    }

    public void UpdateUserInventory(FirebaseUser user, Inventory inventory)
    {
        _userStore.Collection(_userInfo).Document(user.Email).Collection("Inventory").Document("Equipment").GetSnapshotAsync().ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log("Failed to save user inventory : " + task.Exception);
                return;
            }

            if (task.IsCompleted)
            {
                var result = task.Result;

                if (result.Exists)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    for (int i = 0; i < inventory._capacity; i++)
                        data.Add(i.ToString(), inventory._equipmentItemArr[i]);

                    _userStore.Collection(_userInfo).Document(user.Email).Collection("Inventory").Document("Equipment").SetAsync(data);
                }
            }
        });
        _userStore.Collection(_userInfo).Document(user.Email).Collection("Inventory").Document("Consumption").GetSnapshotAsync().ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log("Failed to save user inventory : " + task.Exception);
                return;
            }

            if (task.IsCompleted)
            {
                var result = task.Result;

                if (result.Exists)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    for (int i = 0; i < inventory._capacity; i++)
                        data.Add(i.ToString(), inventory._consumptionItemArr[i]);

                    _userStore.Collection(_userInfo).Document(user.Email).Collection("Inventory").Document("Consumption").SetAsync(data);
                }
            }
        });
        _userStore.Collection(_userInfo).Document(user.Email).Collection("Inventory").Document("Material").GetSnapshotAsync().ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log("Failed to save user inventory : " + task.Exception);
                return;
            }

            if (task.IsCompleted)
            {
                var result = task.Result;

                if (result.Exists)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    for (int i = 0; i < inventory._capacity; i++)
                        data.Add(i.ToString(), inventory._itemArr[i]);

                    _userStore.Collection(_userInfo).Document(user.Email).Collection("Inventory").Document("Material").SetAsync(data);
                }
            }
        });

        Debug.Log("User Inventory Data Updated");
    }

    public void UpdateUserEquipment(FirebaseUser user, Equipment equipment)
    {
        _userStore.Collection(_userInfo).Document(user.Email).Collection("Equipment").Document("Item").GetSnapshotAsync().ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log("Failed to save user equipment : " + task.Exception);
                return;
            }

            if (task.IsCompleted)
            {
                var result = task.Result;

                if (result.Exists)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    data.Add("Head", equipment.Head);
                    data.Add("Body", equipment.Body);
                    data.Add("Hand", equipment.Hand);
                    data.Add("Foot", equipment.Foot);
                    data.Add("Weapon", equipment.Weapon);

                    _userStore.Collection(_userInfo).Document(user.Email).Collection("Equipment").Document("Item").SetAsync(data);
                }
            }
        });
    }

    public async Task<Inventory> LoadUserInventory(string email)
    {
        try
        {
            var equipment = await _userStore.Collection(_userInfo).Document(email).Collection("Inventory").Document("Equipment").GetSnapshotAsync();
            var consumption = await _userStore.Collection(_userInfo).Document(email).Collection("Inventory").Document("Consumption").GetSnapshotAsync();
            var material = await _userStore.Collection(_userInfo).Document(email).Collection("Inventory").Document("Material").GetSnapshotAsync();
            if (equipment.Exists && consumption.Exists && material.Exists)
            {
                Inventory inventory = new();
                Dictionary<string, object> equipDic = equipment.ToDictionary();
                Dictionary<string, object> consumpDic = consumption.ToDictionary();
                Dictionary<string, object> materialDic = material.ToDictionary();

                for (int i = 0; i < equipDic.Count; i++)
                    inventory._equipmentItemArr.Add(equipDic[i.ToString()].ToString());
                for (int i = 0; i < consumpDic.Count; i++)
                    inventory._consumptionItemArr.Add(consumpDic[i.ToString()].ToString());
                for (int i = 0; i < materialDic.Count; i++)
                    inventory._itemArr.Add(materialDic[i.ToString()].ToString());

                return inventory;
            }
            else
                return null;
        }
        catch (FirestoreException e)
        {
            Debug.Log($"유저 인벤토리 로드 실패 : {e.Message}");
            return null;
        }
    }

    public async Task<Equipment> LoadUserEquipment(string email)
    {
        try
        {
            var result = await _userStore.Collection(_userInfo).Document(email).Collection("Equipment").Document("Item").GetSnapshotAsync();
            if (result.Exists)
            {
                Equipment equipment = result.ConvertTo<Equipment>();

                return equipment;
            }
            else
                return null;
        }
        catch (FirestoreException e)
        {
            Debug.Log($"유저 장비 로드 실패 : {e.Message}");
            return null;
        }
    }
    #endregion

    #region Item
    public async Task<bool> CreateItem<T>(T itemInfo, string name)
    {
        try
        {
            var result = await _gameDataStore.Collection(_itemInfo).Document(name).GetSnapshotAsync();

            if (result.Exists)
            {
                await UpdateItemInfo(itemInfo, name);
                Debug.Log($"{name} 수정 완료.");
                return false;
            }

            await _gameDataStore.Collection(_itemInfo).Document(name).SetAsync(itemInfo);
            return true;
        }
        catch (FirestoreException e)
        {
            Debug.Log(e.Message);
            return false;
        }
    }

    public async Task UpdateItemInfo<T>(T itemInfo, string name)
    {
        try
        {
            var result = _gameDataStore.Collection(_itemInfo).Document(name).GetSnapshotAsync();

            if (result.IsCanceled || result.IsFaulted)
            {
                Debug.Log($"{name} 아이템 수정 불가");
                return;
            }

            await _gameDataStore.Collection(_itemInfo).Document(name).SetAsync(itemInfo);
            return;
        }
        catch (FirestoreException e)
        {
            Debug.Log(e);
        }
    }

    public async Task<ItemInfo> LoadItemInfo(string name)
    {
        try
        {
            var result = await _gameDataStore.Collection(_itemInfo).WhereEqualTo("name", name).GetSnapshotAsync();

            if (result.Count > 0)
            {
                ItemInfo itemInfo = null;
                foreach (DocumentSnapshot ds in result)
                {
                    ds.TryGetValue("type", out string type);
                    if (type == ItemType.Null.ToString() || type == ItemType.Material.ToString())
                        itemInfo = ds.ConvertTo<ItemInfo>();
                    else if (type == ItemType.Equipment.ToString())
                        itemInfo = ds.ConvertTo<EquipmentItemInfo>();
                    else if (type == ItemType.Consumption.ToString())
                        itemInfo = ds.ConvertTo<ConsumptionItemInfo>();
                }

                return itemInfo;
            }
            else
            {
                Debug.Log($"{name}이(가) 존재하지 않습니다.");
                return null;
            }
        }
        catch (FirestoreException e)
        {
            Debug.Log($"아이템 데이터 로드 실패 : {e.Message}");
            return null;
        }
    }

    public async Task<List<ItemInfo>> LoadAllItemInfo()
    {
        try
        {
            var result = await _gameDataStore.Collection(_itemInfo).GetSnapshotAsync();

            if(result.Count > 0)
            {
                List<ItemInfo> itemInfos = new List<ItemInfo>();
                foreach (DocumentSnapshot ds in result)
                {
                    ds.TryGetValue("type", out string type);
                    if (type == ItemType.Equipment.ToString())
                        itemInfos.Add(ds.ConvertTo<EquipmentItemInfo>());
                    else if (type == ItemType.Consumption.ToString())
                        itemInfos.Add(ds.ConvertTo<ConsumptionItemInfo>());
                    else if (type == ItemType.Null.ToString() || type == ItemType.Material.ToString())
                        itemInfos.Add(ds.ConvertTo<ItemInfo>());
                }

                return itemInfos;
            }
            else
            {
                Debug.LogError($"전체 아이템 데이터 로드 실패");
                return null;
            }
        }
        catch(FirestoreException e)
        {
            Debug.LogError($"전체 아이템 데이터 로드 실패 : {e.Message}");
            return null;
        }
    }

    public async void LoadItemData()
    {
        try
        {
            var result = await _gameDataStore.Collection(_itemInfo).GetSnapshotAsync();
            if (result.Count > 0)
            {
                ItemInfo itemInfo = null;
                ItemData itemData = new ItemData();
                foreach (var ds in result)
                {
                    ds.TryGetValue("type", out string type);
                    if (type == ItemType.Null.ToString() || type == ItemType.Material.ToString())
                        itemInfo = ds.ConvertTo<ItemInfo>();
                    else if (type == ItemType.Equipment.ToString())
                        itemInfo = ds.ConvertTo<EquipmentItemInfo>();
                    else if (type == ItemType.Consumption.ToString())
                        itemInfo = ds.ConvertTo<ConsumptionItemInfo>();

                    if (itemInfo.Type == ItemType.Null.ToString() || itemInfo.Type == ItemType.Material.ToString())
                        itemData._itemArr.Add(itemInfo);
                    else if (itemInfo.Type == ItemType.Equipment.ToString())
                        itemData._equipmentItemArr.Add(itemInfo as EquipmentItemInfo);
                    else if (itemInfo.Type == ItemType.Consumption.ToString())
                        itemData._consumptionItemArr.Add(itemInfo as ConsumptionItemInfo);
                }

                string json = NewtonsoftJson.Instance.ObjectToJson(itemData);
                NewtonsoftJson.Instance.SaveJsonFile("Assets/Resources/Json/", "ItemData", json);
            }
            ItemDataManager.Instance.Init();
            await ItemDataManager.Instance.CheckItemData();
        }
        catch (FirestoreException e)
        {
            Debug.Log($"아이템 데이터 로드 실패 : {e.Message}");
        }
    }
    #endregion

    #region Unit
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
    #endregion

    #region Enemy
    public async Task<bool> CreateEnemy(EnemyInfo enemyInfo)
    {
        try
        {
            var result = await _gameDataStore.Collection(_enemyInfo).Document(enemyInfo.Name).GetSnapshotAsync();

            await _gameDataStore.Collection(_enemyInfo).Document(enemyInfo.Name).SetAsync(enemyInfo);
            return true;
        }
        catch (FirestoreException e)
        {
            Debug.Log(e.Message);
            return false;
        }
    }

    public async Task<EnemyInfo> LoadEnemyInfo(string name)
    {
        try
        {
            var result = await _gameDataStore.Collection(_enemyInfo).Document(name).GetSnapshotAsync();

            if (result.Exists)
            {
                EnemyInfo enemyInfo = null;
                enemyInfo = result.ConvertTo<EnemyInfo>();

                return enemyInfo;
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
    #endregion

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
                    //int atk = Convert.ToInt32(d["atk"]);
                    //int def = Convert.ToInt32(d["def"]);
                    //int exp = Convert.ToInt32(d["exp"]);
                    //float moveSpeed = Convert.ToSingle(d["moveSpeed"]);
                    //float attackSpeed = Convert.ToSingle(d["attackSpeed"]);
                    //UserInfo userInfo = new UserInfo(name, lv, hp, mp, atk, def, moveSpeed, attackSpeed, exp);

                    UserInfo userInfo = snapshot.ConvertTo<UserInfo>();

                    onComplete(userInfo);
                }
                else
                    onComplete(null);
            }
        });
    }
}