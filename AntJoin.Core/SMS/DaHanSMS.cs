using AntJoin.Core.Configuration;
using AntJoin.Core.Security;
using AntJoin.Core.Utils;
using System;

namespace AntJoin.Core.SMS
{
    /// <summary>
    /// 大汉三通
    /// </summary>
    public class DaHanSMS : IShortMessageService
    {
        private readonly string _apiKey = AJSecurity.Decrypt(ConfigurationHelper.GetDefaultValue("DaHanSmsApiKey", "F438DEFD7DF17226"));
        private readonly string _appSecret = AJSecurity.Decrypt(ConfigurationHelper.GetDefaultValue("DaHanSmsAppSecret", "87567EA0B5B939C27A421B86E8A3666A1B16766CA3AEA0BEA13189C474DCBA3AB00EAB7E37AFF497"));
        public bool SendMessage(string phone, string content, string sign, out string msg)
        {
            try
            {
                SendSmsData sendSmsData = new SendSmsData(_apiKey, _appSecret, content, phone, "【" + sign + "】");
                string data = sendSmsData.BoJsonString();
                string strResponse = HttpHelper.HttpPostJson("http://www.dh3t.com/json/sms/Submit", data);
                SendSmsResult result = strResponse.BoJsonObject<SendSmsResult>();

                msg = result.Desc;
                return result.Result == 0;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
        }


        /// <summary>
        /// 结果json
        /// </summary>
        private class SendSmsResult
        {
            public string Msgid { get; set; }

            public int Result { get; set; }

            public string Desc { get; set; }
            public string FailPhones { get; set; }

        }


        /// <summary>
        /// 发生数据的对象
        /// </summary>
        private class SendSmsData
        {
            public SendSmsData(string account, string password, string content, string phones, string sign)
            {
                Account = account;
                Password = password;
                Phones = phones;
                Content = content;
                Sign = sign;
            }
            public string Account { get; set; }

            public string Password { get; set; }

            public string Content { get; set; }

            public string Phones { get; set; }

            public string Sign { get; set; }

            public string Subcode { get; set; }

            public string Msgid { get; set; }
        }
    }
}
