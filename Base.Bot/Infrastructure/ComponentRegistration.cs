using Base.Bot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WaPesLeague.Constants.Resources;

namespace Base.Bot.Infrastructure
{
    public static class ComponentRegistration
    {
        public static void RegisterBaseSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration.GetSection(typeof(DiscordSettings).Name).Get<DiscordSettings>());
            services.AddSingleton(configuration.GetSection(typeof(DefaultServerSettings).Name).Get<DefaultServerSettings>());
            services.AddSingleton(configuration.GetSection(typeof(ErrorMessages).Name).Get<ErrorMessages>());
            services.AddSingleton(configuration.GetSection(typeof(GeneralMessages).Name).Get<GeneralMessages>());
        }

        public static void AddBaseBotServices(this IServiceCollection services)
        {
            services.AddSingleton<IHostedService, BotService>();
        }

        public static void AddPingService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration.GetSection(typeof(PingSettings).Name).Get<PingSettings>());
            services.AddSingleton<IHostedService, EndPointPingService>();
        }

        public static void ConfigureDbContext<TContext>(this IServiceCollection services, string connectionstring, ServiceLifetime lifeTime = ServiceLifetime.Scoped) where TContext : DbContext
        {
            services.AddDbContext<TContext>(delegate (DbContextOptionsBuilder options)
            {
                options.UseSqlServer(
                    connectionstring,
                    sqlServerOptionsAction: sqlOptions => {
                        sqlOptions.EnableRetryOnFailure();
                        sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                        sqlOptions.MigrationsAssembly("WaPesLeague.Data");
                    }
                );
            }, lifeTime);
        }

        public static void ConfigureAutomapper(this IServiceCollection services)
        {
            IEnumerable<Assembly> assemblies = from s in AppDomain.CurrentDomain.GetAssemblies()
                                               where s.GetTypes().Any((Type p) => p.IsSubclassOf(typeof(Profile)))
                                               select s;
            services.AddAutoMapper(assemblies);
        }
    }
}
