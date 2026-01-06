using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrHuman : BaseHuman
{
    public override bool GameUpdate()
    {
        return base.GameUpdate();
    }

    //进入游戏时调用 向服务器发送enter信息
    public void SendEnterInfo()
    {
        Vector3 pos = transform.position;
        float eulY = transform.eulerAngles.y;
        NetManager.Instance.SendEnter(pos, eulY);
    }
   
}
