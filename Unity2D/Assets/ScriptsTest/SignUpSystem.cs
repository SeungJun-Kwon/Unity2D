using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignUpSystem : MonoBehaviour
{
    [SerializeField] TMP_InputField _nameInput, _idInput, _pwInput;
    [SerializeField] Button _signUpBt, _cancelBt;
    string _name, _id, _pw;

    bool Check()
    {
        _name = _nameInput.text.Trim();
        _id = _idInput.text.Trim();
        _pw = _pwInput.text.Trim();

        if (_name == "" || _id == "" || _pw == "")
            return false;

        return true;
    }

    public void SignUp()
    {
        if (!Check())
        {
            print("입력되지 않은 칸이 있습니다.");
            return;
        }

        FirebaseAuthManager.Instance.SignUp(_id, _pw);

        gameObject.SetActive(false);
    }
}
