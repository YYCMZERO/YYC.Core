namespace AntJoin.Grpc.Client
{
    /// <summary>
    /// Grpc客户端工厂
    /// </summary>
    public interface IGrpcClientFactory
    {
        /// <summary>
        /// 获取一个客户端，通过这个方法去获取需要的GRPC客户端，
        /// 在使用的时候，只需在构造函数中注入 <seealso cref="IGrpcClientFactory"/>
        /// 接口既可以通过它去获取所有GRPC服务
        /// </summary>
        /// <param name="address"></param>
        /// <typeparam name="TGrpcClient"></typeparam>
        /// <returns></returns>
        TGrpcClient Get<TGrpcClient>(string address) where TGrpcClient : class;
    }
}