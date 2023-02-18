using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : IState
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
        if(_count > _attackDuration)
        {
            _unitController.ExitState(State.ATTACK);
            _unitController.EnterState(State.IDLE);
        }
        Debug.DrawRay(_unitController._collider.bounds.center, Vector2.right * _unitController.transform.localScale.x * -1, Color.red);
    }
}
