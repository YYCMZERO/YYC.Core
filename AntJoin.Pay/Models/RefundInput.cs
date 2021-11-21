namespace AntJoin.Pay.Models
{
    /// <summary>
    /// 发起退款传入参数
    /// </summary>
    public class RefundInput
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string TradeNo { get; set; }


        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal Amount { get; set; }


        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal RefundAmount { get; set; }


        /// <summary>
        /// 退款原因
        /// </summary>
        public string RefundReason { get; set; }


        /// <summary>
        /// 商户退款单号，必填，商户系统内部的退款单号，商户系统内部唯一，只能是数字、大小写字母_-|*@ ，同一退款单号多次请求只退一笔。
        /// </summary>
        public string RefundNo { get; set; }


        /// <summary>
        /// 微信，退款结果通知url，可选，异步接收微信支付退款结果通知的回调地址，通知URL必须为外网可访问的url，不允许带参数
        /// 如果参数中传了notify_url，则商户平台上配置的回调地址将不会生效。
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string NotifyUrl { get; set; }
    }
}
