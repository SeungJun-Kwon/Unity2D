using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : UnitController
{
    [SerializeField] Vector2 _attackRange;

    [HideInInspector] public BoxCollider2D _weaponCol;

    public PlayerManager _playerManager;

    protected override void Awake()
    {
        base.Awake();
        _stateDic.Add(State.IDLE, new PlayerIdle(this));
        _stateDic.Add(State.MOVE, new PlayerMove(this));
        _stateDic.Add(State.ATTACK, new PlayerAttack(this));

        TryGetComponent(out _playerManager);

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
                if (_weaponCol && _weaponCol.isActiveAndEnabled)
                    _weaponCol.enabled = false;
                break;
            }
        }
    }

    private void Start()
    {
        _nameText.text = _photonView.IsMine ? PhotonNetwork.NickName : _photonView.Owner.NickName;
        _nameText.color = _photonView.IsMine ? Color.green : Color.red;

        if (photonView.IsMine)
        {
            photonView.RPC("LoadUserData", RpcTarget.AllBuffered);
            _playerManager.photonView.RPC("LoadUserData", RpcTarget.AllBuffered, FirebaseAuthManager.Instance._user.Email);
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
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Debug.Log(_info.Atk);
            }
        }
    }

    [PunRPC]
    public async void LoadUserData()
    {
        _info = await FirebaseFirestoreManager.Instance.LoadUserInfo(FirebaseAuthManager.Instance._user.Email);
        _curHp = _info.Hp;
        _curMp = _info.Mp;
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
        _unitGameUI.transform.localScale = new Vector2(-moveX, 1f);
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
                    enemyController.Hurt(_playerManager._userInfo.Atk);
                }
            }
        }
    }
}
