using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHuman : GameBehavior
{
    public HumanFactory OriginFactory
    {
        get { return originFactory; }
        set { originFactory = value; }
    }

    public string Desc = "";

    private float speed;

    private float health;

    private bool isMoving = false;

    private Vector3 targetPosition;

    private Animator animator;

    private HumanFactory originFactory;

    public void Initialize(float speed,float health,Vector3 bornPosition,string desc)
    {
        this.speed = speed;
        this.health = health;
        this.transform.localPosition = bornPosition;
        this.Desc = desc;
        animator = GetComponent<Animator>();
    }

    public override void Recycle()
    {
        //animator.Stop
        originFactory.Reclaim(this);
    }

    public void MoveTo(Vector3 pos)
    {
        targetPosition = pos;
        isMoving = true;
        animator.SetBool("isMoving", true);
    }

    //被销毁时返回false
    public override bool GameUpdate()
    {
        if (isMoving == false) return true;
        
        transform.position = Vector3.MoveTowards(
            transform.position, targetPosition,
            speed * Time.deltaTime);
        transform.LookAt(targetPosition);
        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            isMoving = false;
            animator.SetBool("isMoving", false);
        }
        return true;
    }

}
