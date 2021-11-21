using System;

namespace AntJoin.Pay.Models.AliPay
{
    internal class TradeBuilderRefund : BaseTradeBuilder
    {
        public TradeBuilderRefund(RefundInput input)
        {
            out_trade_no = input.TradeNo;
            out_request_no = input.RefundNo;
            refund_amount = input.RefundAmount.ToString("#0.00");
            refund_reason = input.RefundReason;
        }
        /// <summary>
        /// 特殊可选，支付宝交易号，和商户订单号不能同时为空
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string trade_no { get; set; }


        /// <summary>
        /// 特殊可选，订单支付时传入的商户订单号,不能和 trade_no同时为空。
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string out_trade_no { get; set; }


        /// <summary>
        /// 必选，需要退款的金额，该金额不能大于订单金额,单位为元，支持两位小数
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string refund_amount { get; set; }


        /// <summary>
        /// 可选，标识一次退款请求，同一笔交易多次退款需要保证唯一，如需部分退款，则此参数必传。
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string out_request_no { get; set; }


        /// <summary>
        /// 可选，退款的原因说明
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string refund_reason { get; set; }


        public override bool Validate()
        {
            throw new NotImplementedException();
        }


    }
}
