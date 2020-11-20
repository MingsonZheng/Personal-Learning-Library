using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OptionsDemo.Services;

namespace OptionsDemo
{
    public static class OrderServiceExtensions
    {
        public static IServiceCollection AddOrderService(this IServiceCollection services, IConfiguration configuration)
        {

            services.Configure<OrderServiceOptions>(configuration);

            services.PostConfigure<OrderServiceOptions>(options =>
            {
                options.MaxOrderCount += 20;
            });

            services.AddSingleton<IOrderService, OrderService>();
            return services;
        }
    }
}
