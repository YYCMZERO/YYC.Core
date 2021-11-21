namespace AntJoin.Pay.Models.WxPay
{
    internal class TradeBuilderSmallApp : BaseTradeBuilderPay
    {
        public TradeBuilderSmallApp(PayInput input) : base(input)
        {
            openid = input.OpenId;
        }
        public TradeBuilderSmallApp(PayInput input, string notifyUrl) : base(input, notifyUrl)
        {
            openid = input.OpenId;
        }

        /// <summary>
        /// 交易类型，必填，JSAPI JSAPI支付，NATIVE Native支付，APP APP支付
        /// </summary>
        public string trade_type => "JSAPI";


        /// <summary>
        /// 用户标识，必填，用户在商户appid下的唯一标识。
        /// </summary>
        public string openid { get; set; }
    }
}
