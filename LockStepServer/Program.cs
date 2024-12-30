using LockStepServer.Server;
using System.Diagnostics;
using System.IO;
using System;
using System.Collections.Generic;
using Commit.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace LockStepServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            UdpServer.Start();

            // protobuf转c#
            //Protobuf2Cs();

            // 2236
            //new Timer(TimerCallback, null, 0, 1); // 定时器
            Console.Read();
        }
        private static List<Operate> frames = new List<Operate>();
        private static Operate currentOperate;
        private static int idx;
        private static void TimerCallback(Object o)
        {
            Console.WriteLine(idx++ + " " + currentOperate);
            BaseResponse response = new BaseResponse()
            {
                ResponseType = ResponseType.RtOperate,
                ResponseData = ResponseData.RdOperate,
                Operate = currentOperate,
            };
            frames.Add(ProtoBufUtils.DeSerializeBaseResponse(ProtoBufUtils.SerializeBaseResponse(response)).Operate);
            currentOperate = new Operate();
            currentOperate.FrameId = idx;
            currentOperate.Move.Add(new Move()
            {
                UserId = 1,
                MoveX = (float)new Random().NextDouble(),
                MoveY = (float)new Random().NextDouble(),
            });
        }

        // .proto转换为csharp
        static void Protobuf2Cs()
        {
            string protoDirectory = @"C:\Users\WIN\Desktop\unityProject\LockStepServer\Commit\Proto"; // 替换为你的.proto文件目录路径
            string outputDirectory = @"C:\Users\WIN\Desktop\unityProject\LockStepServer\Commit\Proto\output"; // 替换为输出目录

            // 确保输出目录存在
            Directory.CreateDirectory(outputDirectory);

            // 获取所有 .proto 文件
            string[] protoFiles = Directory.GetFiles(protoDirectory, "*.proto");

            foreach (string protoFile in protoFiles)
            {
                ConvertProtoToCs(protoFile, outputDirectory);
            }

            Console.WriteLine("所有 .proto 文件已成功转换为 C#。");
        }

        static void ConvertProtoToCs(string protoFile, string outputDirectory)
        {
            //protoc--csharp_out =.person.proto

            // 获取 .proto 文件所在目录
            string protoDirectory = Path.GetDirectoryName(protoFile);

            // 运行 protoc 命令
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "protoc",
                Arguments = $"--proto_path=\"{protoDirectory}\" --csharp_out=\"{outputDirectory}\" \"{protoFile}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processStartInfo))
            {
                process.WaitForExit();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                if (process.ExitCode != 0)
                {
                    Console.WriteLine($"转换文件 {protoFile} 时出错: {error}");
                }
                else
                {
                    Console.WriteLine($"成功转换: {protoFile}");
                }
            }
        }
    
    
    }
}
