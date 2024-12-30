
using Commit.Config;
using Commit.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LockStepServer.Server
{
    public partial class UdpServer
    {
        private static List<Operate> frames = new List<Operate>();
        private static Operate currentOperate;
        private static int frameCount = 0;
        public static void PreStart()
        {
            BaseResponse response = new BaseResponse()
            {
                ResponseType = ResponseType.RtOperate,
                ResponseData = ResponseData.RdOperate,
                Operate = new Operate()
                {
                    FrameId = 0,
                }
            };
            foreach(int i in matchSet)
            {
                response.Operate.Move.Add(new Move()
                {
                    UserId = i,
                    MoveX = 0,
                    MoveY = 0,
                });
            }
            frames.Add(response.Operate);
            foreach (var item in matchSet)
            {
                byte[] msg = ProtoBufUtils.SerializeBaseResponse(response);
                udpServer.Send(msg, msg.Length, clients[item]);
            }
            currentOperate = new Operate();
            frameCount++; // 下一次要收第一帧的数据
            currentOperate.FrameId = frameCount;
        }
        // 开始对战
        public static async Task StartMatch()
        {
            PreStart();
            // 1000 / 50 = 20    
            //new Timer(TimerCallback, null, 0, 50); // 定时器
            while (true)
            {
                await Task.Delay(50); // 
                TimerCallback(null);
            }
        }

        public static void TimerCallback(Object o)
        {
            Console.WriteLine("第" + frameCount + "帧:" + currentOperate);
            BaseResponse response = new BaseResponse()
            {
                ResponseType = ResponseType.RtOperate,
                ResponseData = ResponseData.RdOperate,
                Operate = currentOperate,
            };
            frames.Add(response.Operate);
            foreach (var item in matchSet)
            {
                byte[] msg = ProtoBufUtils.SerializeBaseResponse(response);
                udpServer.Send(msg, msg.Length, clients[item]);
            }
            currentOperate = new Operate();
            frameCount++;
            currentOperate.FrameId = frameCount;

        }

        // 收到玩家的操作
        private static void HandleOperate(BaseRequest request, IPEndPoint client)
        {
            if (request.Operate.FrameId < frameCount) return;
            if (request.Operate.Move.Count > 0)
            {
                currentOperate.Move.Add(request.Operate.Move[0]);
            }
        }
    }
}
