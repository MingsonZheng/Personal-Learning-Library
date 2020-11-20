using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OptionsDemo.Services;

namespace OptionsDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddSingleton<OrderServiceOptions>();
            //services.Configure<OrderServiceOptions>(Configuration.GetSection("OrderService"));
            //services.AddSingleton<IOrderService, OrderService>();
            //services.AddScoped<IOrderService, OrderService>();

            services.AddOrderService(Configuration.GetSection("OrderService"));

            //services.AddOptions<OrderServiceOptions>().Configure(options =>
            //{
            //    Configuration.Bind(options);
            //}).Validate(options =>
            //{
            //    return options.MaxOrderCount <= 100;
            //}, "MaxOrderCount 不能大于100");

            //services.AddOptions<OrderServiceOptions>().Configure(options =>
            //{
            //    Configuration.Bind(options);
            //}).ValidateDataAnnotations();

            services.AddOptions<OrderServiceOptions>().Configure(options =>
            {
                Configuration.Bind(options);
            }).Services.AddSingleton<IValidateOptions<OrderServiceOptions>>(new OrderServiceValidateOptions());

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
