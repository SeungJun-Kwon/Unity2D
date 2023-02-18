using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : IState
{
    UnitController _unitController;
    public PlayerMove(UnitController unitController)
    {
        _unitController = unitController;
    }

    float _moveX;

    public void OnEnter()
    {
        _unitController.PlayAnimation(State.MOVE);
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        if (_unitController._photonView.IsMine)
        {
            _moveX = Input.GetAxisRaw("Horizontal");

            if (_moveX == 0)
            {
                _unitController.ExitState(State.MOVE);
                _unitController.EnterState(State.IDLE);
            }
        }
    }

    public void OnFixedUpdate()
    {
        if(_unitController._photonView.IsMine && _moveX != 0)
            _unitController.Move(_moveX);
    }
}
