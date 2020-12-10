using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelloApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
