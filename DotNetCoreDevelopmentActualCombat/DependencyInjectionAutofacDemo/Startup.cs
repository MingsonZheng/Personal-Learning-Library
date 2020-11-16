using Autofac;
using Autofac.Extensions.DependencyInjection;
using DependencyInjectionAutofacDemo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace DependencyInjectionAutofacDemo
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
            services.AddControllers();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            //builder.RegisterType<MyService>().As<IMyService>();

            //// 命名注册，当需要把一个服务注册多次，并且用不同命名作为区分的时候，可以用这种方式，入参是一个服务名
            //builder.RegisterType<MyServiceV2>().Named<IMyService>("service2");

            //// 属性注入，只需要在注册方法加上 PropertiesAutowired 即可
            //builder.RegisterType<MyNameService>();
            //builder.RegisterType<MyServiceV2>().As<IMyService>().PropertiesAutowired();

            //// 把拦截器注册到容器里面
            //builder.RegisterType<MyInterceptor>();
            //// 注册 MyServiceV2，并且允许它属性注册 （PropertiesAutowired）
            //// 开启拦截器需要使用 InterceptedBy 方法，并且注册类型 MyInterceptor
            //// 最后还要执行一个开关 EnableInterfaceInterceptors 允许接口拦截器
            //builder.RegisterType<MyServiceV2>().As<IMyService>().PropertiesAutowired().InterceptedBy(typeof(MyInterceptor)).EnableInterfaceInterceptors();

            // Autofac 具备给子容器进行命名的特性，可以把以服务注入到子容器中，并且是特定命名的子容器，这就意味着在其他的子容器是获取不到这个对象的
            builder.RegisterType<MyNameService>().InstancePerMatchingLifetimeScope("myscope");
        }

        public ILifetimeScope AutofacContainer { get; private set; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // 注册根容器
            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            //// Autofac 容器获取实例的方式是一组 Resolve 方法
            //var service = this.AutofacContainer.ResolveNamed<IMyService>("service2");
            //service.ShowCode();

            //// 获取没有命名的服务，把 namd 去掉即可
            //var servicenamed = this.AutofacContainer.Resolve<IMyService>();
            //servicenamed.ShowCode();

            // 创建一个 myscope 的子容器
            using (var myscope = AutofacContainer.BeginLifetimeScope("myscope"))
            {
                var service0 = myscope.Resolve<MyNameService>();
                using (var scope = myscope.BeginLifetimeScope())
                {
                    var service1 = scope.Resolve<MyNameService>();
                    var service2 = scope.Resolve<MyNameService>();
                    Console.WriteLine($"service1=service2:{service1 == service2}");
                    Console.WriteLine($"service1=service0:{service1 == service0}");
                }
            }

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
