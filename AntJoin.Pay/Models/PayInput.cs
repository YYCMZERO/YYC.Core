namespace AntJoin.Pay.Models
{
    /// <summary>
    /// 发起支付参数接口
    /// </summary>
    public class PayInput
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string TradeNo { get; set; }


        /// <summary>
        /// 订单名称
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 订单描述
        /// </summary>
        public string Body { get; set; }


        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal Amount { get; set; }


        /// <summary>
        /// 过期时间，分钟
        /// </summary>
        public int TimeExpress { get; set; }


        /// <summary>
        /// 用户IP地址
        /// </summary>
        public string ClientIp { get; set; }


        /// <summary>
        /// 授权码，扫用户付款码支付必须有的值，设备读取用户微信中的条码或者二维码信息
        /// </summary>
        public string AuthCode { get; set; }


        /// <summary>
        /// 用户标识，当为小程序的时候，此参数必传，用户在商户appid下的唯一标识。
        /// </summary>
        public string OpenId { get; set; }


        /// <summary>
        /// 成功回调地址
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}
