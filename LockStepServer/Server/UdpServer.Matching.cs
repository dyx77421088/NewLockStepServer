using Commit.Config;
using Commit.Utils;
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
    public partial class UdpServer
    {
        static HashSet<int> matchSet = new HashSet<int>();
        static int matchCount = 2; // 多少个人一局
        // 匹配
        private static void HandleMatching(BaseRequest request, IPEndPoint client)
        {
            // 响应
            BaseResponse response = new BaseResponse()
            {
                ResponseType = ResponseType.RtMatch,
                ResponseData = ResponseData.RdStatus,
                Status = new Status()
                {
                    St = StatusType.StSuccess,
                    Msg = request.Matching.IsMatch ? "正在匹配中..." : "取消匹配成功！"
                }
            };
            if (request.Matching.IsMatch)
            {
                matchSet.Add(request.Matching.UserId);
                Console.WriteLine("新增一个匹配的:" + request.Matching.UserId);
                if (matchSet.Count >= matchCount)
                {
                    StartMatch(); // 异步
                    Console.WriteLine("人数足够，可以开始了");
                }
            }
            else
            {
                matchSet.Remove(request.Matching.UserId);
                Console.WriteLine("移除一个匹配的:" + request.Matching.UserId);
            }
        }

        // 追帧
        private static void HandleReLoad(BaseRequest request, IPEndPoint client)
        {
            Console.WriteLine("进来追帧了" + request);
            if (!matchSet.Contains(request.Status.Id)) 
                matchSet.Add(request.Status.Id);
            BaseResponse response = new BaseResponse()
            {
                ResponseType = ResponseType.RtOperate,
                ResponseData = ResponseData.RdChasingframes,
                ChasingFrames = new ChasingFrames()
            };
            response.ChasingFrames.Operates.Add(frames);
            byte[] msg = ProtoBufUtils.SerializeBaseResponse(response);
            udpServer.Send(msg, msg.Length, client);
        }
    }

}
