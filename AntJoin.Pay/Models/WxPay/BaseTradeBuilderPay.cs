using AntJoin.Pay.PayConfig;
using System;

namespace AntJoin.Pay.Models.WxPay
{
    internal class BaseTradeBuilderPay : BaseTradeBuilder
    {
        public BaseTradeBuilderPay(PayInput input, string notifyUrl) : this(input)
        {
            if (!string.IsNullOrEmpty(notifyUrl))
                notify_url = notifyUrl;
        }

        public BaseTradeBuilderPay(PayInput input) : base()
        {
            var dtNow = DateTime.Now;
            detail = input.Body ?? input.Subject;
            body = input.Subject;
            out_trade_no = input.TradeNo;
            total_fee = (int)(input.Amount * 100);
            spbill_create_ip = input.ClientIp;
            time_expire = dtNow.AddMinutes(input.TimeExpress).ToString("yyyyMMddHHmmss");
            time_start = dtNow.AddSeconds(-1).ToString("yyyyMMddHHmmss");
        }

        /// <summary>
        /// 商品描述，必填
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string body { get; set; }

        /// <summary>
        /// 商品详情，可选
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string detail { get; set; }

        /// <summary>
        /// 附加数据，可选，在查询API和支付通知中原样返回，可作为自定义参数使用。
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string attach { get; set; }

        /// <summary>
        /// 商户订单号，必填，商户系统内部订单号，要求32个字符内，只能是数字、大小写字母_-|* 且在同一个商户号下唯一。
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string out_trade_no { get; set; }

        ///// <summary>
        ///// 标价币种，可选，符合ISO 4217标准的三位字母代码，默认人民币：CNY
        ///// </summary>
        //// ReSharper disable once InconsistentNaming
        //public string fee_type { get; set; }

        /// <summary>
        /// 标价金额，必填，订单总金额，单位为分
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public int total_fee { get; set; }

        /// <summary>
        /// 终端IP，必填
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string spbill_create_ip { get; set; }

        /// <summary>
        /// 交易起始时间，可选，订单生成时间，格式为yyyyMMddHHmmss
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string time_start { get; set; }

        /// <summary>
        /// 交易结束时间，可选，订单生成时间，格式为yyyyMMddHHmmss
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string time_expire { get; set; }


        /// <summary>
        /// 通知地址，必填，异步接收微信支付结果通知的回调地址，通知url必须为外网可访问的url，不能携带参数。
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string notify_url { get; set; } = WxPayConfig.NotifyUrl;
    }
}
