using System.Threading.Tasks;
using AntJoin.MQ.EventHandlers;
using Microsoft.Extensions.Logging;

namespace Demo.EventHandlers
{
    public class TestEvent : IntegratedEvent
    {
        public string Title { get; set; }
    }


    public class TestEventHandler : IIntegrateEventHandler<TestEvent>
    {
        private readonly ILogger<TestEventHandler> _logger;
        
        public TestEventHandler(ILogger<TestEventHandler> logger)
        {
            _logger = logger;
        }
        
        public async Task Do(TestEvent @event)
        {
            _logger.LogInformation($"接受到测试消息: {@event.Title}");
            await Task.Yield();
        }
    }
}