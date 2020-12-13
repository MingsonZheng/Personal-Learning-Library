using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelloApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HelloApi
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
            //var myOption = new MyOption();
            //Configuration.GetSection("MyOption").Bind(myOption);

            //// 通过 Get 的方式
            //myOption = Configuration.GetSection("MyOption").Get<MyOption>(); 

            //// 单例注入到全局中
            //services.AddSingleton(myOption);

            //// 直接注入到容器中
            //services.Configure<MyOption>(Configuration.GetSection("MyOption"));

            services.Configure<MyOption>("Peter", Configuration.GetSection("Peter"));
            services.Configure<MyOption>("Jack", Configuration.GetSection("Jack"));

            //services.AddOptions<MyOption>().Bind(Configuration.GetSection("MyOption")).ValidateDataAnnotations();

            //services.PostConfigure<MyOption>(option =>
            //{
            //    if (option.Age == 20)
            //    {
            //        option.Age = 19;
            //    }
            //});

            services.AddHostedService<MyBackgroundService>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // 默认启用 https
            app.UseHttpsRedirection();
            app.UseRouting();
            //app.UseCors();
            //app.UseAuthentication();
            //app.UseAuthorization();

            //// 添加一些自定义的中间件
            //app.Use(async (context, next) =>
            //{
            //    await context.Response.WriteAsync("my middleware 1");
            //    await next();
            //});

            //app.Run(async context =>
            //{
            //    await context.Response.WriteAsync("my middleware 2");
            //});

            // 路由和终结点之间的中间件可以拿到终结点的信息
            app.Use(next => context =>
            {
                // 获取当前已经被选择的终结点
                var endpoint = context.GetEndpoint();
                if (endpoint is null)
                {
                    return Task.CompletedTask;
                }
                // 输出终结点的名称
                Console.WriteLine($"Endpoint: {endpoint.DisplayName}");
                // 打印终结点匹配的路由
                if (endpoint is RouteEndpoint routeEndpoint)
                {
                    Console.WriteLine("Endpoint has route pattern: " +
                                      routeEndpoint.RoutePattern.RawText);
                }
                // 打印终结点的元数据
                foreach (var metadata in endpoint.Metadata)
                {
                    Console.WriteLine($"Endpoint has metadata: {metadata}");
                    // 打印 http 方法
                    if (metadata is HttpMethodMetadata httpMethodMetadata)
                    {
                        Console.WriteLine($"Current Http Method: {httpMethodMetadata.HttpMethods.FirstOrDefault()}");
                    }
                }

                return Task.CompletedTask;
            });

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();

                // 将终结点绑定到路由上
                endpoints.MapGet("/blog/{title}", async context =>
                    {
                        var title = context.Request.RouteValues["title"];
                        await context.Response.WriteAsync($"blog title: {title}");
                    }).WithDisplayName("Blog")// 修改名称
                    .WithMetadata("10001");// 修改元数据
            });

            //// 约定路由
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller=Home}/{action=Index}/{id?}");
            //});

            //// 约定路由也可以同时定义多个
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller=Home}/{action=Index}/{id?}");

            //    endpoints.MapControllerRoute(
            //        name: "blog",
            //        pattern: "blog/{*article}",
            //        defaults: new {controller = "blog", action = "Article"});
            //});
        }
    }
}
