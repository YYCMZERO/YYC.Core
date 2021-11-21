using Aop.Api.Response;

namespace AntJoin.Pay.Models.AliPay
{
    internal class ResultMicropay : BaseResult
    {

        public AlipayTradePayResponse Response { get; set; }

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

                    if (Response.SubCode == ResultCode.Close)
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
