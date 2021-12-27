using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WaPesLeague.Bot.Services;
using Base.Bot.Infrastructure;

namespace WaPesLeague.Bot.Infrastructure
{
    public static class StartupExtensions
    {
        public static void ConfigureBackgroundServices(this IServiceCollection services)
        {
            services.AddBaseBotServices();
            services.AddSingleton<IHostedService, BotCloseSessionsService>();
            services.AddSingleton<IHostedService, BotNotifyService>();
            services.AddSingleton<IHostedService, RequestService>();
            services.AddSingleton<IHostedService, BotCalculateStatService>();
            services.AddSingleton<IHostedService, BotServerEventService>();
            services.AddSingleton<IHostedService, BotScanForServerMembersService>();
            //ToDo Activate this when Doing the google sheets
            //services.AddSingleton<IHostedService, ImportDailyGoogleSheetsService>();
            //services.AddSingleton<IHostedService, ProcessDailyFileImportsService>();
        }

        public static void RegisterSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterBaseSettings(configuration);
        }
    }
}
