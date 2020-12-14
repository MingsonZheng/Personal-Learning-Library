using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using HelloApi.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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

        [HttpGet]
        [Route("option/{id}")]
        public IActionResult GetOption([FromRoute] int id, [FromQuery] string name,[FromHeader] string termId)
        {
            //return Ok(_myOption);
            return Ok(new {id, name, termId});
        }

        [HttpGet]
        [Route("option")]
        public IActionResult GetOption([FromQuery] Dictionary<int, string> dic)
        {
            var students = new List<Student>();

            foreach (var item in dic)
            {
                students.Add(new Student {Id = item.Key, Name = item.Value});
            }

            return Ok(students);
        }

        [HttpPost]
        [Route("option/from")]
        public IActionResult CreateOption([FromForm] string name, [FromForm] string id)
        {
            return Ok(new {name, id});
        }

        //[HttpPost]
        //[Route("option/body")]
        //public IActionResult CreateOption([FromBody] string name)
        //{
        //    return Ok(name);
        //}

        [HttpPost]
        [Route("option/body")]
        public IActionResult CreateOption([FromBody] Student student)
        {
            //if (!ModelState.IsValid)
            //{
            //    return ValidationProblem();
            //}

            //return BadRequest();

            //return NotFound();

            return Ok(student);
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
