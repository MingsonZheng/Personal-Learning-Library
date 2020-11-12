using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DependencyInjectionScopeAndDisposableDemo.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DependencyInjectionScopeAndDisposableDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        //[HttpGet]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    var rng = new Random();
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateTime.Now.AddDays(index),
        //        TemperatureC = rng.Next(-20, 55),
        //        Summary = Summaries[rng.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}

        //[HttpGet]
        //public int Get([FromServices] IOrderService orderService,
        //    [FromServices] IOrderService orderService2,
        //    [FromServices] IHostApplicationLifetime hostApplicationLifetime,
        //    [FromQuery] bool stop = false)
        //{
        //    Console.WriteLine("=======1==========");
        //    // HttpContext.RequestServices 是当前请求的一个根容器，应用程序根容器的一个子容器，每个请求会创建一个容器
        //    using (IServiceScope scope = HttpContext.RequestServices.CreateScope())
        //    {
        //        // 在这个子容器下面再创建一个子容器来获取服务
        //        var service = scope.ServiceProvider.GetService<IOrderService>();
        //        var service2 = scope.ServiceProvider.GetService<IOrderService>();
        //    }
        //    Console.WriteLine("=======2==========");

        //    if (stop)
        //    {
        //        hostApplicationLifetime.StopApplication();
        //    }

        //    Console.WriteLine("接口请求处理结束");

        //    return 1;
        //}

        [HttpGet]
        public int Get(
            [FromServices] IHostApplicationLifetime hostApplicationLifetime,
            [FromQuery] bool stop = false)
        {

            if (stop)
            {
                hostApplicationLifetime.StopApplication();
            }

            return 1;
        }
    }
}
