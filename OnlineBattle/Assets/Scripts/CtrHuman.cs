using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrHuman : BaseHuman
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray,out RaycastHit hit);
            if(hit.collider.tag == "Terrain")
            {
                MoveTo(hit.point);
                //发送移动协议
                //告知协议名称 客户端身份 参数信息
                NetManager.Instance.Send("Enter|127.1.1.1,100,200,300,45");
            }
        }
    }
}
