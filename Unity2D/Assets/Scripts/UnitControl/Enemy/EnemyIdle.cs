using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class EnemyIdle : IState, IPunObservable
{
    EnemyController _unitController;

    float _range = 5f;
    float _count, _moveDuration;

    public EnemyIdle(EnemyController unit)
    {
        _unitController = unit;
    }

    public void OnEnter()
    {
        _unitController.PlayAnimation(State.IDLE);
        _count = 0f;
        _moveDuration = Random.Range(3, 7);
    }

    public void OnExit()
    {
        _count = 0f;
    }

    public void OnUpdate()
    {
        //if (_unitController.photonView.IsMine)
        //{
        //    var hit = Physics2D.BoxCast(_unitController._collider.bounds.center, new Vector2(_range, _unitController._collider.bounds.size.y), 0f, Vector2.up, 0f, LayerMask.GetMask("Player"));
        //    if (hit)
        //    {
        //        _unitController._target = hit.transform;
        //        _unitController.ExitState(State.IDLE);
        //        _unitController.EnterState(State.MOVE);
        //    }
        //}
    }

    public void OnFixedUpdate()
    {
        if (_unitController.photonView.IsMine)
        {
            _count += Time.fixedDeltaTime;
            if (_count >= _moveDuration)
            {
                _unitController.ExitState(State.IDLE);
                _unitController.EnterState(State.MOVE);
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(_count);
            stream.SendNext(_moveDuration);
        }
        else if(stream.IsReading)
        {
            _count = (float)stream.ReceiveNext();
            _moveDuration = (float)stream.ReceiveNext();
        }
    }
}
