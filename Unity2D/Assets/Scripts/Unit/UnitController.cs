using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Security.Cryptography;
using System;
using UnityEngine.UI;

public enum State { IDLE, MOVE, JUMP, ATTACK, DIE, }
public interface IState
{
    void OnEnter();
    void OnExit();
    void OnUpdate();
    void OnFixedUpdate();
}

public class UnitController : MonoBehaviourPunCallbacks, IPunObservable
{
    bool _isGrounded = true;
    public bool IsGrounded
    {
        get { return _isGrounded; }
        set
        {
            _isGrounded = value;
            if (_isGrounded)
                photonView.RPC("ExitState", RpcTarget.AllBuffered, State.JUMP);
            //ExitState(State.JUMP);
        }
    }

    [HideInInspector] public Rigidbody2D _rigidbody;
    [HideInInspector] public CapsuleCollider2D _collider;
    [HideInInspector] public Animator _animator;
    [HideInInspector] public PhotonView _photonView;
    public UnitInfo _info;

    public Image _hpBar;

    public float _moveSpeed = 3f;
    public float _maxSpeed = 10f;
    public float _jumpForce = 15f;
    public float _curHp, _curMp;

    protected Dictionary<State, IState> _stateDic = new Dictionary<State, IState>();
    protected List<IState> _iStateArr = new List<IState>();
    protected List<State> _stateArr = new List<State>();

    protected Vector3 _curPos;

    protected virtual void Awake()
    {
        TryGetComponent(out _rigidbody);
        TryGetComponent(out _collider);
        TryGetComponent(out _animator);
        TryGetComponent(out _photonView);

        _stateDic.Add(State.JUMP, new UnitJump(this));
    }

    protected virtual void Update()
    {
        if (photonView.IsMine)
        {
            for (int i = 0; i < _iStateArr.Count; i++)
                _iStateArr[i].OnUpdate();
        }
    }

    protected virtual void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            for (int i = 0; i < _iStateArr.Count; i++)
                _iStateArr[i].OnFixedUpdate();
        }
        else
        {
            if ((transform.position - _curPos).sqrMagnitude >= 100f)
                transform.position = _curPos;
            else
                transform.position = Vector3.Lerp(transform.position, _curPos, Time.deltaTime * 10f);
        }
    }

    public virtual void Move(float moveX)
    {
        // AllBuffered로 하는 이유는 재접속시 동기화해주기 위해서
        // RpcTarget.All 이면, 호출되고 잊어버림.
        // RpcTarget.AllBuffered 이면, 호출되고 버퍼에 저장됨. 이후에 누가 들어오면 자동으로 순차적으로 실행된다.
        // 버퍼에 너무 많이 저장되면, 네트워크가 약한 클라이언트는 끊어질 수 있다고 한다.
        _photonView.RPC("MoveXRPC", RpcTarget.AllBuffered, moveX);

        _rigidbody.AddForce(Vector2.right * moveX * _moveSpeed, ForceMode2D.Impulse);
        if (_rigidbody.velocity.x > _maxSpeed)
            _rigidbody.velocity = new Vector2(_maxSpeed, _rigidbody.velocity.y);
        else if (_rigidbody.velocity.x < -_maxSpeed)
            _rigidbody.velocity = new Vector2(-_maxSpeed, _rigidbody.velocity.y);
    }

    [PunRPC]
    public virtual void MoveXRPC(float moveX)
    {
        transform.localScale = new Vector2(-moveX, 1f);
    }

    public virtual void Move(Transform target)
    {
        float moveX = 0f;
        if (target.position.x - transform.position.x > 0f)
        {
            moveX = 1f;
            Move(moveX);
        }
        else if (target.position.x - transform.position.x < 0f)
        {
            moveX = -1f;
            Move(moveX);
        }
    }

    public void Jump()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.5f, 1 << LayerMask.NameToLayer("Ground"));
        if (hit && _photonView.IsMine)
        {
            _photonView.RPC("JumpRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    public virtual void JumpRPC()
    {
        _rigidbody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        _isGrounded = false;
    }

    public virtual void Attack()
    {
        ExitState(State.MOVE);
        ExitState(State.IDLE);
        EnterState(State.ATTACK);
    }

    public virtual void PlayAnimation(State state)
    {

    }

    [PunRPC]
    public void EnterState(State state)
    {
        if (!_stateArr.Contains(state))
        {
            _stateArr.Add(state);
            _stateDic.TryGetValue(state, out var iState);
            iState.OnEnter();
            _iStateArr.Add(iState);
        }
    }

    [PunRPC]
    public void ExitState(State state)
    {
        if (_stateArr.Contains(state))
        {
            _stateArr.Remove(state);
            _stateDic.TryGetValue(state, out var iState);
            iState.OnExit();
            _iStateArr.Remove(iState);
        }
    }

    public bool FindState(State state)
    {
        return _stateArr.Contains(state);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            _curPos = (Vector3)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void ModifyHp(float value)
    {
        if(FindState(State.DIE))
            return;

        _curHp += value;
        if (_curHp > _info.Hp)
            _curHp = _info.Hp;
        else if (_curHp <= 0)
        {
            _curHp = 0;
            Die();
        }

        _hpBar.fillAmount = _curHp / _info.Hp;
    }

    void Die()
    {
        foreach (var state in _stateArr)
            photonView.RPC("ExitState", RpcTarget.AllBuffered, state);
        photonView.RPC("EnterState", RpcTarget.AllBuffered, State.DIE);
    }
}