using Firebase.Auth;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : UnitManager
{
    public UserInfo _userInfo;

    public int _damage;

    [PunRPC]
    public async void LoadUserData(string email)
    {
        _info = await FirebaseFirestoreManager.Instance.LoadUserInfo(email);
        _userInfo = _info as UserInfo;

        CurHp = _userInfo.Hp;
        CurMp = _userInfo.Mp;

        _damage = (int)(((_userInfo.Str * 5 + _userInfo.Dex * 2.5f + _userInfo.Int * 0.5f + _userInfo.Luk) * _userInfo.Atk) * 0.66);
    }

    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {

        }
        else if(stream.IsReading)
        {

        }
    }
}
