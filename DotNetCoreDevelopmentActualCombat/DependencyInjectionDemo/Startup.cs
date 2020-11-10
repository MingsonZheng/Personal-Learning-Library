using DependencyInjectionDemo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace DependencyInjectionDemo
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
            #region 注册服务不同生命周期的服务

            // 将单例的服务注册为单例的模式
            services.AddSingleton<IMySingletonService, MySingletonService>();

            // Scoped 的服务注册为 Scoped 的生命周期
            services.AddScoped<IMyScopedService, MyScopedService>();

            // 瞬时的服务注册为瞬时的生命周期
            services.AddTransient<IMyTransientService, MyTransientService>();

            #endregion

            #region 花式注册

            services.AddSingleton<IOrderService>(new OrderService1());  //直接注入实例
            services.AddSingleton<IOrderService, OrderService2>();

            #endregion

            #region 尝试注册（如果服务已经注册过，则不再注册）

            services.TryAddSingleton<IOrderService, OrderService2>();// 接口类型重复，则不注册
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IOrderService, OrderService1>());// 相同类型的接口，实现类相同，则不注册

            #endregion

            // 因为已经注册过 OrderService，所以第二句代码不生效
            services.AddSingleton<IOrderService>(new OrderService1());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IOrderService, OrderService1>());

            // 以不同的实现注册服务
            services.AddSingleton<IOrderService>(new OrderService1());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IOrderService, OrderService2>());

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
