using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    //接收缓存 解决粘包问题
    private List<byte> cache = new();

    //添加监听
    public void AddListener(string msgName,MsgListener listener)
    {
        listeners[msgName] = listener;
    }

    //获取描述
    public string GetSelfIP()
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

            if (count <= 0)
            {
                Debug.Log("Server closed connection");
                return;
            }
            cache.AddRange(readBuff.AsSpan(0,count).ToArray());
            while (true)
            {
                if (cache.Count < 2) break;
                Int16 bodyLen = BitConverter.ToInt16(cache.ToArray(), 0);
                if (cache.Count < 2 + bodyLen) break;
                // 取出消息体
                byte[] bodyBytes = cache
                    .GetRange(2, bodyLen)
                    .ToArray();
                // 移除已处理数据
                cache.RemoveRange(0, 2 + bodyLen);

                // 转成字符串，入消息队列
                string msg = System.Text.Encoding.UTF8.GetString(bodyBytes);
                msgList.Add(msg);
            }
            socket.BeginReceive(readBuff,0, 1024, 0,ReceiveCallback, socket);
        }catch(SocketException ex)
        {
            Debug.LogError("Socket Receive Fail" + ex.ToString());
        }
    }

    //发送
    public void Send(string sendStr)
    {
        if (socket == null || !socket.Connected) return;
        byte[] bodyBytes = System.Text.Encoding.UTF8.GetBytes(sendStr);//请求体
        Int16 len =(Int16)bodyBytes.Length;
        byte[] lenBytes = BitConverter.GetBytes(len);//长度标识
        byte[] sendBytes = lenBytes.Concat(bodyBytes).ToArray();
        socket.Send(sendBytes);
    }

    public void SendMove(Vector3 pos,Vector3 rot)
    {
        string sendStr =
            $"Move|{NetManager.Instance.GetSelfIP()},{pos.x},{pos.y},{pos.z},{rot.y}";
        Send(sendStr);
    }

    public void SendEnter(Vector3 pos, Vector3 rot)
    {
        string sendStr =
            $"Enter|{NetManager.Instance.GetSelfIP()},{pos.x},{pos.y},{pos.z},{rot.y}";
        Send(sendStr);
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
