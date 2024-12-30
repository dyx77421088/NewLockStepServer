using Google.Protobuf;

namespace Commit.Utils
{
    /// <summary>
    /// 序列化相关的工具类
    /// </summary>
    public class ProtoBufUtils
    {
        public static BaseRequest DeSerializeBaseRequest(byte[] data)
        {
            return BaseRequest.Parser.ParseFrom(data);
        }
        public static BaseResponse DeSerializeBaseResponse(byte[] data)
        {
            return BaseResponse.Parser.ParseFrom(data);
        }
        public static byte[] SerializeBaseRequest(BaseRequest baseRequest)
        {
            return baseRequest.ToByteArray();
        }
        public static byte[] SerializeBaseResponse(BaseResponse response)
        {
            return response.ToByteArray();
        }
    }
}
