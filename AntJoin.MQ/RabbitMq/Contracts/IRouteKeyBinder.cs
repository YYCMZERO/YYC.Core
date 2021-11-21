namespace AntJoin.MQ.Contracts
{
    public interface IRouteKeyBinder
    {
        /// <summary>
        /// 绑定路由键
        /// </summary>
        /// <param name="routeKey"></param>
        /// <returns></returns>
        IRabbitMqConsumer BindKey(string routeKey);

        /// <summary>
        /// 解绑路由键
        /// </summary>
        /// <param name="routeKey"></param>
        /// <returns></returns>
        IRabbitMqConsumer UnBindKey(string routeKey);
    }
}