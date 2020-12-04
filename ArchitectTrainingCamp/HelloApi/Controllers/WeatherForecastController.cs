using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HelloApi.Controllers
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

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();

            _logger.LogInformation("Get action executed");
            _logger.LogInformation(new EventId(1001, "Action"), "Get action executed");

            return result;
        }

        public IActionResult CreateOrder(dynamic order)
        {
            _logger.LogTrace("Enter CreateOrder method");

            _logger.LogDebug("Start creating order: {0}", "order info");

            _logger.LogTrace("Start executing _orderService.Create method");

            if (order.amount <= 0)
            {
                _logger.LogWarning("Order Amount is:{0}");
            }

            //_orderService.Create(order);

            _logger.LogTrace("Completed executing _orderService.Crete method");

            _logger.LogTrace("Leave CreateOrder Successfully");

            _logger.LogInformation("Leave CreateOrder Successfully");

            return Ok();
        }
    }
}
