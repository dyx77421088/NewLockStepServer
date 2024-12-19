using LockStepServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockStepServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            UdpServer.Start();
        }
    }
}
