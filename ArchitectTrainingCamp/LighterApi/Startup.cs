using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lighter.Application;
using Lighter.Application.Contracts;
using Microsoft.Extensions.Configuration;
using LighterApi.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace LighterApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<LighterDbContext>(options =>
            {
                options.UseMySql(Configuration.GetConnectionString("LighterDbContext"));
            });

            //services.AddDbContextPool<LighterDbContext>(options =>
            //{
            //    options.UseMySql(Configuration.GetConnectionString("LighterDbContext"));
            //});

            services.AddScoped<IQuestionService, QuestionService>()
                .AddScoped<IAnswerService, AnswerService>();

            services.AddControllers()
                .AddNewtonsoftJson(x=>x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            services.AddSingleton<IMongoClient>(sp =>
            {
                return new MongoClient(Configuration.GetConnectionString("LighterMongoServer"));
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                    options => options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true, // 是否验证 Issuer
                        ValidateAudience = true, // 是否验证 Audience
                        ValidateLifetime = true, // 是否验证失效时间
                        ClockSkew = TimeSpan.FromSeconds(30),
                        ValidateIssuerSigningKey = true, // 是否验证 SecurityKey
                        ValidAudience = "https://localhost:6001",
                        ValidIssuer = "https://localhost:6001",
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes("secret88secret666")) // 拿到 SecurityKey
                    });

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("policyName", policy =>
            //    {
            //        policy.RequireRole("role");
            //        policy.RequireClaim("claim");
            //        policy.Requirements.Add(new MinimumAgeRequirement());
            //    });
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
