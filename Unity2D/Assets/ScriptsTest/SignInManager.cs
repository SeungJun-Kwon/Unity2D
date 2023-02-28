using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignInManager : MonoBehaviour
{
    [SerializeField] TMP_InputField _idInput, _pwInput;
    [SerializeField] Button _signInBt, _signUpBt;
    [SerializeField] GameObject _signUpPanel;
    string _id, _pw;

    bool Check()
    {
        _id = _idInput.text.Trim();
        _pw = _pwInput.text.Trim();

        if (_id == "" || _pw == "")
            return false;

        return true;
    }

    public void OpenSignUp() => _signUpPanel.SetActive(true);

    public void SignIn()
    {
        if(!Check())
        {
            print("입력되지 않은 칸이 있습니다.");
            return;
        }

        WWWForm form = new WWWForm();
        form.AddField("order", "signin");
        form.AddField("id", _id);
        form.AddField("pw", _pw);

        StartCoroutine(GoogleSheetManager.instance.Post(form));
    }
}
