using Base.Bot.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WaPesLeague.Business.Infrastructure;
using WaPesLeague.Data;
using WaPesLeague.Data.Infrastructure;

namespace ELO.Bot.Infrastructure
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMemoryCache();
            services.ConfigureDbContext<WaPesDbContext>(Configuration.GetConnectionString("WaPesLeagueDb"));
            services.ConfigureAutomapper();

            services.RegisterSettings(Configuration);
            services.RegisterManagers();
            services.RegisterWorkflows();

            services.AddScoped<Base.Bot.Bot>();

            services.ConfigureBackgroundServices();
        }
        public void Configure(WaPesDbContext waPesDbContext)
        {
            waPesDbContext.Database.Migrate();
        }
    }
}
