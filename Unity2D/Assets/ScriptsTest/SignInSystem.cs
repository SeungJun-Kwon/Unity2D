using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignInSystem : MonoBehaviour
{
    [SerializeField] TMP_InputField _idInput, _pwInput;
    [SerializeField] GameObject _signUpPanel, _channelSellector;
    string _id, _pw;

    bool _result;
    bool Result
    {
        get
        {
            return _result;
        }
        set
        {
            _result = value;
            if(_result == true)
            {
                gameObject.SetActive(false);
                _channelSellector.SetActive(true);
            }
        }
    }

    private void Start()
    {
        FirebaseAuthManager.Instance.Init();
        FirebaseReatimeDatabaseManager.Instance.Init();
        FirebaseFirestoreManager.Instance.Init();
    }

    bool Check()
    {
        _id = _idInput.text.Trim();
        _pw = _pwInput.text.Trim();

        if (_id == "" || _pw == "")
            return false;

        return true;
    }

    public void OpenSignUp() => _signUpPanel.SetActive(true);

    public async void SignIn()
    {
        if (!Check())
        {
            print("입력되지 않은 칸이 있습니다.");
            return;
        }

        Result = await FirebaseAuthManager.Instance.SignIn(_id, _pw);
    }
}
