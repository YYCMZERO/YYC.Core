using System.Threading.Tasks;

namespace AntJoin.Signatures
{
    public interface ITokenClearService
    {
        /// <summary>
        /// 清除Token
        /// </summary>
        /// <param name="clearToken"></param>
        /// <returns></returns>
        Task<bool> ClearToken(string token);
    }
}
