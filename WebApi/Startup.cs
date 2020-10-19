using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using WebApi.Models;

namespace WebApi
{
    public class Startup
    {

         readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // So to use the AppSettings inside our application like controllers and stuff we need to do this.
            //Inject appsetting

            services.Configure<ApplicationSettings>(Configuration.GetSection("ApplicationSettings"));


            services.AddControllers();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddDbContext<AuthenticationContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("IdentityConnection")));


            services.AddIdentityCore<ApplicationUser>().
                AddRoles<IdentityRole>().
                AddEntityFrameworkStores<AuthenticationContext>();


            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 4;
            });


            services.AddCors(options =>
            {
                options.AddPolicy( "AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                    });

            });
            // JWT authehtication 

            var comb = "20e90447-43f0-44ab-be39-6319ac1dd3fb1d85fa45-0923-467d-bb25-12a0b76cc678";

            //var key = Encoding.UTF8.GetBytes(Configuration["ApplicationSettings : JWT_Secret"].ToString());


            var key = Encoding.UTF8.GetBytes("20e90447-43f0-44ab-be39-6319ac1dd3fb1d85fa45-0923-467d-bb25-12a0b76cc678".ToString());

            services.AddAuthentication(x =>
           {
               x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
               x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
               x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

           }).AddJwtBearer(x =>
          {
              x.RequireHttpsMetadata = false;
              x.SaveToken = false;
              x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
              {
                  ValidateIssuerSigningKey = true,
                  IssuerSigningKey = new SymmetricSecurityKey(key),
                  ValidateIssuer = false,
                  ValidateAudience = false,
                  ClockSkew = TimeSpan.Zero
              };

          }) ;

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.Use(async (ctx, next) =>
            {
                await next();

                if (ctx.Response.StatusCode == 204)
                {
                    ctx.Response.ContentLength = 0;
                }


            });
              


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseRouting();

            app.UseCors("AllowAll");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
