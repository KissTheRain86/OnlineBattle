using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHuman : MonoBehaviour
{

    public string Desc = "";

    public float Speed;

    protected bool isMoving = false;

    private Vector3 targetPosition;

    private Animator animator;

    public void MoveTo(Vector3 pos)
    {
        targetPosition = pos;
        isMoving = true;
        animator.SetBool("isMoving", true);
    }

    private void MoveUpdate()
    {
        if (isMoving == false) return;
        
        transform.position = Vector3.MoveTowards(
            transform.position, targetPosition,
            Speed * Time.deltaTime);
        transform.LookAt(targetPosition);
        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            isMoving = false;
            animator.SetBool("isMoving", false);
        }
    }


    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        MoveUpdate();
    }

}
