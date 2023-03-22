using Firebase.Auth;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public UnitInfo _info;

    public float CurHp
    {
        get
        {
            return _curHp;
        }
        set
        {
            _curHp = value;
        }
    }
    public float CurMp
    {
        get
        {
            return _curMp;
        }
        set
        {
            _curMp = value;
        }
    }

    private float _curHp, _curMp;

    [PunRPC]
    public virtual async void LoadUnitData(string name)
    {
        _info = await FirebaseFirestoreManager.Instance.LoadUnitInfo(name);
        if(_info == null)
            gameObject.SetActive(false);

        CurHp = _info.Hp;
        CurMp = _info.Mp;
    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}
