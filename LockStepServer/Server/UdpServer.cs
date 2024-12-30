using Commit.Config;
using Commit.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;

namespace LockStepServer.Server
{
    public partial class UdpServer
    {
        private static UdpClient udpServer = new UdpClient(NetConfig.UDP_PORT);
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
                    BaseRequest requset = ProtoBufUtils.DeSerializeBaseRequest(receivedData);
                    // 处理接收到的消息
                    HandleReceivedData(requset, clientEndPoint);
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
        // 根据不同的请求类型和数据类型到对应的处理
        private static void HandleReceivedData(BaseRequest request, IPEndPoint client)
        {
            if (request.RequestType == RequestType.RtLogin) // 如果是登陆请求
            {
                if (request.RequestData == RequestData.RdUser) // 且携带的数据是user
                {
                    HandleLoginMessage(request, client); // 处理登陆请求
                }
            }
            else if (request.RequestType == RequestType.RtMatch)
            {
                if (request.RequestData == RequestData.RdMatch)
                {
                    HandleMatching(request, client); // 处理登陆请求
                }
                else if (request.RequestData == RequestData.RdStatus)
                {
                    if (request.Status.St == StatusType.StReload)
                    {
                        HandleReLoad(request, client); // 追帧
                    }
                }
            }
            else if (request.RequestType == RequestType.RtOperate)
            {
                if (request.RequestData == RequestData.RdOperate)
                {
                    HandleOperate(request, client); // 处理玩家发送过来的操作
                }
            }
        }

    }
}
