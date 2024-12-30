using Commit.Utils;
using System;
using System.Collections.Generic;
using System.Net;

namespace LockStepServer.Server
{
    public partial class UdpServer
    {
        // 用来存放客户端的连接
        private static Dictionary<int, IPEndPoint> clients = new Dictionary<int, IPEndPoint>();
        // 处理登陆请求
        private static void HandleLoginMessage(BaseRequest request, IPEndPoint client)
        {
            // 服务器响应客户端
            BaseResponse response = new BaseResponse()
            {
                ResponseType = ResponseType.RtLogin,
                ResponseData = ResponseData.RdStatus,
                Status = new Status()
            };
            int userId = CheckLogin(request.User.Name, request.User.Password);
            if (userId != -1)
            {
                response.Status.St = StatusType.StSuccess;
                response.Status.Msg = "登陆成功！";
                response.Status.Id = userId;
                if (!clients.ContainsKey(userId))
                {
                    clients.Add(userId, client);
                }
                else // 如果该用户原本就在里面的，那么如果对局开始了就要追帧
                {
                    // 替换
                    clients[userId] = client;
                    // 返回的状态为重连
                    response.Status.St = StatusType.StReload;
                    Console.WriteLine("重连！！");
                }
                    
                Console.WriteLine("添加一个用户:" + userId);
            }
            else
            {
                response.Status.St = StatusType.StError;
                response.Status.Msg = "用户名或密码错误";
                Console.WriteLine("用户名或密码错误:" + request.User.Name);
            }
            byte[] msg = ProtoBufUtils.SerializeBaseResponse(response);
            udpServer.Send(msg, msg.Length, client);
        }

        private static int CheckLogin(string userName, string password)
        {
            if ("张三".Equals(userName) && "123456".Equals(password)) return 1;
            else if ("里斯".Equals(userName) && "666666".Equals(password)) return 2;
            else if ("admin".Equals(userName) && "admin".Equals(password)) return 3;
            else if ("123".Equals(userName) && "123".Equals(password)) return 4;
            else if ("111".Equals(userName) && "111".Equals(password)) return 5;
            else if ("222".Equals(userName) && "222".Equals(password)) return 6;
            else if ("1".Equals(userName) && "1".Equals(password)) return 7;
            else if ("2".Equals(userName) && "2".Equals(password)) return 8;
            else if ("3".Equals(userName) && "3".Equals(password)) return 9;
            return -1;
        }
    }
}
