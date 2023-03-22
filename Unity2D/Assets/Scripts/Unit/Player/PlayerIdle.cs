using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdle : IState
{
    UnitController _unitController;
    public PlayerIdle(UnitController unitController)
    {
        _unitController = unitController;
    }

    public void OnEnter()
    {
        _unitController.PlayAnimation(State.IDLE);
    }

    public void OnExit()
    {

    }

    public void OnUpdate()
    {
        if (_unitController._photonView.IsMine)
        {
            if ((Input.GetAxisRaw("Horizontal") > 0f || Input.GetAxisRaw("Horizontal") < 0f) && !_unitController.FindState(State.ATTACK))
            {
                _unitController.photonView.RPC("ExitState", RpcTarget.AllBuffered, State.IDLE);
                _unitController.photonView.RPC("EnterState", RpcTarget.AllBuffered, State.MOVE);
                //_unitController.ExitState(State.IDLE);
                //_unitController.EnterState(State.MOVE);
            }
        }
    }

    public void OnFixedUpdate()
    {

    }
}
