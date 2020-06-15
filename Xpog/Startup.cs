namespace Xpog
{
    using Ef6CoreForPosgreSQL.Models;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Text;
    using Xpog.Models;
    using Xpog.Services;
    using Xpog.Services.CronJobs;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddDbContext<ExpenseAppContext>(opt =>
                opt.UseNpgsql(Configuration.GetConnectionString("DbConnection")));

            services.AddControllers();
            var jwtOptions = new UserJwtOptions();
            Configuration.GetSection(UserJwtOptions.Position).Bind(jwtOptions);
            services.AddAuthentication(x =>
              {
                  x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                  x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
              })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    //IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["Jwt:Key"])),
                    IssuerSigningKey = new SymmetricSecurityKey(jwtOptions.GetByteKey()),

                    ValidateIssuer = false,
                    ValidateAudience = false,

                };
            });
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IHashingService, HashingService>();
            services.AddScoped<IJWTService, UserJWTService>();

            services.AddCronJob<RepeatableExpensesSupervisorCronJob>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"10 0 * * *";
            }); 
            
            services.AddCronJob<MonthlyExpensesSupervisorCronJob>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"15 0 * * *";
            });


        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

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
