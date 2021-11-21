using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AntJoin.Signatures
{
    /// <summary>
    /// 签名处理
    /// </summary>
    public interface ISignatureHandler
    {
        /// <summary>
        /// 处理签名
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<bool> HandleAsync(HttpContext context);
    }
}
