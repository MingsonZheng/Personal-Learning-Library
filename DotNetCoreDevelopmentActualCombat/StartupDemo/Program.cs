using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace StartupDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(builder =>
                {
                    Console.WriteLine("ConfigureAppConfiguration");
                })
                .ConfigureServices(service =>
                {
                    Console.WriteLine("ConfigureServices");
                })
                .ConfigureHostConfiguration(builder =>
                {
                    Console.WriteLine("ConfigureHostConfiguration");
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    Console.WriteLine("ConfigureWebHostDefaults");
                    webBuilder.UseStartup<Startup>();
                });
    }
}
