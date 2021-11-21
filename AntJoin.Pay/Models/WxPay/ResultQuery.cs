namespace AntJoin.Pay.Models.WxPay
{
    internal class ResultQuery : BaseResult
    {
        public ResultQuery(string xml) : base(xml)
        {
        }
        /// <summary>
        /// 交易类型
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string trade_type { get; set; }

        /// <summary>
        /// 交易状态
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string trade_state { get; set; }

        /// <summary>
        /// 标价金额，订单总金额，单位为分 
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string total_fee { get; set; }

        /// <summary>
        /// 应结订单金额，当订单使用了免充值型优惠券后返回该参数，应结订单金额=订单金额-免充值优惠券金额。
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string settlement_total_fee { get; set; }

        /// <summary>
        /// 微信支付订单号 
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string transaction_id { get; set; }

        /// <summary>
        /// 支付完成时间 订单支付时间，格式为yyyyMMddHHmmss，如2009年12月25日9点10分10秒表示为20091225091010。
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string time_end { get; set; }

        /// <summary>
        /// 交易状态描述  对当前查询订单状态的描述和下一步操作的指引
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string trade_state_desc { get; set; }
    }
}
