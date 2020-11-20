
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LoggingSimpleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var config = configBuilder.Build();

            IServiceCollection serviceCollection = new ServiceCollection();// 构造容器

            // 用工厂模式将配置对象注册到容器管理
            // 注入的时候使用了一个委托，意味着容器可以帮我们管理这个对象的生命周期
            serviceCollection.AddSingleton<IConfiguration>(p => config);
            // 如果将实例直接注入，容器不会帮我们管理
            //serviceCollection.AddSingleton<IConfiguration>(config);

            // AddLogging 往容器里面注册了几个关键对象：
            // ILoggerFactory，泛型模板 typeof (ILogger<>)，Logger 的过滤配置 IConfigureOptions<LoggerFilterOptions>
            // 最后一行，configure((ILoggingBuilder) new LoggingBuilder(services)); 就是整个注入我们的委托
            serviceCollection.AddLogging(builder =>
            {
                builder.AddConfiguration(config.GetSection("Logging"));// 注册 Logging 配置的 Section
                builder.AddConsole();// 先使用一个 Console 的日志输出提供程序
            });

            //// 从容器里面获取 ILoggerFactory
            //IServiceProvider service = serviceCollection.BuildServiceProvider();
            //ILoggerFactory loggerFactory = service.GetService<ILoggerFactory>();

            //ILogger alogger = loggerFactory.CreateLogger("alogger");

            //alogger.LogDebug(2001, "aiya");
            //alogger.LogInformation("hello");

            //var ex = new Exception("出错了");
            //alogger.LogError(ex, "出错了");

            serviceCollection.AddTransient<OrderService>();
            IServiceProvider service = serviceCollection.BuildServiceProvider();
            var order = service.GetService<OrderService>();
            order.Show();
        }
    }
}
