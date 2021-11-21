namespace AntJoin.Pay.Models.WxPay
{
    internal class TradeBuilderRefund : BaseTradeBuilder
    {
        public TradeBuilderRefund(RefundInput input) : base()
        {
            out_trade_no = input.TradeNo;
            out_refund_no = input.RefundNo;
            total_fee = (int)(input.Amount * 100);
            refund_fee = (int)(input.RefundAmount * 100);
            refund_desc = input.RefundReason;
            notify_url = input.NotifyUrl;
        }
        /// <summary>
        /// 商户订单号，必填
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string out_trade_no { get; set; }

        /// <summary>
        /// 商户退款单号，必填，商户系统内部的退款单号，商户系统内部唯一，只能是数字、大小写字母_-|*@ ，同一退款单号多次请求只退一笔。
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string out_refund_no { get; set; }

        /// <summary>
        /// 订单金额，必填，订单总金额，单位为分，只能为整数
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public int total_fee { get; set; }

        /// <summary>
        /// 退款金额，必填，退款总金额，订单总金额，单位为分，只能为整数
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public int refund_fee { get; set; }

        /// <summary>
        /// 退款原因，可选，若商户传入，会在下发给用户的退款消息中体现退款原因
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string refund_desc { get; set; }

        /// <summary>
        /// 退款结果通知url，可选，异步接收微信支付退款结果通知的回调地址，通知URL必须为外网可访问的url，不允许带参数
        /// 如果参数中传了notify_url，则商户平台上配置的回调地址将不会生效。
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string notify_url { get; set; }
    }
}
