using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    [SerializeField]
    HumanFactory playerFactory = default;

    private CtrHuman selfPlayer = default;
    private Dictionary<string, BaseHuman> OtherPlayers = new();

    // Start is called before the first frame update
    void Start()
    {
        NetManager.Instance.AddListener("Enter", OnEnter);
        NetManager.Instance.AddListener("Move", OnMove);
        NetManager.Instance.AddListener("Leave", OnLeave);
        NetManager.Instance.Connect("127.0.0.1", 8888);
        SpawnSelfPlayer();
    }

    private void SpawnSelfPlayer()
    {
        selfPlayer = playerFactory.Get(NetManager.Instance.GetIP()) as CtrHuman;
        selfPlayer.SendEnterInfo();
    }

    private void Update()
    {
        HndleInput();
        NetManager.Instance.OnUpdate();

        if(selfPlayer!=null)
            selfPlayer.GameUpdate();
    }

    private void HndleInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit hit);
            if (hit.collider.tag == "Terrain")
            {
                //Debug.Log("hit point:" + hit.point);
                selfPlayer.MoveTo(hit.point);
                //发送移动协议
                //告知协议名称 客户端身份 参数信息
                NetManager.Instance.Send("Move|127.1.1.1,100,200,300,45");
            }
        }
    }

    void OnEnter(string msg)
    {
        Debug.Log("OnEnter" + msg._LogRed());
    }

    void OnMove(string msg)
    {
        Debug.Log("OnMove" + msg._LogRed());
    }

    void OnLeave(string msg)
    {
        Debug.Log("OnLeave" + msg._LogRed());
    }

}
