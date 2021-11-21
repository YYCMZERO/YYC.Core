using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using AntJoin.Pay.PayConfig;

namespace AntJoin.Pay.Models.WxPay
{
    internal class BaseTradeBuilder
    {
        private string _wxPayKey;
        public BaseTradeBuilder()
        {
            nonce_str = GetNonceString();
            appid = WxPayConfig.Appid;
            mch_id = WxPayConfig.Mchid;
            _wxPayKey = WxPayConfig.Key;
        }

        /// <summary>
        /// 公众账号ID，必填
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string appid { set; get; }

        /// <summary>
        /// 商户号，必填
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string mch_id { set; get; }

        /// <summary>
        /// 随机字符串，必填，随机字符串，长度要求在32位以内。
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string nonce_str { get; set; }

        /// <summary>
        /// 签名，必填，通过签名算法计算得出的签名值
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string sign { get; set; }

        /// <summary>
        /// 签名类型，可选，签名类型，默认为MD5，支持HMAC-SHA256和MD5。
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string sign_type => "MD5";

        public void SetConfigVaues(PaySetting paySetting)
        {
            if (paySetting != null && !string.IsNullOrEmpty(paySetting.AppId))
            {
                appid = paySetting.AppId;
                mch_id = paySetting.PublicKey;
                _wxPayKey = paySetting.PrivateKey;
            }
        }

        /// <summary>
        /// 获取随机串
        /// </summary>
        /// <returns></returns>
        protected string GetNonceString()
        {
            var str = Guid.NewGuid().ToString();
            return str.Replace("-", "");
        }

        /// <summary>
        /// 转换成Xml字符串
        /// </summary>
        /// <returns></returns>
        public virtual string ToXmlString()
        {
            string xml = "<xml>";
            var type = GetType();
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(this);
                if (value == null)
                    continue;
                var name = property.Name;
                if (property.PropertyType.Name == "Int32")
                {
                    xml += "<" + name + ">" + value + "</" + name + ">";
                }
                else
                {
                    xml += "<" + name + ">" + "<![CDATA[" + value + "]]></" + name + ">";
                }
            }
            xml += "</xml>";
            return xml;
        }

        /// <summary>
        /// Dictionary格式转化成url参数格式
        /// </summary>
        /// <returns>url格式串, 该串不包含sign字段值</returns>
        private string ToUrl()
        {
            string buff = "";
            var type = GetType();
            var properties = type.GetProperties();
            foreach (var property in properties.OrderBy(l => l.Name))
            {
                var value = property.GetValue(this);
                if (value == null)
                    continue;
                var name = property.Name;
                if (name == "sign")
                    continue;
                buff += name + "=" + value + "&";
            }
            buff = buff.Trim('&');
            return buff;
        }

        /// <summary>
        /// 签名
        /// </summary>
        /// <returns></returns>
        public void SetSign()
        {
            //转url格式
            string str = ToUrl();
            //在string后加入API KEY
            str += "&key=" + _wxPayKey;
            //所有字符转为大写
            sign = GetMd5(str).ToUpper();
        }
        private string GetMd5(string input)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }


        /// <summary>
        /// 获取支付的参数
        /// </summary>
        /// <param name="prePayId"></param>
        /// <param name="tradeNo"></param>
        /// <returns></returns>
        public PaymentResult GetPayment(string prePayId, string tradeNo, string type)
        {
            var payment = new PaymentResult(appid, mch_id, tradeNo)
            {
                Noncestr = GetNonceString(),
                SignType = sign_type,
                PrepayId = prePayId
            };
            string input = null;
            switch (type)
            {
                case "app":
                    input = $"appid={appid}&noncestr={payment.Noncestr}&package=Sign=WXPay&partnerid={mch_id}&prepayid={prePayId}&timestamp={payment.TimeStamp}&key={_wxPayKey}";
                    break;
                case "smallApp":
                    input = $"appId={appid}&nonceStr={payment.Noncestr}&package=prepay_id={prePayId}&signType={sign_type}&timeStamp={payment.TimeStamp}&key={_wxPayKey}";
                    break;
            }
            if (input != null)
                payment.PaySign = GetMd5(input);
            return payment;
        }


        /// <summary>
        /// 验证
        /// </summary>
        /// <returns></returns>
        public virtual bool Validate()
        {
            return true;
        }
    }
}
