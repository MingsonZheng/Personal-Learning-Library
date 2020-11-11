using DependencyInjectionDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace DependencyInjectionDemo.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IOrderService _orderService;

        // 在构造函数中添加两个入参，IOrderService 和 IGenericService
        // 通过断点调试查看 genericService 的类型可得知，泛型的具体实现可以用容器里面的任意类型来替代
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IOrderService orderService, IGenericService<IOrderService> genericService)
        {
            _orderService = orderService;
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

        // FromServices 标注的作用是从容器里面获取我们的对象
        // 每个对象获取两遍，用于对比每个生命周期获取的对象是什么样子的
        // HashCode 代表对象的唯一性
        [HttpGet]
        public string GetService(
            [FromServices] IMySingletonService singleton1,
            [FromServices] IMySingletonService singleton2,
            [FromServices] IMyTransientService transient1,
            [FromServices] IMyTransientService transient2,
            [FromServices] IMyScopedService scoped1,
            [FromServices] IMyScopedService scoped2)
        {
            Console.WriteLine($"singleton1:{singleton1.GetHashCode()}");
            Console.WriteLine($"singleton2:{singleton2.GetHashCode()}");
            Console.WriteLine($"transient1:{transient1.GetHashCode()}");
            Console.WriteLine($"transient2:{transient2.GetHashCode()}");
            Console.WriteLine($"scoped1:{scoped1.GetHashCode()}");
            Console.WriteLine($"scoped2:{scoped2.GetHashCode()}");
            Console.WriteLine($"========请求结束========");
            return "GetService";
        }

        // IEnumerable<IOrderService>：获取曾经注册过的所有 IOrderService
        public string GetServiceList([FromServices] IEnumerable<IOrderService> services)
        {
            foreach (var item in services)
            {
                Console.WriteLine($"获取到服务实例：{item}:{item.GetHashCode()}");
            }
            return "GetServiceList";
        }
    }
}
