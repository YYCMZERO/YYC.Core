using AntJoin.MQ.Options;

namespace AntJoin.MQ.EventBus
{
    public class RabbitMqEventBusOption
    {
        public MqConnectionOption Connection { get; set; }
        public MqExchangeOption Exchange { get; set; }
        public MqQueueOption Queue { get; set; }
    }
}