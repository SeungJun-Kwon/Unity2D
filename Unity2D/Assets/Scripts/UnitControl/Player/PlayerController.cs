using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using Unity.VisualScripting;

public class PlayerController : UnitController
{
    [SerializeField] Text _nickName;
    [SerializeField] Image _hpBar;
    [SerializeField] Vector2 _attackRange;

    [HideInInspector] public BoxCollider2D _weaponCol;

    Canvas _canvas;

    protected override void Awake()
    {
        base.Awake();
        _stateDic.Add(State.IDLE, new PlayerIdle(this));
        _stateDic.Add(State.MOVE, new PlayerMove(this));
        _stateDic.Add(State.ATTACK, new PlayerAttack(this));

        transform.Find("Canvas").gameObject.TryGetComponent(out _canvas);
        _nickName.text = _photonView.IsMine ? PhotonNetwork.NickName : _photonView.Owner.NickName;
        _nickName.color = _photonView.IsMine ? Color.green : Color.red;

        if (_photonView.IsMine)
        {
            EnterState(State.IDLE);
            GameObject.Find("CMvcam").TryGetComponent(out CinemachineVirtualCamera cm);
            cm.Follow = transform;
            cm.LookAt = transform;
        }

        foreach (Transform i in GetComponentsInChildren<Transform>())
        {
            if (i.name == "Weapon")
            {
                i.gameObject.TryGetComponent(out _weaponCol);
                //if (_weaponCol && _weaponCol.isActiveAndEnabled)
                //    _weaponCol.enabled = false;
                break;
            }
        }
    }

    protected override void Update()
    {
        base.Update();
        if (_photonView.IsMine)
        {
            if (Input.GetKey(KeyCode.LeftAlt) && IsGrounded)
            {
                EnterState(State.JUMP);
            }
            if (Input.GetButton("Attack"))
                Attack();
        }
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

    [PunRPC]
    public override void MoveXRPC(float moveX)
    {
        base.MoveXRPC(moveX);
        _canvas.transform.localScale = new Vector2(-moveX, 1f);
    }

    public void TryAttack()
    {
        var hit = Physics2D.BoxCastAll(_collider.bounds.center, new Vector2(0.1f, _attackRange.y), 0,
                                        Vector2.right * transform.localScale.x * -1, _attackRange.x, LayerMask.GetMask("Enemy"));

        if (hit.Length > 0)
        {
            foreach (var i in hit)
            {
                if (i.transform.gameObject.TryGetComponent(out EnemyController enemyController))
                {
                    enemyController.Hurt();
                }
            }
        }
    }
}
