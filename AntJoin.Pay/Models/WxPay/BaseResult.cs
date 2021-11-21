using System;
using System.ComponentModel;
using System.Reflection;
using System.Xml;

namespace AntJoin.Pay.Models.WxPay
{
    internal class BaseResult
    {
        public BaseResult(string xml)
        {
            Type type = GetType();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNode xmlNode = xmlDoc.FirstChild;//获取到根节点<xml>
            XmlNodeList nodes = xmlNode.ChildNodes;
            foreach (XmlNode xn in nodes)
            {
                XmlElement xe = (XmlElement)xn;
                var property = type.GetProperty(xe.Name);
                if (property == null)
                    continue;
                property.SetValue(this, xe.InnerText);
            }
        }

        /// <summary>
        /// 返回状态码 SUCCESS/FAIL
        /// 此字段是通信标识，非交易标识，交易是否成功需要查看trade_state来判断
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string return_code { get; set; }

        /// <summary>
        /// 商家订单号当return_code为FAIL时返回信息为错误原因 ，例如 签名失败，参数格式校验错误
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string return_msg { get; set; }

        /// <summary>
        /// 业务结果，SUCCESS/FAIL
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string result_code { get; set; }

        /// <summary>
        /// 错误代码 当result_code为FAIL时返回错误代码，详细参见下文错误列表
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string err_code { get; set; }

        /// <summary>
        /// 错误代码描述，当result_code为FAIL时返回错误描述，详细参见下文错误列表
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string err_code_des { get; set; }

        /// <summary>
        /// 微信生成的预支付回话标识，用于后续接口调用中使用，该值有效期为2小时
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string prepay_id { get; set; }


        /// <summary>
        /// 获取枚举描述
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public void SetResult(PayResult result)
        {
            var state = GetTradeEnum(err_code);
            result.SetResult(5, GetDescript(state) ?? err_code_des ?? return_msg);
        }


        /// <summary>
        /// 获取枚举描述
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private TradeEnum? GetTradeEnum(string code)
        {
            try
            {
                return (TradeEnum)Enum.Parse(typeof(TradeEnum), code);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取枚举描述
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private string GetDescript(Enum val)
        {
            if (val == null)
                return null;
            FieldInfo field = val.GetType().GetField(val.ToString());
            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute?.Description ?? val.ToString();
        }
    }
}
