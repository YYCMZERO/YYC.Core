using Newtonsoft.Json;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AntJoin.Redis
{
    internal class Fingerprint
    {
        internal static string ToMd5Fingerprint(string data)
        {
            var bytes = Encoding.Unicode.GetBytes(data.ToCharArray());
            var source = new MD5CryptoServiceProvider().ComputeHash(bytes);
            return source.Aggregate(new StringBuilder(32), (StringBuilder sb, byte b) => sb.Append(b.ToString("X2"))).ToString();
        }
    }
}
