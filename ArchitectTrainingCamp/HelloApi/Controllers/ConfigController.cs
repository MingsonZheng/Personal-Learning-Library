using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;

namespace HelloApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly MyOption _myOption;

        //public ConfigController(IConfiguration configuration, MyOption myOption)
        //{
        //    _configuration = configuration;
        //    _myOption = myOption;
        //}

        //public ConfigController(IConfiguration configuration, IOptions<MyOption> myOption)
        //{
        //    _configuration = configuration;
        //    _myOption = myOption.Value;
        //}

        public ConfigController(IConfiguration configuration, IOptionsSnapshot<MyOption> myOption)
        {
            _configuration = configuration;
            _myOption = myOption.Value;

            _myOption = myOption.Get(MyOption.PETER);
            _myOption = myOption.Get(MyOption.JACK);
        }

        //public ConfigController(IConfiguration configuration, IOptionsMonitor<MyOption> myOption)
        //{
        //    _configuration = configuration;
        //    _myOption = myOption.CurrentValue;

        //    // 配置变化处理
        //    myOption.OnChange(option =>
        //    {

        //    });
        //}

        [HttpGet("option")]
        public IActionResult GetOption()
        {
            return Ok(_myOption);
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
