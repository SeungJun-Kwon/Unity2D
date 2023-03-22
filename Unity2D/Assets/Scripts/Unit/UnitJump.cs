using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitJump : IState, IPunObservable
{
    UnitController _unitController;

    public UnitJump(UnitController unitController)
    {
        _unitController = unitController;
    }

    public void OnEnter()
    {
        _unitController.Jump();
    }

    public void OnExit()
    {

    }

    public void OnFixedUpdate()
    {
        if (_unitController._photonView.IsMine)
        {
            if (_unitController._rigidbody.velocity.y < -0.01f || _unitController._rigidbody.velocity.y > 0.01f)
                _unitController.IsGrounded = false;
            else if (_unitController._rigidbody.velocity.y <= 0.01f && _unitController._rigidbody.velocity.y >= -0.01f)
                _unitController.IsGrounded = true;
        }
    }

    public void OnUpdate()
    {

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_unitController.IsGrounded);
        }
        else if (stream.IsReading)
        {
            _unitController.IsGrounded = (bool)stream.ReceiveNext();
        }
    }

}
