using System;
using Microsoft.Extensions.Logging;

namespace LoggingSimpleDemo
{
    public class OrderService
    {
        ILogger<OrderService> _logger;

        public OrderService(ILogger<OrderService> logger)
        {
            _logger = logger;
        }

        public void Show()
        {
            // 在我们决定要输出的时候，也就是在 LogInformation 内部 console 要输出的时候才做拼接的动作
            _logger.LogInformation("Show Time{time}", DateTime.Now);
            // 我们在字符串拼接好以后，输入给了 LogInformation
            _logger.LogInformation($"Show Time{DateTime.Now}");
        }
    }
}
