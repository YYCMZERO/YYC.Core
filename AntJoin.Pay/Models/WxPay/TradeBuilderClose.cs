namespace AntJoin.Pay.Models.WxPay
{
    internal class TradeBuilderClose : BaseTradeBuilder
    {
        public TradeBuilderClose(string outTradeNo) : base()
        {
            out_trade_no = outTradeNo;
        }
        /// <summary>
        /// 商户订单号，必填
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string out_trade_no { get; set; }
    }
}
