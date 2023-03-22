using Google.MiniJSON;
using Photon.Pun;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : UnitController
{
    [SerializeField] string _name;

    [HideInInspector] public Transform _target;

    Vector3 _originPos;

    protected override void Awake()
    {
        base.Awake();
        _stateDic.Add(State.IDLE, new EnemyIdle(this));
        _stateDic.Add(State.MOVE, new EnemyMove(this));
        if (photonView.IsMine)
        {
            EnterState(State.IDLE);
        }
    }

    private void Start()
    {
        _originPos = transform.position;

        photonView.RPC("LoadUnitData", RpcTarget.AllBuffered);
    }


    [PunRPC]
    public async void LoadUnitData()
    {
        _info = await FirebaseFirestoreManager.Instance.LoadUnitInfo(_name);
        if (_info == null)
        {
            gameObject.SetActive(false);
            return;
        }

        _curHp = _info.Hp;
        _curMp = _info.Mp;
    }


    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void PlayAnimation(State state)
    {
        switch (state)
        {
            case State.IDLE:
                _animator.SetBool("IsWalking", false);
                break;
            case State.MOVE:
                _animator.SetBool("IsWalking", true);
                break;
            case State.ATTACK:
                _animator.SetTrigger("Attack");
                break;
            case State.DIE:
                _animator.SetTrigger("Die");
                break;
        }
    }

    public void MoveOriginPos()
    {
        float moveX = (transform.position - _originPos).x;
        if (moveX > 0.1f || moveX < -0.1f)
        {
            if (moveX < 0)
                moveX /= -moveX;
            else
                moveX /= moveX;
            Move(-moveX);
        }
        else
        {
            ExitState(State.MOVE);
            EnterState(State.IDLE);
        }
    }

    public override void Move(float moveX)
    {
        RaycastHit2D hit;
        hit = Physics2D.BoxCast(_collider.bounds.center, new Vector2(0.1f, _collider.bounds.size.y / 2), 0f, Vector2.right * moveX, 1f, LayerMask.GetMask("Player"));
        if (hit)
            return;

        hit = Physics2D.BoxCast(_collider.bounds.center, new Vector2(0.1f, _collider.bounds.size.y / 2), 0f, Vector2.right * moveX, 1f, LayerMask.GetMask("Ground"));
        if (hit)
            EnterState(State.JUMP);

        base.Move(moveX);
    }

    public void Hurt(float damage)
    {
        _animator.SetTrigger("hurt");
        photonView.RPC("ModifyHp", RpcTarget.AllBuffered, -damage);
    }
}
