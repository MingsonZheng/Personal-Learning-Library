using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.FileProviders;

namespace StaticFilesDemo
{
    public class Startup
    {
        const int BufferSize = 64 * 1024;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDirectoryBrowser();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // 通过这一行代码就可以访问到静态配置文件
            app.UseStaticFiles();

            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    // 我们希望把我们的静态目录映射为某一个特定的 URL 地址目录下面
            //    RequestPath = "/files",
            //    // 注入我们的物理文件提供程序，把我们的当前目录加 file，就是 file 目录，赋值给我们的提供程序
            //    // 这样子的效果就是我们的 wwwroot 会优先去寻找我们的文件，如果没有的话就会执行下一个中间件
            //    // 然后在这个中间件里面再找我们的文件是否存在，如果没有的话，它会去执行后面的路由和 MVC 的 Web API 的 Controller
            //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "file"))
            //});

            // 判断我们当前的请求是否满足条件
            app.MapWhen(context =>
            {
                // 如果我们的请求不是以 API 开头的请求
                return !context.Request.Path.Value.StartsWith("/api");
            }, appBuilder =>
            {
                //// 如果满足条件，我就走我下面这一段中间件的逻辑
                //var option = new RewriteOptions();
                //// 重写为 /index.html
                //option.AddRewrite(".*", "/index.html", true);
                //appBuilder.UseRewriter(option);

                //// 重写完之后再使用我们的静态文件中间件
                //appBuilder.UseStaticFiles();

                appBuilder.Run(async c =>
                {
                    // 读取静态文件，并且输出给我们的 Response
                    var file = env.WebRootFileProvider.GetFileInfo("index.html");
                    c.Response.ContentType = "text/html";
                    using (var fileStream = new FileStream(file.PhysicalPath, FileMode.Open, FileAccess.Read))
                    {
                        await StreamCopyOperation.CopyToAsync(fileStream, c.Response.Body, null, BufferSize, c.RequestAborted);
                    }
                });
            });

            //app.UseDefaultFiles();

            //app.UseDirectoryBrowser();

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
