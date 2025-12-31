using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBattleServer
{
    public class EventHandler
    {
        public static void OnDisconnect(ClientState state)
        {
            Console.WriteLine("OnDisconnect");
        }
    }
}
