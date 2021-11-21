using AntJoin.Core.Configuration;
using AntJoin.Core.Utils;
using System;
using System.Xml;

namespace AntJoin.Core.SMS
{
    public class KaiXinTongSMS
    {
        private readonly string _apikey = ConfigurationHelper.GetValue("KxtSmsApiKey");
        private readonly string _appname = ConfigurationHelper.GetValue("KxtSmsAppName");

        /// <summary>
        /// 发送短信通知
        /// </summary>
        /// <param name="phone">电话号码</param>
        /// <param name="content">内容</param>
        /// <param name="sign"></param
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool SendMessage(string phone, string content, string sign, out string msg)
        {
            //https://way.jd.com/kaixintong/kaixintong?mobile=15659049959&content=【凯信通】您的验证码是：123456&appkey=f502e2efdd0ce1638c867d3e155f7940
            //{"code":"10000","charge":true,"msg":"查询成功,扣费","result":"<?xml version=\"1.0\" encoding=\"utf-8\" ?><returnsms>\n <returnstatus>Faild</returnstatus>\n <message>非法签名</message>\n <remainpoint>0</remainpoint>\n <taskID>0</taskID>\n <successCounts>0</successCounts></returnsms>"}
            string param = "mobile=" + phone + "&content=【" + (string.IsNullOrEmpty(sign) ? _appname : sign) + "】" + content + "&appkey=" + _apikey;
            string url = "https://way.jd.com/kaixintong/kaixintong?" + param;
            string result = HttpHelper.HttpGet(url);
            if (result.IsNotNullOrEmpty())
            {
                var pResponse = result.BoJsonObject<ApiResponse>();
                if (pResponse.charge)
                {
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(pResponse.result);
                    XmlNode node = xml.SelectSingleNode("returnsms/returnstatus");
                    if (node?.FirstChild?.Value == "Success")
                    {
                        msg = "获取验证码成功";
                        return true;
                    }
                    else
                    {
                        node = xml.SelectSingleNode("returnsms/message");
                        msg = node.Value;
                        return false;
                    }
                }
                else
                {
                    msg = "获取验证码失败";
                    return false;
                }

            }
            else
            {
                msg = "获取验证码失败";
                return false;
            }
        }

        private class ApiResponse
        {
            public string code { get; set; }
            public bool charge { get; set; }
            public string result { get; set; }
        }
    }
}
