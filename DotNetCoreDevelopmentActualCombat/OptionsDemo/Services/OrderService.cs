using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace OptionsDemo.Services
{
    public interface IOrderService
    {
        int ShowMaxOrderCount();
    }

    public class OrderService : IOrderService
    {
        //OrderServiceOptions _options;
        //IOptions<OrderServiceOptions> _options;
        //IOptionsSnapshot<OrderServiceOptions> _options;
        IOptionsMonitor<OrderServiceOptions> _options;

        //public OrderService(OrderServiceOptions options)
        //public OrderService(IOptions<OrderServiceOptions> options)
        //public OrderService(IOptionsSnapshot<OrderServiceOptions> options)
        public OrderService(IOptionsMonitor<OrderServiceOptions> options)
        {
            _options = options;

            _options.OnChange(option =>
            {
                Console.WriteLine($"配置更新了，最新的值是:{_options.CurrentValue.MaxOrderCount}");
            });
        }

        public int ShowMaxOrderCount()
        {
            //return _options.MaxOrderCount;
            //return _options.Value.MaxOrderCount;
            return _options.CurrentValue.MaxOrderCount;
        }
    }

    // 代表从配置中读取的值
    public class OrderServiceOptions
    {
        [Range(30, 100)]
        public int MaxOrderCount { get; set; } = 100;
    }
}