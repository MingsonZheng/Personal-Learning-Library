using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExceptionDemo.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ExceptionDemo.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        [Route("/error")]
        public IActionResult Index()
        {
            // 获取当前上下文里面报出的异常信息
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            var ex = exceptionHandlerPathFeature?.Error;

            // 特殊处理，尝试转换为 IKnownException
            var knownException = ex as IKnownException;
            // 对于未知异常，我们并不应该把错误异常完整地输出给客户端，而是应该定义一个特殊的信息 Unknown 传递给用户
            // Unknown 其实也是一个 IKnownException 的实现，它的 Message = "未知错误", ErrorCode = 9999
            // 也就是说我们在控制器 throw new Exception("报个错"); 就会看到错误信息
            if (knownException == null)
            {
                var logger = HttpContext.RequestServices.GetService<ILogger<MyExceptionFilterAttribute>>();
                // 我们看到的信息是未知错误，但是在我们的日志系统里面，我们还是记录的原有的异常信息
                logger.LogError(ex, ex.Message);
                knownException = KnownException.Unknown;
            }
            else// 当识别到异常是已知的业务异常时，输出已知的异常，包括异常消息，错误状态码和错误信息，就是在 IKnownException 中的定义
            {
                knownException = KnownException.FromKnownException(knownException);
            }
            return View(knownException);
        }
    }
}
