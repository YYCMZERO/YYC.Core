namespace AntJoin.Pay.Models.AliPay
{
    internal class ResultCode
    {
        public const string Success = "10000";
        public const string Inrrocess = "10003";
        //业务处理失败:	具体失败原因参见接口返回的错误码    
        public const string Fail = "40004";
        //业务出现未知错误或者系统异常: 如果支付接口返回，需要调用查询接口确认订单状态或者发起撤销
        public const string Error = "20000";

        //订单关闭时的SubCode值，需要更改订单编号
        public const string Close = "ACQ.TRADE_HAS_CLOSE";
    }
}
