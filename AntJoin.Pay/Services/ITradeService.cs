using AntJoin.Pay.Models;
using System.Threading.Tasks;

namespace AntJoin.Pay.Services
{
    public interface ITradeService
    {
        /// <summary>
        /// 统一收单线下交易查询
        /// </summary>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        PayResult TradeQuery(string outTradeNo);

        /// <summary>
        /// 统一收单线下交易查询
        /// </summary>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        Task<PayResult> TradeQueryAsync(string outTradeNo);

        /// <summary>
        /// 统一收单线下交易预创建，即生成二维码供使用
        /// </summary>
        /// <param name="input"></param>
        /// <param name="notifyUrl"></param>
        /// <returns></returns>
        PayResult TradePrecreate(PayInput input, string notifyUrl = null);

        /// <summary>
        /// 统一收单线下交易预创建，即生成二维码供使用
        /// </summary>
        /// <param name="input"></param>
        /// <param name="notifyUrl"></param>
        /// <returns></returns>
        Task<PayResult> TradePrecreateAsync(PayInput input, string notifyUrl = null);

        /// <summary>
        /// 统一收单交易退款接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        PayResult TradeRefund(RefundInput input);

        /// <summary>
        /// 统一收单交易退款接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PayResult> TradeRefundAsync(RefundInput input);

        /// <summary>
        /// 统一收单交易关闭接口
        /// </summary>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        PayResult TradeClose(string outTradeNo);

        /// <summary>
        /// 统一收单交易关闭接口
        /// </summary>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        Task<PayResult> TradeCloseAsync(string outTradeNo);

        /// <summary>
        /// 统一收单交易支付接口（商家扫付款码支付）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="notifyUrl"></param>
        /// <returns></returns>
        PayResult TradeMicropay(PayInput input, string notifyUrl = null);

        /// <summary>
        /// 统一收单交易支付接口（商家扫付款码支付）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="notifyUrl"></param>
        /// <returns></returns>
        Task<PayResult> TradeMicropayAsync(PayInput input, string notifyUrl = null);


        /// <summary>
        /// 统一收单下单并支付页面接口（电脑端支付\H5支付）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="notifyUrl"></param>
        /// <returns></returns>
        PayResult TradePage(PayInput input, string notifyUrl = null);


        /// <summary>
        /// 统一收单下单并支付页面接口（电脑端支付\H5支付）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="notifyUrl"></param>
        /// <returns></returns>
        Task<PayResult> TradePageAsync(PayInput input, string notifyUrl = null);


        /// <summary>
        /// 统一收单下单APP支付（APP支付）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="notifyUrl"></param>
        /// <returns></returns>
        PayResult TradeApp(PayInput input, string notifyUrl = null);


        /// <summary>
        /// 统一收单下单APP支付（APP支付）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="notifyUrl"></param>
        /// <returns></returns>
        Task<PayResult> TradeAppAsync(PayInput input, string notifyUrl = null);


        /// <summary>
        /// 统一收单下单小程序支付（小程序支付）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="notifyUrl"></param>
        /// <returns></returns>
        PayResult TradeSmallApp(PayInput input, string notifyUrl = null);


        /// <summary>
        /// 统一收单下单小程序支付（小程序支付）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="notifyUrl"></param>
        /// <returns></returns>
        Task<PayResult> TradeSmallAppAsync(PayInput input, string notifyUrl = null);
    }
}
