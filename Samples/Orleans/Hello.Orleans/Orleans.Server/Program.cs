using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using Orleans.Grains;
using Orleans.Hosting;

namespace Orleans.Server
{
    class Program
    {
        static Task Main(string[] args)
        {
            Console.Title = typeof(Program).Namespace;

            // define the cluster configuration
            return Host.CreateDefaultBuilder()// 创建泛型主机提供宿主环境
                .UseOrleans((builder) =>// 用来配置Oleans
                {
                        builder.UseLocalhostClustering()// 用于在开发环境下指定连接到本地集群
                            .AddMemoryGrainStorageAsDefault()
                            .Configure<ClusterOptions>(options =>// 用于指定连接到那个集群
                            {
                                options.ClusterId = "Hello.Orleans";
                                options.ServiceId = "Hello.Orleans";
                            })
                            .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)// 用于配置silo与silo、silo与client之间的通信端点。开发环境下可仅指定回环地址作为集群间通信的IP地址
                            .ConfigureApplicationParts(parts =>// 用于指定暴露哪些Grain服务
                                parts.AddApplicationPart(typeof(ISessionControlGrain).Assembly).WithReferences());
                    }
                )
                .ConfigureServices(services =>
                {
                    services.Configure<ConsoleLifetimeOptions>(options =>
                    {
                        options.SuppressStatusMessages = true;
                    });
                })
                .ConfigureLogging(builder => { builder.AddConsole(); })
                .RunConsoleAsync();
        }
    }
}
