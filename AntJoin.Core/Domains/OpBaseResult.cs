using Newtonsoft.Json;
using System;

namespace AntJoin.Core.Domains
{
    /// <summary>
    /// 操作结果类
    /// </summary>
    [Serializable]
    public sealed class OpBaseResult : OpBaseResult<object>
    {
        /// <summary>
        /// 设置结果
        /// </summary>
        public static new OpBaseResult Success(object data = default)
        {
            var ret = new OpBaseResult
            {
                Status = 1,
                Message = "操作成功",
                Data = data,
                Code = null
            };
            return ret;
        }


        /// <summary>
        /// 设置结果
        /// </summary>
        public static new OpBaseResult Fail(int status, string message, object data = default, int? code = null)
        {
            var ret = new OpBaseResult
            {
                Status = status,
                Message = message,
                Data = data,
                Code = code
            };
            return ret;
        }
    }

   

    /// <summary>
    /// 基类返回结果
    /// </summary>
    [Serializable]
    public class OpBaseResult<T>
    {
        /// <summary>
        /// 状态
        /// </summary>
        [JsonProperty("status")]
        public int Status { get; set; }


        /// <summary>
        /// 消息
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }


        /// <summary>
        /// 服务器时间
        /// </summary>
        [JsonProperty("serverTime")]
        public DateTime ServerTime { get; set; } = DateTime.Now;


        /// <summary>
        /// 错误状态
        /// </summary>
        [JsonProperty("code")]
        public int? Code { get; set; }


        /// <summary>
        /// 数据
        /// </summary>
        [JsonProperty("data")]
        public T Data { get; set; }


        /// <summary>
        /// 设置结果
        /// </summary>
        public static OpBaseResult<T> Success(T data = default)
        {
            var ret = new OpBaseResult<T>
            {
                Status = 1,
                Message = "操作成功",
                Data = data,
                Code = null
            };
            return ret;
        }


        /// <summary>
        /// 设置结果
        /// </summary>
        public static OpBaseResult<T> Fail(int status, string message, T data = default, int? code = null)
        {
            var ret = new OpBaseResult<T>
            {
                Status = status,
                Message = message,
                Data = data,
                Code = code
            };
            return ret;
        }
    }
}
