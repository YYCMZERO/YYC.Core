using System;
using System.Collections.Generic;

namespace AntJoin.Pay.Models.AliPay
{
    internal class TradeBuilderApp : BaseTradeBuilder
    {
        public TradeBuilderApp(PayInput input)
        {
            out_trade_no = input.TradeNo;
            total_amount = input.Amount.ToString("#0.00");
            subject = input.Subject;
            body = input.Body ?? input.Subject;
            timeout_express = input.TimeExpress + "m";
        }


        /// <summary>
        /// 必选，商户订单号,64个字符以内、只能包含字母、数字、下划线；需保证在商户端不重复
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string out_trade_no { get; set; }

        /// <summary>
        /// 可选，卖家支付宝用户ID。 如果该值为空，则默认为商户签约账号对应的支付宝用户ID
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string seller_id { get; set; }

        /// <summary>
        /// 必选，订单总金额，单位为元，精确到小数点后两位，取值范围[0.01,100000000] 如果同时传入了【打折金额】，【不可打折金额】，【订单总金额】三者，则必须满足如下条件：【订单总金额】=【打折金额】+【不可打折金额】
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string total_amount { get; set; }

        /// <summary>
        /// 可选，可打折金额. 参与优惠计算的金额，单位为元，精确到小数点后两位，取值范围[0.01,100000000] 如果该值未传入，但传入了【订单总金额】，【不可打折金额】则该值默认为【订单总金额】-【不可打折金额】
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string discountable_amount { get; set; }

        /// <summary>
        /// 必选，订单标题
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string subject { get; set; }

        /// <summary>
        /// 可选，对交易或商品的描述
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string body { get; set; }


        /// <summary>
        /// 可选，订单包含的商品列表信息.json格式. 其它说明详见：“商品明细说明”
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public List<GoodsInfo> goods_detail { get; set; }

        /// <summary>
        /// 可选，销售产品码，如果签约的是当面付快捷版，则传OFFLINE_PAYMENT; 其它支付宝当面付产品传FACE_TO_FACE_PAYMENT； 不传默认使用FACE_TO_FACE_PAYMENT；
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string product_code { get; set; } = "FACE_TO_FACE_PAYMENT";

        /// <summary>
        /// 可选，商户操作员编号
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string operator_id { get; set; }


        /// <summary>
        /// 可选，商户门店编号
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string store_id { get; set; }


        /// <summary>
        ///	可选，商户机具终端编号
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string terminal_id { get; set; }


        /// <summary>
        /// 可选，业务扩展参数
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public ExtendParams extend_params { get; set; }

        /// <summary>
        /// 可选，该笔订单允许的最晚付款时间，逾期将关闭交易。取值范围：1m～15d。m-分钟，h-小时，d-天，1c-当天（1c-当天的情况下，无论交易何时创建，都在0点关闭）。 该参数数值不接受小数点， 如 1.5h，可转换为 90m。
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string timeout_express { get; set; }


        public override bool Validate()
        {
            throw new NotImplementedException();
        }
    }
}
