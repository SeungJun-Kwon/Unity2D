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

    public FirebaseUser _user;     // ������ �Ϸ�� ���� ����

    FirebaseAuth _auth;     // �α���, ȸ������ � ���

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
                Debug.Log("ȸ������ ���");
                return;
            }
            else if(task.IsFaulted)
            {
                // ���� ���� => �̸����� ������, ��й�ȣ�� �ʹ� ����, �̹� ���Ե� �̸��� ���
                Debug.Log("ȸ������ ���� : " + task.Exception);
                return;
            }

            FirebaseUser user = task.Result;
            FirebaseFirestoreManager.Instance.CreateUserInfo(user);
            Debug.Log("ȸ������ �Ϸ�");
        });
    }

    public void SignIn(string email, string pw)
    {
        _auth.SignInWithEmailAndPasswordAsync(email, pw).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.Log("�α��� ���");
                return;
            }
            else if (task.IsFaulted)
            {
                // ���� ���� => �̸����� ������, ��й�ȣ�� �ʹ� ����, �̹� ���Ե� �̸��� ���
                Debug.Log("�α��� ���� : " + task.Exception);
                return;
            }

            _user = task.Result;
            Debug.Log("�α��� �Ϸ�");
        });
    }

    public void SignOut()
    {
        _auth.SignOut();
    }
}
