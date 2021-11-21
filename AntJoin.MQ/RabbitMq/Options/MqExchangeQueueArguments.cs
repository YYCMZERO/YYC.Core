namespace AntJoin.MQ.Options
{
    /// <summary>
    /// 消息交换机或者队列的附加属性参数
    /// </summary>
    public class MqExchangeQueueArguments
    {
        /// <summary>
        /// 消息的过期时间，单位：毫秒
        /// </summary>
        public const string XMessageTtl = "x-message-ttl";

        /// <summary>
        /// 队列过期时间，队列在多长时间未被访问将被删除，单位：毫秒
        /// </summary>
        public const string XExpires = "x-expires";

        /// <summary>
        /// 队列最大长度，超过该最大值，则将从队列头部开始删除消息
        /// </summary>
        public const string XMaxLength = "x-max-length";
        
        /// <summary>
        /// 队列消息内容占用最大空间，受限于内存大小，超过该阈值则从队列头部开始删除消息
        /// </summary>
        public const string XMaxLengthBytes = "x-max-length-bytes";

        /// <summary>
        /// 设置队列溢出行为。这决定了当达到队列的最大长度时消息会发生什么。
        /// 有效值是drop-head、reject-publish或reject-publish-dlx。
        /// 仲裁队列类型仅支持drop-head；
        /// </summary>
        public const string XOverflow = "x-overflow";

        /// <summary>
        /// 死信交换器名称，过期或被删除（因队列长度超长或因空间超出阈值）的消息可指定发送到该交换器中
        /// </summary>
        public const string XDeadLetterExchange = "x-dead-letter-exchange";

        /// <summary>
        /// 死信消息路由键，在消息发送到死信交换器时会使用该路由键，如果不设置，则使用消息的原来的路由键值
        /// </summary>
        public const string XDeadLetterRoutingKey = "x-dead-letter-routing-key";

        /// <summary>
        /// 表示队列是否是单一活动消费者，true时，注册的消费组内只有一个消费者消费消息，
        /// 其他被忽略，false时消息循环分发给所有消费者(默认false)
        /// </summary>
        public const string XSingleActiveConsumer = "x-single-active-consumer";

        /// <summary>
        /// 队列要支持的最大优先级数;如果未设置，队列将不支持消息优先级
        /// </summary>
        public const string XMaxPriority = "x-max-priority";

        /// <summary>
        /// 将队列设置为延迟模式，在磁盘上保留尽可能多的消息，以减少RAM的使用;如果未设置，队列将保留内存缓存以尽可能快地传递消息
        /// </summary>
        public const string XQueueMode = "x-queue-mode";

        /// <summary>
        /// 在集群模式下设置镜像队列的主节点信息
        /// </summary>
        public const string XQueueMasterLocator = "x-queue-master-locator";

    }
}