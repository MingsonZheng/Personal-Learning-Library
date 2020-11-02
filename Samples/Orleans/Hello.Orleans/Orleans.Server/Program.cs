using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.AdoNet.SqlServer.Clustering;
using Orleans.AdoNet.SqlServer.Persistence;
using Orleans.AdoNet.SqlServer.Reminder;
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
                    var connectionString =
                        @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Hello.Orleans;Integrated Security=True;Pooling=False;Max Pool Size=200;MultipleActiveResultSets=True";

                    //use AdoNet for clustering 
                    builder.UseSqlServerClustering(options =>
                    {
                        options.ConnectionString = connectionString;
                    }).Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "Hello.Orleans";
                        options.ServiceId = "Hello.Orleans";
                    }).ConfigureEndpoints(new Random().Next(10001, 20000), new Random().Next(20001, 30000));

                    //use AdoNet for reminder service
                    builder.UseSqlServerReminderService(options =>
                    {
                        options.ConnectionString = connectionString;
                    });

                    //use AdoNet for Persistence
                    builder.AddSqlServerGrainStorageAsDefault(options =>
                    {
                        options.ConnectionString = connectionString;
                        options.UseJsonFormat = true;
                    });

                    builder.ConfigureApplicationParts(parts =>
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
