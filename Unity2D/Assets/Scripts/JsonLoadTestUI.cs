using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JsonLoadTestUI : MonoBehaviour
{
    public Text _id, _name, _lv, _hp, _mp, _dmg;
    public InputField _inputField;

    Text _selectedText;

    private void Start()
    {
        LoadData();
    }

    public void OpenInputField()
    {
        EventSystem.current.currentSelectedGameObject.TryGetComponent<Text>(out var t);
        if (t != null)
        {
            _selectedText = t;
            _inputField.gameObject.SetActive(true);
            _inputField.text = t.text;
        }
    }

    public void EndInputField()
    {
        _selectedText.text = _inputField.text;
        _inputField.gameObject.SetActive(false);
    }

    public void LoadData()
    {
        var json = NewtonsoftJson.Instance.LoadJsonFile<JsonTest>("Assets/Resources/Json/", "JsonTest");

        _id.text = json.monsterId.ToString();
        _name.text = json.monsterName;
        _lv.text = json.monsterLv.ToString();
        _hp.text = json.monsterHp.ToString();
        _mp.text = json.monsterMp.ToString();
        _dmg.text = json.monsterDmg.ToString();
    }

    public void SaveData()
    {
        JsonTest jsonTest = new JsonTest();

        jsonTest.monsterId = int.Parse(_id.text);
        jsonTest.monsterName = _name.text;
        jsonTest.monsterLv = int.Parse(_lv.text);
        jsonTest.monsterHp = int.Parse(_hp.text);
        jsonTest.monsterMp = int.Parse(_mp.text);
        jsonTest.monsterDmg = int.Parse(_dmg.text);

        string json = NewtonsoftJson.Instance.ObjectToJson(jsonTest);
        NewtonsoftJson.Instance.SaveJsonFile("Assets/Resources/Json/", "JsonTest", json);
    }
}
