using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : IState, IPunObservable
{
    PlayerController _unitController;

    float _attackDuration = 1f;

    public PlayerAttack(PlayerController unit)
    {
        _unitController = unit;
        for (int i = 0; i < _unitController._animator.runtimeAnimatorController.animationClips.Length; i++)
        {
            if (_unitController._animator.runtimeAnimatorController.animationClips[i].name == "attack")
            {
                _attackDuration = _unitController._animator.runtimeAnimatorController.animationClips[i].length;
                break;
            }
        }
    }

    float _count;

    public void OnEnter()
    {
        _unitController.PlayAnimation(State.ATTACK);
        _unitController._weaponCol.enabled = true;
        _count = 0;
    }

    public void OnExit()
    {
        _unitController._weaponCol.enabled = false;
    }

    public void OnFixedUpdate()
    {

    }

    public void OnUpdate()
    {
        _count += Time.deltaTime;
        if (_count > _attackDuration)
        {
            _unitController.photonView.RPC("ExitState", RpcTarget.AllBuffered, State.ATTACK);
            _unitController.photonView.RPC("EnterState", RpcTarget.AllBuffered, State.IDLE);
            //_unitController.ExitState(State.ATTACK);
            //_unitController.EnterState(State.IDLE);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_count);
            stream.SendNext(_attackDuration);
        }
        else if (stream.IsReading)
        {
            _count = (float)stream.ReceiveNext();
            _attackDuration = (float)stream.ReceiveNext();
        }
    }
}
