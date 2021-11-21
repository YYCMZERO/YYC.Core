using System.ComponentModel;

namespace AntJoin.Pay.Models
{
    public enum PayEnum
    {
        /// <summary>
        /// 支付宝支付
        /// </summary>
        [Description("支付宝支付")]
        AliPay = 1,

        /// <summary>
        /// 微信支付
        /// </summary>
        [Description("微信支付")]
        WeChatPay = 2
    }
}
