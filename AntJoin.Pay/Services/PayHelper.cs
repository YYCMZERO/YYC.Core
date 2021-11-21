using AntJoin.Pay.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AntJoin.Pay.Services
{
    public static class PayHelper
    {
        private static readonly Dictionary<string, ITradeService> Dictionary;
        private static readonly object Padlocks = new object();

        static PayHelper()
        {
            Dictionary = new Dictionary<string, ITradeService>();
        }

        /// <summary>
        /// 获取支付对象
        /// </summary>
        /// <param name="paySetting"></param>
        /// <returns></returns>
        private static ITradeService GetService(PaySetting paySetting)
        {
            var payName = paySetting.PayEnum + "-" + paySetting.AppId + "-" + paySetting.PublicKey;
            var isSuc = Dictionary.TryGetValue(payName, out var instance);
            if (!isSuc)
            {
                lock (Padlocks)
                {
                    isSuc = Dictionary.TryGetValue(payName, out instance);
                    if (!isSuc)
                    {
                        switch (paySetting.PayEnum)
                        {
                            case PayEnum.AliPay:
                                var appid = paySetting.AppId;
                                instance = string.IsNullOrEmpty(appid) ? new AliTradeService() : new AliTradeService(paySetting);
                                break;
                            case PayEnum.WeChatPay:
                                instance = new WxTradeService(paySetting);
                                break;
                            default:
                                throw new InvalidOperationException($"未知的支付方式");
                        }
                        Dictionary[payName] = instance;
                    }
                }
            }

            return instance;
        }

        /// <summary>
        /// 预支付，创建二维码
        /// </summary>
        /// <param name="input"></param>
        /// <param name="paySetting"></param>
        /// <returns></returns>
        public static PayResult TradePrecreate(PayInput input, PaySetting paySetting)
        {
            return GetService(paySetting).TradePrecreate(input, paySetting.NotifyUrl);
        }

        /// <summary>
        /// 预支付，创建二维码
        /// </summary>
        /// <param name="input"></param>
        /// <param name="paySetting"></param>
        /// <returns></returns>
        public static async Task<PayResult> TradePrecreateAsync(PayInput input, PaySetting paySetting)
        {
            return await GetService(paySetting).TradePrecreateAsync(input, paySetting.NotifyUrl);
        }


        /// <summary>
        /// 查询订单情况
        /// </summary>
        /// <param name="outTradeNo">商户订单号</param>
        /// <param name="paySetting"></param>
        /// <returns></returns>
        public static PayResult TradeQuery(string outTradeNo, PaySetting paySetting)
        {
            return GetService(paySetting).TradeQuery(outTradeNo);
        }

        /// <summary>
        /// 查询订单情况
        /// </summary>
        /// <param name="outTradeNo">商户订单号</param>
        /// <param name="paySetting"></param>
        /// <returns></returns>
        public static async Task<PayResult> TradeQueryAsync(string outTradeNo, PaySetting paySetting)
        {
            return await GetService(paySetting).TradeQueryAsync(outTradeNo);
        }


        /// <summary>
        /// 退款
        /// </summary>
        /// <param name="input"></param>
        /// <param name="paySetting"></param>
        /// <returns></returns>
        public static PayResult TradeRefund(RefundInput input, PaySetting paySetting)
        {
            return GetService(paySetting).TradeRefund(input);
        }


        /// <summary>
        /// 退款
        /// </summary>
        /// <param name="input"></param>
        /// <param name="paySetting"></param>
        /// <returns></returns>
        public static async Task<PayResult> TradeRefundAsync(RefundInput input, PaySetting paySetting)
        {
            return await GetService(paySetting).TradeRefundAsync(input);
        }


        /// <summary>
        /// 关闭订单
        /// </summary>
        /// <param name="outTradeNo">商户订单号</param>
        /// <param name="paySetting"></param>
        /// <returns></returns>
        public static PayResult TradeClose(string outTradeNo, PaySetting paySetting)
        {
            return GetService(paySetting).TradeClose(outTradeNo);
        }


        /// <summary>
        /// 关闭订单
        /// </summary>
        /// <param name="outTradeNo">商户订单号</param>
        /// <param name="paySetting"></param>
        /// <returns></returns>
        public static async Task<PayResult> TradeCloseAsync(string outTradeNo, PaySetting paySetting)
        {
            return await GetService(paySetting).TradeCloseAsync(outTradeNo);
        }


        /// <summary>
        /// 提交付款码支付
        /// </summary>
        /// <param name="input"></param>
        /// <param name="paySetting"></param>
        /// <returns></returns>
        public static PayResult TradeMicropay(PayInput input, PaySetting paySetting)
        {
            return GetService(paySetting).TradeMicropay(input, paySetting.NotifyUrl);
        }


        /// <summary>
        /// 提交付款码支付
        /// </summary>
        /// <param name="input"></param>
        /// <param name="paySetting"></param>
        /// <returns></returns>
        public static async Task<PayResult> TradeMicropayAsync(PayInput input, PaySetting paySetting)
        {
            return await GetService(paySetting).TradeMicropayAsync(input, paySetting.NotifyUrl);
        }


        /// <summary>
        /// 电脑端支付
        /// </summary>
        /// <param name="input"></param>
        /// <param name="paySetting"></param>
        /// <returns></returns>
        public static PayResult TradePage(PayInput input, PaySetting paySetting)
        {
            return GetService(paySetting).TradePage(input, paySetting.NotifyUrl);
        }


        /// <summary>
        /// 电脑端支付
        /// </summary>
        /// <param name="input"></param>
        /// <param name="paySetting"></param>
        /// <returns></returns>
        public static async Task<PayResult> TradePageAsync(PayInput input, PaySetting paySetting)
        {
            return await GetService(paySetting).TradePageAsync(input, paySetting.NotifyUrl);
        }

        /// <summary>
        /// 统一收单下单APP支付（APP支付）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="paySetting"></param>
        /// <returns></returns>
        public static PayResult TradeApp(PayInput input, PaySetting paySetting)
        {
            return GetService(paySetting).TradeApp(input, paySetting.NotifyUrl);
        }

        /// <summary>
        /// 统一收单下单APP支付（APP支付）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="paySetting"></param>
        /// <returns></returns>
        public static async Task<PayResult> TradeAppAsync(PayInput input, PaySetting paySetting)
        {
            return await GetService(paySetting).TradeAppAsync(input, paySetting.NotifyUrl);
        }


        /// <summary>
        /// 统一收单下单小程序支付（小程序支付）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="paySetting"></param>
        /// <returns></returns>
        public static PayResult TradeSmallApp(PayInput input, PaySetting paySetting)
        {
            return GetService(paySetting).TradeSmallApp(input, paySetting.NotifyUrl);
        }


        /// <summary>
        /// 统一收单下单小程序支付（小程序支付）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="paySetting"></param>
        /// <returns></returns>
        public static async Task<PayResult> TradeSmallAppAsync(PayInput input, PaySetting paySetting)
        {
            return await GetService(paySetting).TradeSmallAppAsync(input, paySetting.NotifyUrl);
        }
    }
}
