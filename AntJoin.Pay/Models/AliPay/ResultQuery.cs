using Aop.Api.Response;

namespace AntJoin.Pay.Models.AliPay
{
    internal class ResultQuery : BaseResult
    {
        public AlipayTradeQueryResponse Response { get; set; }

        public override ResultEnum Status
        {
            get
            {
                if (Response != null)
                {
                    if (Response.Code == ResultCode.Success &&
                        (Response.TradeStatus.Equals(TradeStatus.TradeSuccess) || Response.TradeStatus.Equals(TradeStatus.TradeFinished)))
                    {
                        return ResultEnum.Success;
                    }
                    if (Response.Code == ResultCode.Error)
                    {
                        return ResultEnum.Failed;
                    }

                    if (Response.Code == ResultCode.Success && Response.TradeStatus.Equals(TradeStatus.TradeClosed))
                    {
                        return ResultEnum.Closed;
                    }
                    return ResultEnum.Unknown;
                }

                return ResultEnum.Unknown;
            }
        }
    }
}
