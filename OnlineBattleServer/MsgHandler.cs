using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBattleServer
{
    public class MsgHandler
    {
        public static void MsgEnter(ClientState client,string msgArgs)
        {
            string[] split = msgArgs.Split(',');
            string desc = split[0];
            float x = float.Parse(split[1]);
            float y = float.Parse(split[2]);
            float z = float.Parse(split[3]);
            float eulY = float.Parse(split[4]);
            client.RefreshPlayerInfo(100, x, y, z, eulY);
            //广播
            string sendStr = "Enter|" + msgArgs;
            foreach(ClientState state in MainClass.Clients.Values)
            {
                MainClass.Send(state,sendStr);
            }
            //Console.WriteLine("MsgEnter" + msgArgs);
        }
        public static void MsgList(ClientState c, string msgArgs)
        {
            string sendStr = "List|";
            foreach(ClientState state in MainClass.Clients.Values)
            {
                sendStr += state.socket.RemoteEndPoint.ToString()+",";
                sendStr += state.x.ToString() + ",";
                sendStr += state.y.ToString()+",";
                sendStr += state.z.ToString()+",";
                sendStr += state.eulY.ToString()+",";
                sendStr += state.hp.ToString()+",";
            }
            Console.WriteLine("send:"+sendStr);
            MainClass.Send(c,sendStr);
           // Console.WriteLine("MsgList" + msgArgs);
        }

        public static void MsgMove(ClientState c, string msgArgs)
        {
            string[] split = msgArgs.Split(',');
            string desc = split[0];
            float x = float.Parse(split[1]);
            float y = float.Parse(split[2]);
            float z = float.Parse(split[3]);  
            float eulY = float.Parse(split[4]); 
            c.RefreshPlayerInfo(x,y,z,eulY);
            string sendStr = "Move|" + msgArgs;
            foreach (var cs in MainClass.Clients.Values)
            {
                MainClass.Send(cs,sendStr);
            }
            //Console.WriteLine("MsgMove" + msgArgs);
        }
    }
}
