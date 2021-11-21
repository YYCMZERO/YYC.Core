namespace AntJoin.Pay.Models.WxPay
{
    internal class TradeBuilderPrecreate : BaseTradeBuilderPay
    {
        public TradeBuilderPrecreate(PayInput input) : base(input)
        {
            product_id = input.TradeNo;
        }
        public TradeBuilderPrecreate(PayInput input, string notifyUrl) : base(input, notifyUrl)
        {
        }

        /// <summary>
        /// 交易类型，必填，JSAPI JSAPI支付，NATIVE Native支付，APP APP支付
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string trade_type => "NATIVE";



        /// <summary>
        /// 商品ID，必填，此参数为二维码中包含的商品ID，商户自行定义。
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string product_id { get; set; }
    }
}
