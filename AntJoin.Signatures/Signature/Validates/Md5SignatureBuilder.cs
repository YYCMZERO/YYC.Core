using AntJoin.Core.Security;
using AntJoin.Log;
using System.Collections.Generic;
using System.Linq;

namespace AntJoin.Signatures
{
    /// <summary>
    /// 签名验证处理
    /// </summary>
    public class Md5SignatureBuilder : ISignatureBuilder
    {
        public Md5SignatureBuilder()
        {
        }

        public string Build(IDictionary<string, string> parameters, string secret, string signatureText)
        {
            var sortedDict = new SortedDictionary<string, object>();
            foreach (var parameter in parameters)
            {
                sortedDict.Add(parameter.Key, parameter.Value);
            }
            var text = string.Join("&", sortedDict.Select(s => $"{s.Key}={s.Value}"));
            text = $"{text}{secret}".ToLower();
            LogHelper.Log(LogLevel.INFO, text);
            return Md5.Hash(text).ToLower();
        }
    }
}
