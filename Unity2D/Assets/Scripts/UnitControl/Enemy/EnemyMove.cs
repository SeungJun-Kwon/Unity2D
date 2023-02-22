using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMove : IState, IPunObservable
{
    EnemyController _unitController;

    public EnemyMove(EnemyController unit)
    {
        _unitController = unit;
    }

    float _range = 10f;
    float _count, _moveDuration, _randomMoveX;

    public void OnEnter()
    {
        _count = 0f;
        _moveDuration = Random.Range(1, 4);
        _randomMoveX = Random.Range(-1, 2);

        if (_randomMoveX != 0)
            _unitController.PlayAnimation(State.MOVE);
    }

    public void OnExit()
    {
        _count = 0f;
    }

    public void OnUpdate()
    {
        //if (_unitController._photonView.IsMine)
        //{
        //    var hit = Physics2D.BoxCast(_unitController._collider.bounds.center, new Vector2(_range, _unitController._collider.bounds.size.y), 0f, Vector2.up, 0f, LayerMask.GetMask("Player"));
        //    if (!hit)
        //        _unitController._target = null;
        //}
    }

    public void OnFixedUpdate()
    {
        //if (_unitController._photonView.IsMine)
        //{
        //    if (_unitController._target != null)
        //        _unitController.Move(_unitController._target);
        //    else
        //        _unitController.MoveOriginPos();
        //}

        if (_unitController.photonView.IsMine)
        {
            _count += Time.fixedDeltaTime;
            if (_count >= _moveDuration)
            {
                _unitController.photonView.RPC("ExitState", RpcTarget.AllBuffered, State.MOVE);
                _unitController.photonView.RPC("EnterState", RpcTarget.AllBuffered, State.IDLE);
                //_unitController.ExitState(State.MOVE);
                //_unitController.EnterState(State.IDLE);
            }

            if (_randomMoveX != 0)
                _unitController.Move(_randomMoveX);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_count);
            stream.SendNext(_moveDuration);
            stream.SendNext(_randomMoveX);
        }
        else if (stream.IsReading)
        {
            _count = (float)stream.ReceiveNext();
            _moveDuration = (float)stream.ReceiveNext();
            _randomMoveX = (float)stream.ReceiveNext();
        }
    }
}
