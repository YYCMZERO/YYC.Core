namespace AntJoin.Pay.Models.AliPay
{
    internal class TradeBuilderClose : BaseTradeBuilder
    {
        public TradeBuilderClose(string outTradeNo)
        {
            out_trade_no = outTradeNo;
        }

        /// <summary>
        /// 支付宝交易号
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string trade_no { get; set; }

        /// <summary>
        /// 商家订单号
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string out_trade_no { get; set; }

        public override bool Validate()
        {
            throw new System.NotImplementedException();
        }
    }
}
