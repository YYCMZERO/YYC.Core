using Aop.Api.Response;

namespace AntJoin.Pay.Models.AliPay
{
    internal class ResultRefund : BaseResult
    {

        public AlipayTradeRefundResponse Response { get; set; }

        public override ResultEnum Status
        {
            get
            {
                if (Response != null)
                {
                    if (Response.Code == ResultCode.Success)
                    {
                        return ResultEnum.Success;
                    }
                    if (Response.Code == ResultCode.Error)
                    {
                        return ResultEnum.Unknown;
                    }

                    return ResultEnum.Failed;
                }

                return ResultEnum.Unknown;
            }
        }

    }
}
