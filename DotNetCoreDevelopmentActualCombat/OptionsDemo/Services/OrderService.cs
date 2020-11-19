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
        IOptions<OrderServiceOptions> _options;

        //public OrderService(OrderServiceOptions options)
        public OrderService(IOptions<OrderServiceOptions> options)
        {
            _options = options;
        }

        public int ShowMaxOrderCount()
        {
            //return _options.MaxOrderCount;
            return _options.Value.MaxOrderCount;
        }
    }

    // 代表从配置中读取的值
    public class OrderServiceOptions
    {
        public int MaxOrderCount { get; set; } = 100;
    }
}