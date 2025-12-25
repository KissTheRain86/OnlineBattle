using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class NetManager : Singleton<NetManager>
{
    Socket socket;
    //客户端接收信息缓冲区
    byte[] readBuff = new byte[1024];
    //委托类型
    public delegate void MsgListener(string str);
    //监听列表
    private Dictionary<string, MsgListener> listeners = new();
    //消息队列
    private List<string> msgList = new();

    //添加监听
    public void AddListener(string msgName,MsgListener listener)
    {
        listeners[msgName] = listener;
    }

    //获取描述
    public string GetDesc()
    {
        if (socket == null || !socket.Connected) return "";
        return socket.LocalEndPoint.ToString();
    }

    public void Connect(string ip,int port)
    {
        //创建socket
        socket = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);
        //同步连接远程ip地址
        socket.Connect(ip, port);
        //开始接收消息
        socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallback, socket);
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndReceive(ar);
            string resStr = 
                System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
            //Debug.Log("Recieve Str:" + resStr);
            msgList.Add(resStr);
            socket.BeginReceive(readBuff,0, 1024, 0,
                ReceiveCallback, socket);
        }catch(SocketException ex)
        {
            Debug.LogError("Socket Receive Fail" + ex.ToString());
        }
    }

    //发送
    public void Send(string sendStr)
    {
        if (socket == null || !socket.Connected) return;
        byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendStr);
        socket.Send(sendBytes);
    }

    public void OnUpdate()
    {
        if (msgList.Count <= 0) return;
        string msgStr = msgList[0];
        msgList.RemoveAt(0);
        string[] split = msgStr.Split('|');
        string msgName = split[0];
        string msgArgs = split[1];
        if (listeners.ContainsKey(msgName))
        {
            listeners[msgName](msgArgs);
        }
    }
}
