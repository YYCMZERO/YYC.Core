using AntJoin.MQ.EventHandlers;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Demo.EventHandlers
{
    public class TestEvent2 : IntegratedEvent
    {
        public bool IsDelivery { get; set; }
        public int DevliveryNumber { get; set; }
    }


    public class TestEvent2Handler1 : IIntegrateEventHandler<TestEvent2>
    {
        private readonly ILogger<TestEvent2Handler1> _logger;

        public TestEvent2Handler1(ILogger<TestEvent2Handler1> logger)
        {
            _logger = logger;
        }

        public async Task Do(TestEvent2 @event)
        {
            if(@event.IsDelivery)
            {
                _logger.LogInformation($"从主仓库开始挑拨数量：{@event.DevliveryNumber}");
            }
            await Task.Yield();
        }
    }


    public class TestEvent2Handler2 : IIntegrateEventHandler<TestEvent2>
    {
        private readonly ILogger<TestEvent2Handler2> _logger;
        private readonly TestEventHandler _testEventHandler;

        public TestEvent2Handler2(ILogger<TestEvent2Handler2> logger, TestEventHandler testEventHandler)
        {
            _logger = logger;
            _testEventHandler = testEventHandler;
        }

        public async Task Do(TestEvent2 @event)
        {
            if(@event.IsDelivery)
            {
                _logger.LogInformation($"分仓库开始发货，数量：{@event.DevliveryNumber}");
            }
            await Task.Yield();
            await _testEventHandler.Do(new TestEvent { Title = "事件处理中嵌套其他事件处理" });
        }
    }
}
