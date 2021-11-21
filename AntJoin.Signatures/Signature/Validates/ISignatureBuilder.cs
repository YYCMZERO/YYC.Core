using System.Collections.Generic;

namespace AntJoin.Signatures
{
    public interface ISignatureBuilder
    {
        /// <summary>
        /// 生成签名
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="secret"></param>
        /// <param name="signatureText"></param>
        /// <returns></returns>
        string Build(IDictionary<string, string> parameters, string secret, string signatureText);
    }
}
