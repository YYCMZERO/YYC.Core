namespace AntJoin.Pay.Models.WxPay
{
    internal class ResultRefund : BaseResult
    {
        public ResultRefund(string xml) : base(xml)
        {
        }
        /// <summary>
        /// 商户退款单号
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string out_refund_no { get; set; }

        /// <summary>
        /// 退款金额 退款总金额,单位为分,可以做部分退款
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string refund_fee { get; set; }

        /// <summary>
        /// 应结退款金额 去掉非充值代金券退款金额后的退款金额，退款金额=申请退款金额-非充值代金券退款金额，退款金额小于等于申请退款金额
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string settlement_refund_fee { get; set; }

        /// <summary>
        /// 标价金额
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string total_fee { get; set; }
    }
}
