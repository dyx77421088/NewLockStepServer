using Commit.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;

namespace LockStepServer.Server
{
    public class UdpServer
    {
        private static UdpClient udpServer = new UdpClient(NetConfig.UDP_PORT);
        // 用来存放客户端的连接
        private static List<IPEndPoint> clients = new List<IPEndPoint>();
        public static void Start()
        {
            Console.WriteLine("UDP 聊天服务器已启动，等待消息...");

            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, NetConfig.UDP_PORT);

            //const uint IOC_IN = 0x80000000;
            //int IOC_VENDOR = 0x18000000;
            //int SIO_UDP_CONNRESET = (int)(IOC_IN | IOC_VENDOR | 12);

            ////因为我使用的是UdpClient, 所以先get出Socket（Client）来。
            //udpServer.Client.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
            ReceiveMessages();
        }



        /// <summary>
        /// 循环接收信息
        /// </summary>
        private static void ReceiveMessages()
        {
            // 接收所有ip发送过来的UDP消息
            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            // 一直接收消息
            while (true)
            {
                try
                {
                    byte[] receivedData = udpServer.Receive(ref clientEndPoint);
                    Console.WriteLine(Encoding.UTF8.GetString(receivedData));
                    // 处理接收到的消息
                    HandleLoginMessage(receivedData, clientEndPoint);
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"SocketException: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }
        }
        /// <summary>
        /// 处理接收到的消息
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="client">发送这个消息的客户端</param>
        private static void HandleBroadcastMessage()
        {

        }

        private static void HandleLoginMessage(byte[] loginInfo, IPEndPoint client)
        {
            string[] strs = Encoding.UTF8.GetString(loginInfo).Split(' ');
            string message = CheckLogin(strs[0], strs[1]) ? "登陆成功" : "用户名或密码错误";
            byte[] msg = Encoding.UTF8.GetBytes(message);
            udpServer.Send(msg, msg.Length, client);
        }

        private static bool CheckLogin(string userName, string password)
        {
            if ("张三".Equals(userName) && "123456".Equals(password)) return true;
            else if ("里斯".Equals(userName) && "666666".Equals(password)) return true;
            else if ("admin".Equals(userName) && "admin".Equals(password)) return true;
            return false;
        }
    }
}
