using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace HelloApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigController : Controller
    {
        private readonly IConfiguration _configuration;

        public ConfigController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetConfigurations()
        {
            var result = new List<string>();

            //foreach (var key in _configuration.AsEnumerable())
            //{
            //    result.Add($"Key: {key.Key}, value: {key.Value}");
            //}

            return Content(string.Format("Default Log Level: {0}", _configuration["Logging:LogLevel:Default"]));
        }
    }
}
