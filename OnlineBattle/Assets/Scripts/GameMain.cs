using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NetManager.Instance.AddListener("Enter", OnEnter);
        NetManager.Instance.AddListener("Move", OnMove);
        NetManager.Instance.AddListener("Leave", OnLeave);
        NetManager.Instance.Connect("127.0.0.1", 8888);
    }

    private void Update()
    {
        NetManager.Instance.OnUpdate();
    }

    void OnEnter(string msg)
    {
        Debug.Log("OnEnter"+ msg);
    }

    void OnMove(string msg)
    {
        Debug.Log("OnMove" + msg);
    }

    void OnLeave(string msg)
    {
        Debug.Log("OnLeave" + msg);
    }

}
