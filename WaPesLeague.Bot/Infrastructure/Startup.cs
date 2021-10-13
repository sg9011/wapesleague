using Base.Bot.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WaPesLeague.Business.Infrastructure;
using WaPesLeague.Data;
using WaPesLeague.Data.Infrastructure;

namespace WaPesLeague.Bot.Infrastructure
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

            services.AddPingService(Configuration);
            services.ConfigureBackgroundServices();
        }
        public void Configure(IApplicationBuilder app, WaPesDbContext waPesDbContext)
        {
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            waPesDbContext.Database.Migrate();
        }
    }
}
