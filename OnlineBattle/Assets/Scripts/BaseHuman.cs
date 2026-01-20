using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaseHuman : GameBehavior
{
    public HumanFactory OriginFactory
    {
        get { return _originFactory; }
        set { _originFactory = value; }
    }
    public bool IsAttacking { get { return _isAttacking; }}
    public bool IsMoving { get { return _isMoving; }}

    public string Desc = "";

    private float _speed;
    private float _health;
    private float _attackCD;
    private float _attackTime;

    private bool _isMoving = false;
    private bool _isAttacking = false;

    private Vector3 _targetPosition;

    private Animator _animator;
    private HumanFactory _originFactory;

    public void Initialize(float speed,float health,Vector3 bornPosition,float eulY,string desc,
        float attackCD)
    {
        _speed = speed;
        _health = health;
        transform.localPosition = bornPosition;
        transform.localEulerAngles = new Vector3(0, eulY, 0);
        Desc = desc;
        _attackCD = attackCD;
        _animator = GetComponent<Animator>();
    }

    public override void Recycle()
    {
        //_animator.Stop
        _originFactory.Reclaim(this);
    }

    public void Attack()
    {
        _isAttacking = true;
        _attackTime = Time.time;
        _animator.SetBool("isAttacking", true);
    }

    public void MoveTo(Vector3 pos)
    {
        _targetPosition = pos;
        _isMoving = true;
        _animator.SetBool("isMoving", true);
    }

    //被销毁时返回false
    public override void GameUpdate()
    {
        MoveUpdate();
        AttackUpdate();
    }

    private void MoveUpdate()
    {
        if (_isMoving == false) return;

        transform.position = Vector3.MoveTowards(
            transform.position, _targetPosition,
            _speed * Time.deltaTime);
        transform.LookAt(_targetPosition);
        if (Vector3.Distance(transform.position, _targetPosition) < 0.05f)
        {
            _isMoving = false;
            _animator.SetBool("isMoving", false);
        }
        return;
    }

    private void AttackUpdate()
    {
        if( _isAttacking == false) return;
        if (Time.time - _attackTime < _attackCD) return;
        _isAttacking = false;
        _animator.SetBool("isAttacking", false);
    }

}
