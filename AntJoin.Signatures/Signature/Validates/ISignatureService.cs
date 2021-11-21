using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AntJoin.Signatures
{
    public interface ISignatureService
    {
        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="context"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        Task<bool> Validate(HttpContext context,string secret);
    }
}
