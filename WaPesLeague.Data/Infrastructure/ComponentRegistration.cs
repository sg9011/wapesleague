using Microsoft.Extensions.DependencyInjection;
using WaPesLeague.Data.Managers;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Infrastructure
{
    public static class ComponentRegistration
    {
        public static void RegisterManagers(this IServiceCollection services)
        {
            services.AddScoped<IMixGroupManager, MixGroupManager>();

            services.AddScoped<IMixChannelManager, MixChannelManager>();
            services.AddScoped<IMixChannelTeamManager, MixChannelTeamManager>();
            services.AddScoped<IMixChannelTeamTagManager, MixChannelTeamTagManager>();
            services.AddScoped<IMixChannelTeamPositionManager, MixChannelTeamPositionManager>();

            services.AddScoped<IMixPositionReservationManager, MixPositionReservationManager>();
            services.AddScoped<IMixPositionManager, MixPositionManager>();
            services.AddScoped<IMixSessionManager, MixSessionManager>();
            services.AddScoped<IMixTeamManager, MixTeamManager>();
            services.AddScoped<IMixUserPositionSessionStatManager, MixUserPositionSessionStatManager>();

            services.AddScoped<IPlatformManager, PlatformManager>();
            services.AddScoped<IServerManager, ServerManager>();
            services.AddScoped<IServerTeamManager, ServerTeamManager>();
            services.AddScoped<IServerFormationManager, ServerFormationManager>();

            services.AddScoped<IFormationManager, FormationManager>();
            services.AddScoped<IPositionManager, PositionManager>();
            services.AddScoped<IPositionGroupManager, PositionGroupManager>();
            services.AddScoped<IPositionTagManager, PositionTagManager>();
            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped<IUserPlatformManager, UserPlatformManager>();
            services.AddScoped<IUserMemberManager, UserMemberManager>();
        }
    }
}
