namespace AntJoin.Pay.Models.WxPay
{
    internal class TradeBuilderMicropay : BaseTradeBuilderPay
    {
        public TradeBuilderMicropay(PayInput input) : base(input)
        {
            auth_code = input.AuthCode;
        }
        public TradeBuilderMicropay(PayInput input, string notifyUrl) : base(input, notifyUrl)
        {
        }

        /// <summary>
        /// 授权码，必填，扫码支付授权码，设备读取用户微信中的条码或者二维码信息
        ///（注：用户付款码条形码规则：18位纯数字，以10、11、12、13、14、15开头）
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string auth_code { get; set; }
    }
}
