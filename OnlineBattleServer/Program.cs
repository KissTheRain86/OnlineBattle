using System;
using System.Net;
using System.Net.Sockets;

namespace OnlineBattleServer
{
    class MainClass
    {
        //监听Socket
        static Socket listenfd;
        //客户端Socket及状态信息
        static Dictionary<Socket, ClientState> clients = new();

        //checkReadList
        static List<Socket> checkReadList = new List<Socket>();
        public static void Main(string[] args)
        {
            // 建立连接socket          
            listenfd = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            //bind
            IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEdp = new IPEndPoint(ipAddr, 8888);
            listenfd.Bind(ipEdp);
            //listen
            listenfd.Listen(0);
            Console.WriteLine("Server Start");

            while (true)
            {
                //填充checkRead列表
                checkReadList.Clear();
                checkReadList.Add(listenfd);
                foreach (ClientState s in clients.Values)
                {
                    checkReadList.Add(s.socket);
                }
                //Select 多路复用 一次性检查多个socket是否可读、可写
                Socket.Select(checkReadList, null, null, 1000);
                foreach (Socket s in checkReadList)
                {
                    if (s == listenfd)
                    {
                        //负责监听的socket
                        //有新的客户端连接 
                        ReadListenfd(s);
                    }
                    else
                    {
                        //负责读的socket
                        ReadClientfd(s);
                    }
                }
            }
        }

        public static void ReadListenfd(Socket listenfd)
        {
            Console.WriteLine("Server Accept");
            Socket clientfd = listenfd.Accept();
            ClientState state = new ClientState();
            state.socket = clientfd;
            clients.Add(clientfd, state);
        }

        public static bool ReadClientfd(Socket clientfd)
        {
            ClientState state = clients[clientfd];
            //接收
            int count = 0;
            try
            {
                count = clientfd.Receive(state.readBuff);
            }
            catch (SocketException ex)
            {
                clientfd.Close();
                clients.Remove(clientfd);
                Console.WriteLine("Recive SocketException" + ex.ToString());
                return false;
            }
            //客户端关闭
            if (count == 0)
            {
                clientfd.Close();
                clients.Remove(clientfd);
                Console.WriteLine("Socket Close");
                return false;
            }
            //收到客户端信息 广播
            string recvStr = System.Text.Encoding.UTF8.GetString(state.readBuff, 0, count);
            Console.WriteLine("Recieve:" + recvStr);
            string sendStr = recvStr;
            byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendStr);
            foreach (ClientState clientState in clients.Values)
            {
                clientState.socket.Send(sendBytes);
            }
            return true;
        }
    }

}