using Base.Bot.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WaPesLeague.Business.Infrastructure;
using WaPesLeague.Data;
using WaPesLeague.Data.Infrastructure;
using WaPesLeague.GoogleSheets.Infrastructure;

namespace WaPesLeague.API.Infrastructure
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers();
            services.AddMemoryCache();
            services.ConfigureDbContext<WaPesDbContext>(Configuration.GetConnectionString("WaPesLeagueDb"));
            services.ConfigureSwagger(Configuration);
            services.ConfigureAutomapper();

            services.RegisterBaseSettings(Configuration);
            services.RegisterManagers();
            services.RegisterWorkflows();
            services.RegisterGoogleHandlers();
            
            services.AddPingService(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, WaPesDbContext waPesDbContext, IApiVersionDescriptionProvider provider)
        {
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            waPesDbContext.Database.Migrate();

            app.UseSwagger(provider);
        }
    }
}
