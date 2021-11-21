namespace AntJoin.Pay.Models.WxPay
{
    internal class TradeBuilderApp : BaseTradeBuilderPay
    {
        public TradeBuilderApp(PayInput input) : base(input)
        {
        }

        public TradeBuilderApp(PayInput input, string notifyUrl) : base(input, notifyUrl)
        {
        }

        /// <summary>
        /// 交易类型，必填，JSAPI JSAPI支付，NATIVE Native支付，APP APP支付
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string trade_type => "APP";
    }
}
