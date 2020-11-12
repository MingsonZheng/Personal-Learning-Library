using DependencyInjectionScopeAndDisposableDemo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DependencyInjectionScopeAndDisposableDemo
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
            //// 瞬时服务在每一次获取的时候都会获得一个新的对象，两个对象的 HashCode 不同
            //services.AddTransient<IOrderService, DisposableOrderService>();

            //services.AddScoped<IOrderService>(p => new DisposableOrderService());

            //// 把服务切换为单例模式，通过工厂的方式
            //services.AddSingleton<IOrderService>(p => new DisposableOrderService());

            //// 切换为瞬时模式，通过工厂的方式
            //services.AddTransient<IOrderService>(p => new DisposableOrderService());

            //// 把服务调整为自己创建，并注册进去
            //var service = new DisposableOrderService();
            //services.AddSingleton<IOrderService>(service);

            //services.AddSingleton<IOrderService, DisposableOrderService>();

            services.AddTransient<IOrderService, DisposableOrderService>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var s = app.ApplicationServices.GetService<IOrderService>();

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
