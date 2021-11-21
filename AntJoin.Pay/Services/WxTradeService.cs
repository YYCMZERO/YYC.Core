using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AntJoin.Pay.Models;
using AntJoin.Pay.Models.WxPay;
using AntJoin.Pay.PayConfig;

namespace AntJoin.Pay.Services
{
    public class WxTradeService : ITradeService
    {
        private readonly PaySetting _paySetting;

        public WxTradeService()
        {
        }

        public WxTradeService(PaySetting paySetting)
        {
            _paySetting = paySetting;
        }

        #region 接口方法实现
        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        public PayResult TradeQuery(string outTradeNo)
        {
            var result = new PayResult();
            var builder = new TradeBuilderQuery(outTradeNo);
            builder.SetConfigVaues(_paySetting);
            builder.SetSign();
            string url = "https://api.mch.weixin.qq.com/pay/orderquery";
            string xml = builder.ToXmlString();
            string response = GetUrlContentByPostXml(xml, url, false);//调用HTTP通信接口提交数据
            var resultQuery = new ResultQuery(response);
            if (resultQuery.result_code == "SUCCESS" && resultQuery.return_code == "SUCCESS")
            {
                switch (resultQuery.trade_state)
                {
                    case "SUCCESS":
                        result.SetResult(1, "订单支付成功");
                        break;
                    default:
                        result.SetResult(5, resultQuery.trade_state_desc ?? resultQuery.return_msg);
                        break;
                }
            }
            else
            {
                resultQuery.SetResult(result);
            }
            return result;
        }

        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        public async Task<PayResult> TradeQueryAsync(string outTradeNo)
        {
            var result = new PayResult();
            var builder = new TradeBuilderQuery(outTradeNo);
            builder.SetConfigVaues(_paySetting);
            builder.SetSign();
            string url = "https://api.mch.weixin.qq.com/pay/orderquery";
            string xml = builder.ToXmlString();
            string response = await GetUrlContentByPostXmlAsync(xml, url, false);//调用HTTP通信接口提交数据
            var resultQuery = new ResultQuery(response);
            if (resultQuery.result_code == "SUCCESS" && resultQuery.return_code == "SUCCESS")
            {
                switch (resultQuery.trade_state)
                {
                    case "SUCCESS":
                        result.SetResult(1, "订单支付成功");
                        break;
                    default:
                        result.SetResult(5, resultQuery.trade_state_desc ?? resultQuery.return_msg);
                        break;
                }
            }
            else
            {
                resultQuery.SetResult(result);
            }
            return result;
        }

        /// <summary>
        /// 统一收单线下交易预创建（生成二维码支付）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="notifyUrl">通知URL</param>
        /// <returns></returns>
        public PayResult TradePrecreate(PayInput input, string notifyUrl = null)
        {
            var result = new PayResult();
            var builder = new TradeBuilderPrecreate(input, notifyUrl);
            builder.SetConfigVaues(_paySetting);
            builder.SetSign();
            string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
            string xml = builder.ToXmlString();
            string response = GetUrlContentByPostXml(xml, url, false);
            var resultPay = new ResultPrecreate(response);
            if (resultPay.result_code == "SUCCESS" && !string.IsNullOrEmpty(resultPay.code_url))
            {
                result.Data = new PaymentResult(null, null, input.TradeNo) { Form = resultPay.code_url };
                result.SetResult(1, "订单预创建成功！");
            }
            else
            {
                resultPay.SetResult(result);
            }
            return result;
        }

        /// <summary>
        /// 统一收单线下交易预创建（生成二维码支付）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="notifyUrl">通知URL</param>
        /// <returns></returns>
        public async Task<PayResult> TradePrecreateAsync(PayInput input, string notifyUrl = null)
        {
            var result = new PayResult();
            var builder = new TradeBuilderPrecreate(input, notifyUrl);
            builder.SetConfigVaues(_paySetting);
            builder.SetSign();
            string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
            string xml = builder.ToXmlString();
            string response = await GetUrlContentByPostXmlAsync(xml, url, false);
            var resultPay = new ResultPrecreate(response);
            if (resultPay.result_code == "SUCCESS" && !string.IsNullOrEmpty(resultPay.code_url))
            {
                result.Data = new PaymentResult(null, null, input.TradeNo) { Form = resultPay.code_url };
                result.SetResult(1, "订单预创建成功！");
            }
            else
            {
                resultPay.SetResult(result);
            }
            return result;
        }


        /// <summary>
        /// 退款
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public PayResult TradeRefund(RefundInput input)
        {
            var result = new PayResult();
            var builder = new TradeBuilderRefund(input);
            builder.SetConfigVaues(_paySetting);
            builder.SetSign();
            string url = "https://api.mch.weixin.qq.com/secapi/pay/refund";
            string xml = builder.ToXmlString();
            string response = GetUrlContentByPostXml(xml, url, true);//调用HTTP通信接口提交数据
            var resultRefund = new ResultRefund(response);
            if (resultRefund.result_code == "SUCCESS")
            {
                result.SetResult(1, "订单退款成功");
            }
            else
            {
                resultRefund.SetResult(result);
            }
            return result;
        }


        /// <summary>
        /// 退款
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PayResult> TradeRefundAsync(RefundInput input)
        {
            var result = new PayResult();
            var builder = new TradeBuilderRefund(input);
            builder.SetConfigVaues(_paySetting);
            builder.SetSign();
            string url = "https://api.mch.weixin.qq.com/secapi/pay/refund";
            string xml = builder.ToXmlString();
            string response = await GetUrlContentByPostXmlAsync(xml, url, true);//调用HTTP通信接口提交数据
            var resultRefund = new ResultRefund(response);
            if (resultRefund.result_code == "SUCCESS")
            {
                result.SetResult(1, "订单退款成功");
            }
            else
            {
                resultRefund.SetResult(result);
            }
            return result;
        }

        /// <summary>
        /// 关闭订单
        /// </summary>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        public PayResult TradeClose(string outTradeNo)
        {
            var result = new PayResult();
            var builder = new TradeBuilderClose(outTradeNo);
            builder.SetConfigVaues(_paySetting);
            builder.SetSign();
            string url = "https://api.mch.weixin.qq.com/pay/closeorder";
            string xml = builder.ToXmlString();
            string response = GetUrlContentByPostXml(xml, url, false);//调用HTTP通信接口提交数据

            var resultClose = new ResultClose(response);
            if (resultClose.result_code == "SUCCESS" && resultClose.return_code == "SUCCESS")
            {
                result.SetResult(1, "订单关闭成功");
            }
            else
            {
                resultClose.SetResult(result);
            }
            return result;
        }

        /// <summary>
        /// 关闭订单
        /// </summary>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        public async Task<PayResult> TradeCloseAsync(string outTradeNo)
        {
            var result = new PayResult();
            var builder = new TradeBuilderClose(outTradeNo);
            builder.SetConfigVaues(_paySetting);
            builder.SetSign();
            string url = "https://api.mch.weixin.qq.com/pay/closeorder";
            string xml = builder.ToXmlString();
            string response = await GetUrlContentByPostXmlAsync(xml, url, false);//调用HTTP通信接口提交数据

            var resultClose = new ResultClose(response);
            if (resultClose.result_code == "SUCCESS" && resultClose.return_code == "SUCCESS")
            {
                result.SetResult(1, "订单关闭成功");
            }
            else
            {
                resultClose.SetResult(result);
            }
            return result;
        }

        /// <summary>
        /// 提交付款码支付
        /// </summary>
        /// <param name="input"></param>
        /// <param name="notifyUrl">通知URL</param>
        /// <returns></returns>
        public PayResult TradeMicropay(PayInput input, string notifyUrl = null)
        {
            var result = new PayResult();
            var builder = new TradeBuilderMicropay(input, notifyUrl);
            builder.SetConfigVaues(_paySetting);
            builder.SetSign();
            string url = "https://api.mch.weixin.qq.com/pay/micropay";
            string xml = builder.ToXmlString();
            string response = GetUrlContentByPostXml(xml, url, false);//调用HTTP通信接口提交数据

            var resultPay = new ResultMicropay(response);
            if (resultPay.result_code == "SUCCESS" && resultPay.return_code == "SUCCESS")
            {
                result.SetResult(1, "订单支付成功");
            }
            else
            {
                resultPay.SetResult(result);
            }
            return result;
        }

        /// <summary>
        /// 提交付款码支付
        /// </summary>
        /// <param name="input"></param>
        /// <param name="notifyUrl">通知URL</param>
        /// <returns></returns>
        public async Task<PayResult> TradeMicropayAsync(PayInput input, string notifyUrl = null)
        {
            var result = new PayResult();
            var builder = new TradeBuilderMicropay(input, notifyUrl);
            builder.SetConfigVaues(_paySetting);
            builder.SetSign();
            string url = "https://api.mch.weixin.qq.com/pay/micropay";
            string xml = builder.ToXmlString();
            string response = await GetUrlContentByPostXmlAsync(xml, url, false);//调用HTTP通信接口提交数据

            var resultPay = new ResultMicropay(response);
            if (resultPay.result_code == "SUCCESS" && resultPay.return_code == "SUCCESS")
            {
                result.SetResult(1, "订单支付成功");
            }
            else
            {
                resultPay.SetResult(result);
            }
            return result;
        }

        /// <summary>
        /// 统一收单下单手机H5支付（手机H5支付）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="notifyUrl">通知URL</param>
        /// <returns></returns>
        public PayResult TradePage(PayInput input, string notifyUrl = null)
        {
            var result = new PayResult();
            var builder = new TradeBuilderPage(input, notifyUrl);
            builder.SetConfigVaues(_paySetting);
            builder.SetSign();
            string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
            string xml = builder.ToXmlString();
            string response = GetUrlContentByPostXml(xml, url, false);//调用HTTP通信接口提交数据

            var resultPay = new ResultPage(response);
            if (resultPay.result_code == "SUCCESS" && resultPay.return_code == "SUCCESS")
            {
                result.Data = new PaymentResult(null, null, input.TradeNo) { Form = resultPay.mweb_url + "&redirect_url=" + input.ReturnUrl };
                result.SetResult(1, "订单预创建成功");
            }
            else
            {
                resultPay.SetResult(result);
            }
            return result;
        }

        /// <summary>
        /// 统一收单下单手机H5支付（手机H5支付）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="notifyUrl">通知URL</param>
        /// <returns></returns>
        public async Task<PayResult> TradePageAsync(PayInput input, string notifyUrl = null)
        {
            var result = new PayResult();
            var builder = new TradeBuilderPage(input, notifyUrl);
            builder.SetConfigVaues(_paySetting);
            builder.SetSign();
            string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
            string xml = builder.ToXmlString();
            string response = await GetUrlContentByPostXmlAsync(xml, url, false);//调用HTTP通信接口提交数据

            var resultPay = new ResultPage(response);
            if (resultPay.result_code == "SUCCESS" && resultPay.return_code == "SUCCESS")
            {
                result.Data = new PaymentResult(null, null, input.TradeNo) { Form = resultPay.mweb_url + "&redirect_url=" + input.ReturnUrl };
                result.SetResult(1, "订单预创建成功");
            }
            else
            {
                resultPay.SetResult(result);
            }
            return result;
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
            var builder = new TradeBuilderApp(input, notifyUrl);
            builder.SetConfigVaues(_paySetting);
            builder.SetSign();
            string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
            string xml = builder.ToXmlString();
            string response = GetUrlContentByPostXml(xml, url, false);//调用HTTP通信接口提交数据

            var resultPay = new ResultPage(response);
            if (resultPay.result_code == "SUCCESS" && resultPay.return_code == "SUCCESS")
            {
                result.Data = builder.GetPayment(resultPay.prepay_id, builder.out_trade_no, "app");
                result.SetResult(1, "订单预创建成功");
            }
            else
            {
                resultPay.SetResult(result);
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
            var result = new PayResult();
            var builder = new TradeBuilderApp(input, notifyUrl);
            builder.SetConfigVaues(_paySetting);
            builder.SetSign();
            string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
            string xml = builder.ToXmlString();
            string response = await GetUrlContentByPostXmlAsync(xml, url, false);//调用HTTP通信接口提交数据

            var resultPay = new ResultPage(response);
            if (resultPay.result_code == "SUCCESS" && resultPay.return_code == "SUCCESS")
            {
                result.Data = builder.GetPayment(resultPay.prepay_id, builder.out_trade_no, "app");
                result.SetResult(1, "订单预创建成功");
            }
            else
            {
                resultPay.SetResult(result);
            }
            return result;
        }

        /// <summary>
        /// 统一收单下单小程序支付（小程序支付）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="notifyUrl"></param>
        /// <returns></returns>
        public PayResult TradeSmallApp(PayInput input, string notifyUrl = null)
        {
            var result = new PayResult();
            var builder = new TradeBuilderSmallApp(input, notifyUrl);
            builder.SetConfigVaues(_paySetting);
            builder.SetSign();
            string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
            string xml = builder.ToXmlString();
            string response = GetUrlContentByPostXml(xml, url, false);//调用HTTP通信接口提交数据

            var resultPay = new ResultSmallApp(response);
            if (resultPay.result_code == "SUCCESS" && resultPay.return_code == "SUCCESS")
            {
                result.Data = builder.GetPayment(resultPay.prepay_id, builder.out_trade_no, "smallApp");
                result.SetResult(1, "订单预创建成功");
            }
            else
            {
                resultPay.SetResult(result);
            }
            return result;
        }

        /// <summary>
        /// 统一收单下单小程序支付（小程序支付）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="notifyUrl"></param>
        /// <returns></returns>
        public async Task<PayResult> TradeSmallAppAsync(PayInput input, string notifyUrl = null)
        {
            var result = new PayResult();
            var builder = new TradeBuilderSmallApp(input, notifyUrl);
            builder.SetConfigVaues(_paySetting);
            builder.SetSign();
            string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
            string xml = builder.ToXmlString();
            string response = await GetUrlContentByPostXmlAsync(xml, url, false);//调用HTTP通信接口提交数据

            var resultPay = new ResultSmallApp(response);
            if (resultPay.result_code == "SUCCESS" && resultPay.return_code == "SUCCESS")
            {
                result.Data = builder.GetPayment(resultPay.prepay_id, builder.out_trade_no, "smallApp");
                result.SetResult(1, "订单预创建成功");
            }
            else
            {
                resultPay.SetResult(result);
            }
            return result;
        }
        #endregion

        #region http连接基础类，负责底层的http通信
        /// <summary>
        /// http连接基础类，负责底层的http通信
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="url"></param>
        /// <param name="isUseCert">是否使用证书</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        private string GetUrlContentByPostXml(string xml, string url, bool isUseCert, int timeout = 6)
        {
            //设置最大连接数
            ServicePointManager.DefaultConnectionLimit = 200;


            /***************************************************************
            * 下面设置HttpWebRequest的相关属性
            * ************************************************************/
            var request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "POST";
            request.Timeout = timeout * 1000;


            //设置POST的数据类型和长度
            request.ContentType = "text/xml";
            byte[] data = Encoding.UTF8.GetBytes(xml);
            request.ContentLength = data.Length;

            //是否使用证书
            if (isUseCert)
            {
                X509Certificate2 cert = GetCertObject();
                request.ClientCertificates.Add(cert);
            }
            //往服务器写入数据
            using (var reqStream = request.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }


            //获取服务端返回
            using WebResponse response = request.GetResponse();
            //获取返回值
            using Stream myResponseStream = response.GetResponseStream();
            if (myResponseStream != null)
            {
                using StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                return myStreamReader.ReadToEnd();
            }
            return null;
        }



        /// <summary>
        /// http连接基础类，负责底层的http通信
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="url"></param>
        /// <param name="isUseCert">是否使用证书</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        private async Task<string> GetUrlContentByPostXmlAsync(string xml, string url, bool isUseCert, int timeout = 6)
        {
            //设置最大连接数
            ServicePointManager.DefaultConnectionLimit = 200;
            /***************************************************************
            * 下面设置HttpWebRequest的相关属性
            * ************************************************************/
            var request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "POST";
            request.Timeout = timeout * 1000;


            //设置POST的数据类型和长度
            request.ContentType = "text/xml";
            byte[] data = Encoding.UTF8.GetBytes(xml);
            request.ContentLength = data.Length;

            //是否使用证书
            if (isUseCert)
            {
                X509Certificate2 cert = GetCertObject();
                request.ClientCertificates.Add(cert);
            }
            //往服务器写入数据
            using (var reqStream = request.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }


            //获取服务端返回
            using WebResponse response = request.GetResponse();
            //获取返回值
            using Stream myResponseStream = response.GetResponseStream();
            if (myResponseStream != null)
            {
                using StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                return await myStreamReader.ReadToEndAsync();
            }
            return null;
        }

        /// <summary>
        /// 获取证书
        /// </summary>
        /// <returns></returns>
        private X509Certificate2 GetCertObject()
        {
            string path = _paySetting.KeyFilePath ?? WxPayConfig.PhysicalPath;
            X509Certificate2 pathcert = new X509Certificate2(path + "/" + _paySetting.PublicKey + ".p12", _paySetting.PublicKey);
            X509Certificate2 cert = null;
            try
            {
                X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                var certs = store.Certificates;
                for (int i = 0; i < certs.Count; i++)
                {
                    if (certs[i].SerialNumber == pathcert.SerialNumber)
                    {
                        cert = certs[i];
                    }
                }
            }
            catch (System.Exception)
            {
                return pathcert;
            }
            return cert ?? pathcert;
        }
        #endregion
    }
}
