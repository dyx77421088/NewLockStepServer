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
        private static int port = 5000;
        private static UdpClient udpServer = new UdpClient(port);
        // 用来存放客户端的连接
        private static List<IPEndPoint> clients = new List<IPEndPoint>();
        public static void Start()
        {
            Console.WriteLine("UDP 聊天服务器已启动，等待消息...");

            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, port);

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
                    HandleBroadcastMessage(receivedData, clientEndPoint);
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
        private static void HandleBroadcastMessage(byte[] msg, IPEndPoint client)
        {
            // 如果这个客户端没有被添加进来，那么就添加到这个集合中
            if (!clients.Contains(client))
            {
                clients.Add(client);
            }
            // 把这个消息广播给所有的客户端（出去发送者）
            foreach (IPEndPoint endpoint in clients)
            {
                
                if (!endpoint.Equals(client))
                {
                    udpServer.Send(msg, msg.Length, endpoint);
                }
            }
        }
    }
}
