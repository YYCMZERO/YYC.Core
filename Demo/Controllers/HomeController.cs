using System;
using System.Text;
using System.Threading.Tasks;
using AntJoin.MQ.Contracts;
using AntJoin.MQ.EventBus;
using Demo.EventHandlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRabbitMqChannelFactory _channelFactory;
        private readonly ILogger<HomeController> _logger;
        private readonly IEventBus _eventBus;

        public HomeController(IRabbitMqChannelFactory channelFactory, ILogger<HomeController> logger, IEventBus eventBus)
        {
            _channelFactory = channelFactory;
            _logger = logger;
            _eventBus = eventBus;
        }

        public IActionResult T1()
        {
            var message = Encoding.UTF8.GetBytes($"Time:[{DateTime.Now}] Message: [发送了消息]"); 
            _channelFactory
                .Create()
                .DefinedRoute("demo").CreateProducer()
                .Send("demo", message)
                .Close();

            return Content("OK");
        }

        public IActionResult T2()
        {
            _channelFactory
                .Create()
                .DefinedRoute("demo")
                .BindQueue($"demo")
                .CreateConsumer()
                .BindKey("demo")
                .OnReceived(async (model, args) =>
                {
                    var message = Encoding.UTF8.GetString(args.Body.ToArray());
                    _logger.LogInformation($"接受消息1：{message}");
                    await Task.CompletedTask;
                })
                .OnReceived(async (model, args) =>
                {
                    var message = Encoding.UTF8.GetString(args.Body.ToArray());
                    _logger.LogInformation($"接受消息2：{message}");
                    await Task.CompletedTask;
                });
            return Content("ok");
        }


        public async Task<IActionResult> T3()
        {
            await _eventBus.Publish(new TestEvent
            {
                Title = $"{DateTime.Now}: 发送了消息"
            });
            return Content("OK");
        }


        public async Task<IActionResult> T4()
        {
            await _eventBus.Publish(new TestEvent2
            {
                IsDelivery = true,
                DevliveryNumber = 10
            });

            return Content("OK");
        }
    }
}
