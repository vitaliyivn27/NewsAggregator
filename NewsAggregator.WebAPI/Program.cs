using Hangfire;
using Hangfire.SqlServer;
using HtmlAgilityPack;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NewsAggregator.Business.ServicesImplementations;
using NewsAggregator.Core.Abstractions;
using NewsAggregator.Data.Abstractions;
using NewsAggregator.Data.Abstractions.Repositories;
using NewsAggregator.Data.CQS.Commands;
using NewsAggregator.Data.Repositories;
using NewsAggregator.DataBase;
using NewsAggregator.DataBase.Entities;
using NewsAggregator.WebAPI.Utils;
using Serilog;
using Serilog.Events;
using System.Text;

namespace NewsAggregator.WebAPI
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var myCorsPolicyName = "myCors";

            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((ctx, lc) =>
                lc.WriteTo.File(
                        @"E:\Vitalik\Second_course\Logger\logs\data_api.log",
                        LogEventLevel.Information)
                    .WriteTo.Console(LogEventLevel.Verbose));

            // Add services to the container.

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(myCorsPolicyName, policyBuilder =>
                {
                    policyBuilder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();
                });
            });

            builder.Services.AddControllers();

            var connectionString = builder.Configuration.GetConnectionString("Default");

            builder.Services.AddDbContext<GoodNewsAggregatorContext>(
            optionsBuilder => optionsBuilder.UseSqlServer(connectionString));

            builder.Services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(connectionString,
                    new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true,
                    }));

            // Add the processing server as IHostedService
            builder.Services.AddHangfireServer();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.AddScoped<IArticleService, ArticleService>();
            builder.Services.AddScoped<ISourceService, SourceService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IAdditionalArticleRepository, ArticleGenericRepository>();
            builder.Services.AddScoped<IRepository<Source>, Repository<Source>>();
            builder.Services.AddScoped<IRepository<User>, Repository<User>>();
            builder.Services.AddScoped<IRepository<Role>, Repository<Role>>();
            builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
            builder.Services.AddScoped<ISourceRepository, SourceRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IJwtUtil, JwtUtilSha256>();
            builder.Services.AddScoped<HtmlWeb>();

            builder.Services.AddMediatR(typeof(AddArticleDataFromRssFeedCommand).Assembly);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.IncludeXmlComments(builder.Configuration["XmlDoc"]);
            });

            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(opt =>
                {
                    opt.RequireHttpsMetadata = false;
                    opt.SaveToken = true;
                    opt.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = builder.Configuration["Token:Issuer"],
                        ValidAudience = builder.Configuration["Token:Issuer"],
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:JwtSecret"])),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            var app = builder.Build();
            app.UseStaticFiles();
            app.UseHangfireDashboard();
            app.UseRouting();

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapHangfireDashboard();

            app.UseCors(myCorsPolicyName);

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();


            app.Run();
        }
    }
}