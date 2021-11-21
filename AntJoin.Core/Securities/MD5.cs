using System.Text;

namespace AntJoin.Core.Security
{
    /// <summary>
    /// Md5 哈希
    /// </summary>
    public class Md5
    {
        private Md5()
        {

        }

        /// <summary>
        /// MD5 加密
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Hash(string value)
        {
            byte[] bytes;
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(value));
            }
            var result = new StringBuilder();
            foreach (byte t in bytes)
            {
                result.Append(t.ToString("X2"));
            }
            return result.ToString();
        }
    }
}
