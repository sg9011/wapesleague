using Microsoft.Extensions.DependencyInjection;
using WaPesLeague.GoogleSheets.Interfaces;

namespace WaPesLeague.GoogleSheets.Infrastructure
{
    public static class ComponentRegistration
    {
        public static void RegisterGoogleHandlers(this IServiceCollection services)
        {
            services.AddScoped<IImportFileTabDataHandler, ImportFileTabDataHandler>();
        }
    }
}
