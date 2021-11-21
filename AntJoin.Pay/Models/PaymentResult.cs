using System;

namespace AntJoin.Pay.Models
{
    /// <summary>
    /// 返回支付结果
    /// </summary>
    public class PaymentResult
    {
        public PaymentResult()
        {
            TimeStamp = GetTimestamp();
        }

        public PaymentResult(string appid, string publicKey, string tradeNo = null)
        {
            TradeNo = tradeNo;
            AppId = appid;
            PublicKey = publicKey;
            TimeStamp = GetTimestamp();
        }

        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppId { get; set; }


        /// <summary>
        /// 公钥\商户号
        /// </summary>
        public string PublicKey { get; set; }


        /// <summary>
        /// 秒级时间戳
        /// </summary>
        public long TimeStamp { get; set; }


        /// <summary>
        /// 统一下单接口返回的 prepay_id 参数值
        /// </summary>
        public string PrepayId { get; set; }


        /// <summary>
        /// 随机字符串
        /// </summary>
        public string Noncestr { get; set; }


        /// <summary>
        /// 签名类型
        /// </summary>
        public string SignType { get; set; }


        /// <summary>
        /// 签名
        /// </summary>
        public string PaySign { get; set; }


        /// <summary>
        /// 订单编号
        /// </summary>
        public string TradeNo { get; set; }


        /// <summary>
        /// form
        /// </summary>
        public string Form { get; set; }


        /// <summary> 
        /// 将DateTime时间格式转换为Unix时间戳格式
        /// </summary> 
        /// <param name="time"> 时间 </param> 
        /// <returns> double </returns> 
        private long GetTimestamp()
        {
            DateTime dtStart = new DateTime(1970, 1, 1);
            return (DateTime.Now.Ticks - dtStart.Ticks) / 10000000;
        }
    }
}
