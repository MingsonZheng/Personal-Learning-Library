using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace MiddlewareDemo
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (context, next) =>
            {
                //await context.Response.WriteAsync("Hello");
                await next();
                await context.Response.WriteAsync("Hello2");
            });


            app.Map("/abc", abcBuilder =>
            {
                abcBuilder.Use(async (context, next) =>
                {
                    //await context.Response.WriteAsync("Hello");
                    await next();
                    await context.Response.WriteAsync("Hello2");
                });
            });


            app.MapWhen(context =>
            {
                return context.Request.Query.Keys.Contains("abc");
            }, builder =>
            {
                builder.Run(async context =>
                {
                    await context.Response.WriteAsync("new abc");
                });

            });

            app.UseMyMiddleware();

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
