namespace AntJoin.Pay.PayConfig
{
    public class WxPayConfig
    {
        /// <summary>
        /// 绑定支付的APPID（必须配置）
        /// </summary>
        public const string Appid = "wxb91b4724c86c572a";

        /// <summary>
        /// 商户号（必须配置）
        /// </summary>
        public const string Mchid = "1518604721";


        /// <summary>
        /// 商户支付密钥，参考开户邮件设置（必须配置）
        /// </summary>
        public const string Key = "UMBCIORf9wEW8tA9qy02jrh6SUMeyhSD";

        /// <summary>
        /// 支付结果通知回调url，用于商户接收支付结果
        /// </summary>
        public const string NotifyUrl = "http://face.gzkuaixiang.com/Callback/WechatNotify";


        /// <summary>
        /// 证书路径设置
        /// 证书路径,注意应该填写绝对路径（仅退款、撤销订单时需要）
        /// </summary>
        public const string SslcertPath = "cert/apiclient_cert.p12";
        public const string SslcertPassword = "1233410002";
        public const string PhysicalPath = "";
    }
}
