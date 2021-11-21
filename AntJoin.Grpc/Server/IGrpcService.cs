namespace AntJoin.Grpc.Server
{
    /// <summary>
    /// Grpc服务实现类要继承这个接口，方可以在
    /// 调用 <code>endpoints.MapGrpcEndPoint&lt;IGrpcService&gt;()</code>
    /// 自动注入到节点中
    /// </summary>
    public interface IGrpcService
    {
        
    }
}