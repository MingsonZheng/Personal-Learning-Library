using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace RoutingDemo.Controllers
{
    [Route("api/[controller]/[action]")]// RouteAttribute 的方式
    [ApiController]
    public class OrderController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">必须可以转为long</param>
        /// <returns></returns>
        //[HttpGet("{id:MyRouteConstraint}")]// 这里使用了自定义的约束
        [HttpGet("{id:isLong}")]
        //public bool OrderExist(object id)
        public bool OrderExist([FromRoute] string id)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">最大20</param>
        /// <param name="linkGenerator"></param>
        /// <returns></returns>
        [HttpGet("{id:max(20)}")]// 这里使用了 Max 的约束
        //public bool Max(long id)
        public bool Max([FromRoute] long id, [FromServices] LinkGenerator linkGenerator)
        {
            // 这两行就是分别获取完整 Uri 和 path 的代码
            // 它还有不同的重载，可以根据需要传入不同的路由的值
            var path = linkGenerator.GetPathByAction(HttpContext,
                action: "Reque",
                controller: "Order",
                values: new { name = "abc" });// 因为下面对 name 有一个必填的约束，所以这里需要传值

            var uri = linkGenerator.GetUriByAction(HttpContext,
                action: "Reque",
                controller: "Order",
                values: new { name = "abc" });
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ss">必填</param>
        /// <returns></returns>
        [HttpGet("{name:required}")]// 必填约束
        [Obsolete]
        public bool Reque(string name)
        {
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="number">以三个数字开始</param>
        /// <returns></returns>
        [HttpGet("{number:regex(^\\d{{3}}$)}")]// 正则表达式约束
        public bool Number(string number)
        {
            return true;
        }
    }
}