using System.Reflection;
using AntJoin.Redis;
using Demo.EventHandlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            //AddRedisSample(services);
            //AddRabbitMqSample(services);

            AddEventBusService(services);
        }


        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.AddSubscibuteService<TestEvent, TestEventHandler>();
            app.AddSubscibuteService<TestEvent2, TestEvent2Handler1>();
            app.AddSubscibuteService<TestEvent2, TestEvent2Handler2>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }


        private void AddEventBusService(IServiceCollection services)
        {
            services.AddEventBus(Configuration, "Demo.EventHandlers");
        }
        

        /// <summary>
        /// 添加RabbitMQ服务
        /// </summary>
        /// <param name="services"></param>
        private void AddRabbitMqSample(IServiceCollection services)
        {
            services.AddRabbitMqClient(options =>
            {
                //options.EndPoints.Add(new MqEndPoint("111.229.179.90",5672));
                options.UserName = "chewel";
                options.Password = "chewel";
            });
        }


        /// <summary>
        /// 添加Redis服务
        /// </summary>
        /// <param name="services"></param>
        private void AddRedisSample(IServiceCollection services)
        {
            services.AddRedis(options =>
            {
                options.EndPoints.Add(new ServerEndPoint("39.108.169.158", 6379));
                options.Password = "OhIoC3Vr";
                options.DefaultDb = 2;
                options.KeyPrefix = "RedisTestHc";
                options.WorkerCount = 50;
            });
        }
    }
}
