using System.Threading.Tasks;

namespace AntJoin.Redis
{
    internal interface IRedisClientBuilder
    {
        IRedisClient Build(ConnectionOption option);
    }
}
