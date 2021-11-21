using Newtonsoft.Json;
using System;

namespace AntJoin.Pay.Models.AliPay
{
    /// <summary>
    /// Class1 的摘要说明
    /// </summary>
    internal abstract class BaseTradeBuilder
    {

        // 验证bizContent对象
        public abstract bool Validate();

        // 将bizContent对象转换为json字符串
        public string BuildJson()
        {
            try
            {
                return JsonConvert.SerializeObject(this);
            }
            catch (Exception ex)
            {

                throw new Exception("JsonConvert.SerializeObject(): " + ex.Message);
            }
        }
    }
}
