using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    [SerializeField]
    HumanFactory humanFactory = default;

    private CtrHuman selfPlayer = default;
    private Dictionary<string, BaseHuman> otherPlayers = new();

    // Start is called before the first frame update
    void Start()
    {
        NetManager.Instance.AddListener("Enter", OnEnter);
        NetManager.Instance.AddListener("List", OnList);
        NetManager.Instance.AddListener("Move", OnMove);
        NetManager.Instance.AddListener("Leave", OnLeave);
        NetManager.Instance.Connect("127.0.0.1", 8888);
        SpawnSelfPlayer();
        //请求玩家列表
        NetManager.Instance.Send("List|");
    }

    private void SpawnSelfPlayer()
    {
        selfPlayer = humanFactory.GetSelfPlayer() as CtrHuman;
        selfPlayer.SendEnterInfo();
    }

    private void Update()
    {
        HndleInput();
        NetManager.Instance.OnUpdate();

        if(selfPlayer!=null) selfPlayer.GameUpdate();

        foreach (var otherPlayer in otherPlayers.Values)
        {
            otherPlayer.GameUpdate();
        }
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
                NetManager.Instance.SendMove(hit.point, selfPlayer.transform.eulerAngles);
            }
        }
    }
    //收到其他人进入游戏的推送
    void OnEnter(string msg)
    {
        Debug.Log("OnEnter" + msg._LogRed());

        string[] split = msg.Split(',');
        string ip = split[0];
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        Vector3 bornPos = new Vector3(x, y, z);

        if (ip == NetManager.Instance.GetSelfIP()) return;
        BaseHuman otherPlayer = humanFactory.GetOtherPlayer(bornPos,ip);
        if (otherPlayer != null)
        {
            otherPlayers[otherPlayer.Desc] = otherPlayer;
        }
    }
    //收到其他玩家列表
    void OnList(string msg)
    {
        Debug.Log("OnList" + msg._LogRed());
        string[] split = msg.Split(',');
        int count = split.Length / 6;
        for (int i = 0; i < count; i++)
        {
            string ip = split[i * 6 + 0];
            float x = float.Parse(split[i * 6 + 1]);
            float y = float.Parse(split[i * 6 + 2]);
            float z = float.Parse(split[i * 6 + 3]);
            float eulY = float.Parse(split[i * 6 + 4]);
            int hp = int.Parse(split[i * 6 + 5]);
            Vector3 bornPos = new Vector3(x, y, z);

            if (ip == NetManager.Instance.GetSelfIP()) continue;
            BaseHuman otherPlayer = humanFactory.GetOtherPlayer(bornPos, ip);
            if (otherPlayer != null)
            {
                otherPlayers[otherPlayer.Desc] = otherPlayer;
            }
        }
    }

    void OnMove(string msg)
    {
        Debug.Log("OnMove" + msg._LogRed());
        //解析参数
        string[] split = msg.Split(',');
        string desc = split[0];
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        //移动
        if (!otherPlayers.ContainsKey(desc))
            return;
        BaseHuman h = otherPlayers[desc];
        Vector3 targetPos = new Vector3(x, y, z);
        h.MoveTo(targetPos);

    }

    void OnLeave(string msg)
    {
        Debug.Log("OnLeave" + msg._LogRed());
        string[] split = msg.Split(",");
        string desc = split[0];
        if (!otherPlayers.ContainsKey(desc)) return;
        BaseHuman h = otherPlayers[desc];
        h.Recycle();
        otherPlayers.Remove(desc);
    }

}
