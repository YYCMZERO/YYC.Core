using AntJoin.Pay.Models.AliPay;
using AntJoin.Pay.PayConfig;
using Aop.Api;
using Aop.Api.Request;
using AntJoin.Pay.Models;
using System;
using System.Threading.Tasks;

namespace AntJoin.Pay.Services
{
    public class AliTradeService : ITradeService
    {
        private readonly IAopClient _client;
        private readonly PaymentResult _paymentResult = new PaymentResult();

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public AliTradeService() : this(AliPayConfig.ServerUrl, AliPayConfig.AppId, AliPayConfig.MerchantPrivateKey, "json", AliPayConfig.Version,
                AliPayConfig.SignType, AliPayConfig.AlipayPublicKey, AliPayConfig.Charset)
        {
        }

        /// <summary>
        /// 构造函数
        /// <param name="appId">应用ID</param>
        /// <param name="privateKey">开发者私钥</param>
        /// <param name="publicKey">开发者公钥</param>
        /// </summary>
        public AliTradeService(string appId, string privateKey, string publicKey) : this(AliPayConfig.ServerUrl, appId, privateKey, "json", AliPayConfig.Version,
                AliPayConfig.SignType, publicKey, AliPayConfig.Charset)
        {
        }

        /// <summary>
        /// 构造函数
        /// <param name="appId">应用ID</param>
        /// <param name="privateKey">开发者私钥</param>
        /// </summary>
        public AliTradeService(PaySetting paySetting)
        {
            if (paySetting.KeyFromFile)
            {
                var path = paySetting.KeyFilePath.TrimEnd('/');
                var certParams = new CertParams
                {
                    AlipayPublicCertPath = $"{path}/{paySetting.AppId}/alipayCertPublicKey_RSA2.crt",
                    AppCertPath = $"{path}/{paySetting.AppId}/appCertPublicKey_{paySetting.AppId}.crt",
                    RootCertPath = $"{path}/{paySetting.AppId}/alipayRootCert.crt"
                };
                _client = new DefaultAopClient(AliPayConfig.ServerUrl, paySetting.AppId, paySetting.PrivateKey,
                "json", "1.0", "RSA2", "utf-8", false, certParams);
            }
            else
            {
                _client = new DefaultAopClient(AliPayConfig.ServerUrl, paySetting.AppId, paySetting.PrivateKey, "json", AliPayConfig.Version,
                    AliPayConfig.SignType, paySetting.PublicKey, AliPayConfig.Charset, paySetting.KeyFromFile);
            }
        }

        /// <summary>
        /// 构造函数
        /// <param name="appId">应用ID</param>
        /// <param name="privateKey">开发者私钥</param>
        /// </summary>
        public AliTradeService(string appId, string privateKey)
        {
            _client = new DefaultAopClient(AliPayConfig.ServerUrl, appId, privateKey, true);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serverUrl">支付宝网关</param>
        /// <param name="appId">应用ID</param>
        /// <param name="privateKey">开发者私钥</param>
        /// <param name="format">格式</param>
        /// <param name="version">版本</param>
        /// <param name="signType">签名类型</param>
        /// <param name="publicKey">开发者公钥</param>
        /// <param name="charset">编码</param>
        /// <returns></returns>
        public AliTradeService(string serverUrl, string appId, string privateKey, string format, string version,
     string signType, string publicKey, string charset)
        {
            _client = new DefaultAopClient(serverUrl, appId, privateKey, format, version,
           signType, publicKey, charset);
        }
        #endregion

        #region 接口方法实现
        /// <summary>
        /// 统一收单线下交易查询
        /// </summary>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        public PayResult TradeQuery(string outTradeNo)
        {
            var result = new PayResult();
            var payResult = new ResultQuery();
            try
            {
                TradeBuilderQuery build = new TradeBuilderQuery(outTradeNo);
                AlipayTradeQueryRequest payRequest = new AlipayTradeQueryRequest { BizContent = build.BuildJson() };
                payResult.Response = _client.Execute(payRequest);
            }
            catch (Exception e)
            {
                payResult.Response.SubMsg = e.Message;
            }
            var status = payResult.Status;
            switch (status)
            {
                case ResultEnum.Success:
                    result.SetResult(status, "订单支付成功");
                    break;
                default:
                    SetResult(result, status, payResult.Response);
                    break;
            }
            return result;
        }


        /// <summary>
        /// 统一收单线下交易查询
        /// </summary>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        public async Task<PayResult> TradeQueryAsync(string outTradeNo)
        {
            return await Task.Run(() => TradeQuery(outTradeNo));
        }


        /// <summary>
        /// 统一收单线下交易预创建（扫码支付）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="notifyUrl">通知URL</param>
        /// <returns></returns>
        public PayResult TradePrecreate(PayInput input, string notifyUrl = null)
        {
            var result = new PayResult();
            ResultPrecreate payResult = new ResultPrecreate();
            try
            {
                var builder = new TradeBuilderPrecreate(input);
                AlipayTradePrecreateRequest payRequest = new AlipayTradePrecreateRequest
                {
                    BizContent = builder.BuildJson()
                };
                if (notifyUrl != null)
                    payRequest.SetNotifyUrl(notifyUrl);
                payResult.Response = _client.Execute(payRequest);
            }
            catch (Exception e)
            {
                payResult.Response.SubMsg = e.Message;
            }
            var status = payResult.Status;
            switch (status)
            {
                case ResultEnum.Success:
                    _paymentResult.TradeNo = input.TradeNo;
                    _paymentResult.Form = payResult.Response.QrCode;
                    result.Data = _paymentResult;
                    result.SetResult(status, "预创建成功！");
                    break;
                default:
                    SetResult(result, status, payResult.Response);
                    break;
            }
            return result;
        }


        /// <summary>
        /// 统一收单线下交易预创建（扫码支付）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="notifyUrl">通知URL</param>
        /// <returns></returns>
        public async Task<PayResult> TradePrecreateAsync(PayInput input, string notifyUrl = null)
        {
            return await Task.Run(() => TradePrecreate(input, notifyUrl));
        }


        /// <summary>
        /// 统一收单交易退款接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public PayResult TradeRefund(RefundInput input)
        {
            var result = new PayResult();
            ResultRefund payResult = new ResultRefund();
            try
            {
                var builder = new TradeBuilderRefund(input);
                AlipayTradeRefundRequest refundRequest = new AlipayTradeRefundRequest
                {
                    BizContent = builder.BuildJson()
                };
                refundRequest.SetNotifyUrl(input.NotifyUrl);
                payResult.Response = _client.Execute(refundRequest);
            }
            catch (Exception e)
            {
                payResult.Response.SubMsg = e.Message;
            }
            var status = payResult.Status;
            switch (status)
            {
                case ResultEnum.Success:
                    result.SetResult(status, "订单退款成功！");
                    break;
                default:
                    SetResult(result, status, payResult.Response);
                    break;
            }
            return result;
        }


        /// <summary>
        /// 统一收单交易退款接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PayResult> TradeRefundAsync(RefundInput input)
        {
            return await Task.Run(() => TradeRefund(input));
        }


        /// <summary>
        /// 统一收单交易关闭接口
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public PayResult TradeClose(string outTradeNo)
        {
            var result = new PayResult();
            ResultClose payResult = new ResultClose();
            try
            {
                TradeBuilderClose builder = new TradeBuilderClose(outTradeNo);
                AlipayTradeCloseRequest refundRequest = new AlipayTradeCloseRequest
                {
                    BizContent = builder.BuildJson()
                };
                payResult.Response = _client.Execute(refundRequest);
            }
            catch (Exception e)
            {
                payResult.Response.SubMsg = e.Message;
            }
            var status = payResult.Status;
            switch (status)
            {
                case ResultEnum.Success:
                    result.SetResult(status, "订单关闭成功！");
                    break;
                default:
                    SetResult(result, status, payResult.Response);
                    break;
            }
            return result;
        }

        /// <summary>
        /// 统一收单交易关闭接口
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public async Task<PayResult> TradeCloseAsync(string outTradeNo)
        {
            return await Task.Run(() => TradeClose(outTradeNo));
        }

        /// <summary>
        /// 统一收单交易支付接口（扫付款码支付）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="notifyUrl">通知URL</param>
        /// <returns></returns>
        public PayResult TradeMicropay(PayInput input, string notifyUrl = null)
        {
            var result = new PayResult();
            ResultMicropay payResult = new ResultMicropay();
            try
            {
                var builder = new TradeBuilderMicropay(input);
                AlipayTradePayRequest payRequest = new AlipayTradePayRequest
                {
                    BizContent = builder.BuildJson()
                };
                if (notifyUrl != null)
                    payRequest.SetNotifyUrl(notifyUrl);
                payResult.Response = _client.Execute(payRequest);
            }
            catch
            {
                payResult.Response = null;
            }
            var status = payResult.Status;
            switch (status)
            {
                case ResultEnum.Success:
                    result.SetResult(status, "支付成功！");
                    break;
                default:
                    SetResult(result, status, payResult.Response);
                    break;
            }
            return result;
        }


        /// <summary>
        /// 统一收单交易支付接口（扫付款码支付）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="notifyUrl">通知URL</param>
        /// <returns></returns>
        public async Task<PayResult> TradeMicropayAsync(PayInput input, string notifyUrl = null)
        {
            return await Task.Run(() => TradeMicropay(input, notifyUrl));
        }

        /// <summary>
        /// 统一收单下单并支付页面接口（电脑端支付）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="notifyUrl">通知URL</param>
        /// <returns></returns>
        public PayResult TradePage(PayInput input, string notifyUrl = null)
        {
            var result = new PayResult();
            var payResult = new ResultPage();
            try
            {
                var builder = new TradeBuilderPcPage(input);
                var payRequest = new AlipayTradePagePayRequest
                {
                    BizContent = builder.BuildJson()
                };
                if (notifyUrl != null)
                    payRequest.SetNotifyUrl(notifyUrl);
                payRequest.SetReturnUrl(input.ReturnUrl);
                payResult.Response = _client.pageExecute(payRequest);
            }
            catch
            {
                payResult.Response = null;
            }
            var status = payResult.Status;
            if (string.IsNullOrEmpty(payResult.Response.Body))
            {
                SetResult(result, status, payResult.Response);
            }
            else
            {
                _paymentResult.TradeNo = input.TradeNo;
                _paymentResult.Form = payResult.Response.Body;
                result.Data = _paymentResult;
                result.SetResult(1, "订单预创建成功！");
            }
            return result;
        }


        /// <summary>
        /// 统一收单下单并支付页面接口（电脑端支付）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="notifyUrl">通知URL</param>
        /// <returns></returns>
        public async Task<PayResult> TradePageAsync(PayInput input, string notifyUrl = null)
        {
            return await Task.Run(() => TradePage(input, notifyUrl));
        }


        /// <summary>
        /// 统一收单下单APP支付（APP支付）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="notifyUrl"></param>
        /// <returns></returns>
        public PayResult TradeApp(PayInput input, string notifyUrl = null)
        {
            var result = new PayResult();
            var payResult = new ResultApp();
            try
            {
                var builder = new TradeBuilderApp(input);
                var payRequest = new AlipayTradeAppPayRequest
                {
                    BizContent = builder.BuildJson()
                };
                if (notifyUrl != null)
                    payRequest.SetNotifyUrl(notifyUrl);
                payRequest.SetReturnUrl(input.ReturnUrl);
                payResult.Response = _client.SdkExecute(payRequest);
            }
            catch
            {
                payResult.Response = null;
            }
            var status = payResult.Status;
            if (string.IsNullOrEmpty(payResult.Response.Body))
            {
                SetResult(result, status, payResult.Response);
            }
            else
            {
                _paymentResult.TradeNo = input.TradeNo;
                _paymentResult.Form = payResult.Response.Body;
                result.Data = _paymentResult;
                result.SetResult(1, "订单预创建成功！");
            }
            return result;
        }


        /// <summary>
        /// 统一收单下单APP支付（APP支付）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="notifyUrl"></param>
        /// <returns></returns>
        public async Task<PayResult> TradeAppAsync(PayInput input, string notifyUrl = null)
        {
            return await Task.Run(() => TradeApp(input, notifyUrl));
        }


        /// <summary>
        /// 统一收单下单小程序支付（小程序支付）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="notifyUrl"></param>
        /// <returns></returns>
        public PayResult TradeSmallApp(PayInput input, string notifyUrl = null)
        {
            throw new NotImplementedException("支付宝无小程序支付！");
        }


        /// <summary>
        /// 统一收单下单小程序支付（小程序支付）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="notifyUrl"></param>
        /// <returns></returns>
        public Task<PayResult> TradeSmallAppAsync(PayInput input, string notifyUrl = null)
        {
            throw new NotImplementedException("支付宝无小程序支付！");
        }

        /// <summary>
        /// 设置错误信息
        /// </summary>
        /// <param name="result"></param>
        /// <param name="status"></param>
        /// <param name="response"></param>
        private void SetResult(PayResult result, ResultEnum status, AopResponse response)
        {
            switch (status)
            {
                case ResultEnum.Failed:
                    result.SetResult(status, response?.SubMsg ?? "用户授权码校验错误");
                    break;
                case ResultEnum.Unknown:
                    result.SetResult(status, response?.SubMsg ?? "配置或网络异常，请检查后重试");
                    break;
                case ResultEnum.Closed:
                    result.SetResult(status, response?.SubMsg ?? "订单已关闭");
                    break;
            }
        }
        #endregion
    }
}
