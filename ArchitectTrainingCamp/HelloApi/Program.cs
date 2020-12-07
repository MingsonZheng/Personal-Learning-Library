using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using HelloApi.Services;
using Microsoft.Extensions.Configuration;
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

            var applicationLifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

            applicationLifetime.ApplicationStarted.Register((() =>
            {
                Console.WriteLine("Application Started");
            }));

            applicationLifetime.ApplicationStopping.Register((() =>
            {
                Console.WriteLine("Application Stopping");
            }));

            applicationLifetime.ApplicationStopped.Register((() =>
            {
                Console.WriteLine("Application Stopped");
            }));

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(configure =>
                {

                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.Sources.Clear();

                    // 内容根目录
                    var root = hostingContext.HostingEnvironment.ContentRootPath;

                    // 环境
                    var envName = hostingContext.HostingEnvironment.EnvironmentName;

                    var env = hostingContext.HostingEnvironment;

                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                    config.AddEnvironmentVariables();

                    config.AddCommandLine(source =>
                    {
                        source.Args = args;
                    });
                })
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