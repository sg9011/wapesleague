using Microsoft.Extensions.DependencyInjection;
using WaPesLeague.Business.Workflows;
using WaPesLeague.Business.Workflows.Interfaces;

namespace WaPesLeague.Business.Infrastructure
{
    public static class ComponentRegistration
    {
        public static void RegisterWorkflows(this IServiceCollection services)
        {
            services.AddScoped<IServerWorkflow, ServerWorkflow>();
            services.AddScoped<IMixGroupWorkflow, MixGroupWorkflow>();
            services.AddScoped<IMixChannelWorkflow, MixChannelWorkflow>();
            services.AddScoped<IMixSessionWorkflow, MixSessionWorkflow>();
            services.AddScoped<IMixStatsWorkflow, MixStatsWorkflow>();
            services.AddScoped<IUserWorkflow, UserWorkflow>();
            services.AddScoped<IPlatformWorkflow, PlatformWorkflow>();
            services.AddScoped<IServerFormationWorkflow, ServerFormationWorkflow>();
            services.AddScoped<IPositionWorkflow, PositionWorkflow>();
            services.AddScoped<IServerRoleWorkflow, ServerRoleWorkflow>();
            services.AddScoped<IMixGroupRoleOpeningWorkflow, MixGroupRoleOpeningWorkflow>();
        }
    }
}
