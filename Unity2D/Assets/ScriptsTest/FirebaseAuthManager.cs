using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FirebaseAuthManager
{
    private static FirebaseAuthManager instance = null;
    public static FirebaseAuthManager Instance
    {
        get
        {
            if(instance == null)
                instance = new FirebaseAuthManager();

            return instance;
        }
    }

    public FirebaseUser _user;     // 인증이 완료된 유저 정보

    FirebaseAuth _auth;     // 로그인, 회원가입 등에 사용

    public void Init()
    {
        _auth = FirebaseAuth.DefaultInstance;

        _auth.StateChanged += OnChanged;
    }

    private void OnChanged(object sender, EventArgs e)
    {
        if(_auth.CurrentUser != _user)
        {
            bool signed = (_auth.CurrentUser != _user && _auth.CurrentUser != null);


        }
    }

    public void SignUp(string email, string pw)
    {
        _auth.CreateUserWithEmailAndPasswordAsync(email, pw).ContinueWith(task => 
        {
            if(task.IsCanceled)
            {
                Debug.Log("회원가입 취소");
                return;
            }
            else if(task.IsFaulted)
            {
                // 실패 이유 => 이메일이 비정상, 비밀번호가 너무 간단, 이미 가입된 이메일 등등
                Debug.Log("회원가입 실패 : " + task.Exception);
                return;
            }

            FirebaseUser user = task.Result;
            FirebaseFirestoreManager.Instance.CreateUserInfo(user);
            Debug.Log("회원가입 완료");
        });
    }

    public void SignIn(string email, string pw)
    {
        _auth.SignInWithEmailAndPasswordAsync(email, pw).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.Log("로그인 취소");
                return;
            }
            else if (task.IsFaulted)
            {
                // 실패 이유 => 이메일이 비정상, 비밀번호가 너무 간단, 이미 가입된 이메일 등등
                Debug.Log("로그인 실패 : " + task.Exception);
                return;
            }

            _user = task.Result;
            Debug.Log("로그인 완료");
        });
    }

    public void SignOut()
    {
        _auth.SignOut();
    }
}
