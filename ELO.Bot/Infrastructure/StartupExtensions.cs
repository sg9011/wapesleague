using Microsoft.Extensions.DependencyInjection;
using Base.Bot.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace ELO.Bot.Infrastructure
{
    public static class StartupExtensions
    {
        public static void ConfigureBackgroundServices(this IServiceCollection services)
        {
            services.AddBaseBotServices();
        }
        public static void RegisterSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterBaseSettings(configuration);
        }
    }
}
