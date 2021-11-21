namespace AntJoin.Pay.Models.WxPay
{
    internal class TradeBuilderPage : BaseTradeBuilderPay
    {
        public TradeBuilderPage(PayInput input) : base(input)
        {
        }
        public TradeBuilderPage(PayInput input, string notifyUrl) : base(input, notifyUrl)
        {
        }

        /// <summary>
        /// 交易类型，必填，JSAPI JSAPI支付，NATIVE Native支付，APP APP支付，MWEB 手机H5支付
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string trade_type => "MWEB";
    }
}
