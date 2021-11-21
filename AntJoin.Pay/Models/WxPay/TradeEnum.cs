using System.ComponentModel;

namespace AntJoin.Pay.Models.WxPay
{
    internal enum TradeEnum
    {
        /// <summary>
        /// 支付成功 
        /// </summary>
        [Description("订单支付成功")]
        SUCCESS = 1,

        /// <summary>
        /// 订单转入退款 
        /// </summary>
        [Description("订单转入退款")]
        REFUND,

        /// <summary>
        /// 订单未付款
        /// </summary>
        [Description("订单未付款")]
        NOTPAY,

        /// <summary>
        /// 已关闭 
        /// </summary>
        [Description("订单已关闭")]
        CLOSED,

        /// <summary>
        /// 订单已撤销（付款码支付） 
        /// </summary>
        [Description("订单已撤销（付款码支付）")]
        REVOKED,

        /// <summary>
        /// 用户支付中，需要输入密码 
        /// </summary>
        [Description("用户支付中，需要输入密码")]
        USERPAYING,

        /// <summary>
        /// 订单支付失败(其他原因，如银行返回失败)
        /// </summary>
        [Description("订单支付失败(其他原因，如银行返回失败)")]
        PAYERROR,

        /// <summary>
        /// 订单已支付，不能进行二次操作
        /// </summary>
        [Description("订单已支付，不能进行二次操作")]
        ORDERPAID,

        /// <summary>
        /// 系统超时，接口返回错误
        /// </summary>
        [Description("系统超时，接口返回错误")]
        SYSTEMERROR,

        /// <summary>
        /// 订单已关闭，无法重复关闭
        /// </summary>
        [Description("订单已关闭，无法重复关闭")]
        ORDERCLOSED,

        /// <summary>
        /// 参数签名结果不正确
        /// </summary>
        [Description("参数签名结果不正确")]
        SIGNERROR,

        /// <summary>
        /// 未使用post传递参数
        /// </summary>
        [Description("未使用post传递参数")]
        REQUIRE_POST_METHOD,

        /// <summary>
        /// XML格式错误
        /// </summary>
        [Description("XML格式错误")]
        XML_FORMAT_ERROR,

        /// <summary>
        /// 请求参数错误
        /// </summary>
        [Description("请求参数错误")]
        PARAM_ERROR,

        /// <summary>
        /// 二维码已过期，请用户在微信上刷新后再试
        /// </summary>
        [Description("二维码已过期，请用户在微信上刷新后再试")]
        AUTHCODEEXPIRE,

        /// <summary>
        /// 用户余额不足
        /// </summary>
        [Description("用户余额不足")]
        NOTENOUGH,

        /// <summary>
        /// 不支持卡类型
        /// </summary>
        [Description("不支持卡类型")]
        NOTSUPORTCARD,

        /// <summary>
        /// 用户授权码参数错误
        /// </summary>
        [Description("用户授权码参数错误")]
        AUTH_CODE_ERROR,

        /// <summary>
        /// 用户授权码检验错误
        /// </summary>
        [Description("用户授权码检验错误")]
        AUTH_CODE_INVALID,

        /// <summary>
        /// 商家订单号已存在
        /// </summary>
        [Description("商家订单号已存在")]
        INVALID_REQUEST
    }
}
