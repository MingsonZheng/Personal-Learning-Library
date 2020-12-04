using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using HelloApi.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HelloApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            Console.WriteLine("Server started");

            //// 依赖查找
            //var helloService1 = host.Services.GetRequiredService<IHelloService>();
            //helloService1.Hello();

            //var helloService2 = host.Services.GetRequiredService<IHelloService>();
            //helloService2.Hello();

            //using (var scope = host.Services.CreateScope())
            //{
            //    // 依赖查找
            //    var helloService1 = scope.ServiceProvider.GetRequiredService<IHelloService>();
            //    helloService1.Hello();

            //    var helloService2 = scope.ServiceProvider.GetRequiredService<IHelloService>();
            //    helloService2.Hello();
            //}

            var providers = host.Services.GetServices<ILoggerProvider>();// 获取容器中所有注入的实例
            foreach (var provider in providers)
            {
                Console.WriteLine(provider.GetType().ToString());
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                //.ConfigureServices((ctx, services) =>
                //{
                //    // 依赖注入
                //    // 任何时候获取到的都是一个新的实例
                //    //services.AddTransient<IHelloService, HelloService>();
                //    // 单例，整个应用程序的生命周期只有一个实例
                //    //services.AddSingleton<IHelloService, HelloService>();
                //    // 每个 scope 都有一个实例
                //    services.AddScoped<IHelloService, HelloService>();
                //})
                //.ConfigureLogging((ctx, logger) =>
                //{
                //    //logger.AddProvider();// 添加
                //    logger.ClearProviders();// 清除
                //})
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}