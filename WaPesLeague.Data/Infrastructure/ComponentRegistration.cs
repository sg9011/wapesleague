﻿using Microsoft.Extensions.DependencyInjection;
using WaPesLeague.Data.Managers;
using WaPesLeague.Data.Managers.Association;
using WaPesLeague.Data.Managers.Association.Interfaces;
using WaPesLeague.Data.Managers.FileImport;
using WaPesLeague.Data.Managers.FileImport.Intefaces;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Infrastructure
{
    public static class ComponentRegistration
    {
        public static void RegisterManagers(this IServiceCollection services)
        {
            services.AddScoped<IMetadataManager, MetadataManager>();
            services.AddScoped<IMixGroupManager, MixGroupManager>();
            services.AddScoped<IMixGroupRoleOpeningManager, MixGroupRoleOpeningManager>();

            services.AddScoped<IMixChannelManager, MixChannelManager>();
            services.AddScoped<IMixChannelTeamManager, MixChannelTeamManager>();
            services.AddScoped<IMixChannelTeamTagManager, MixChannelTeamTagManager>();
            services.AddScoped<IMixChannelTeamPositionManager, MixChannelTeamPositionManager>();

            services.AddScoped<IMixPositionReservationManager, MixPositionReservationManager>();
            services.AddScoped<IMixPositionManager, MixPositionManager>();
            services.AddScoped<IMixSessionManager, MixSessionManager>();
            services.AddScoped<IMixTeamManager, MixTeamManager>();
            services.AddScoped<IMixTeamRoleOpeningManager, MixTeamRoleOpeningManager>();
            services.AddScoped<IMixUserPositionSessionStatManager, MixUserPositionSessionStatManager>();

            services.AddScoped<IPlatformManager, PlatformManager>();
            services.AddScoped<IServerManager, ServerManager>();
            services.AddScoped<IServerTeamManager, ServerTeamManager>();
            services.AddScoped<IServerFormationManager, ServerFormationManager>();
            services.AddScoped<IServerRoleManager, ServerRoleManager>();
            services.AddScoped<IServerButtonManager, ServerButtonManager>();
            services.AddScoped<IServerButtonGroupManager, ServerButtonGroupManager>();
            services.AddScoped<IServerSnipingManager, ServerSnipingManager>();
            services.AddScoped<ISniperManager, SniperManager>();
            services.AddScoped<IUserMemberServerRoleManager, UserMemberServerRoleManager>();

            services.AddScoped<IFormationManager, FormationManager>();
            services.AddScoped<IPositionManager, PositionManager>();
            services.AddScoped<IPositionGroupManager, PositionGroupManager>();
            services.AddScoped<IPositionTagManager, PositionTagManager>();
            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped<IUserPlatformManager, UserPlatformManager>();
            services.AddScoped<IUserMemberManager, UserMemberManager>();
            services.AddScoped<IUserMetadataManager, UserMetadataManager>();

            services.AddScoped<IGoogleSheetImportManager, GoogleSheetImportManager>();
            services.AddScoped<IFileImportManager, FileImportManager>();
            services.AddScoped<IFileImportRecordManager, FileImportRecordManager>();

            services.AddScoped<IAssociationTenantManager, AssociationTenantManager>();
            services.AddScoped<IAssociationManager, AssociationManager>();
            services.AddScoped<IAssociationTeamManager, AssociationTeamManager>();
            services.AddScoped<IAssociationTenantPlayerManager, AssociationTenantPlayerManager>();
            services.AddScoped<IDivisionGroupManager, DivisionGroupManager>();
            services.AddScoped<IDivisionGroupRoundManager, DivisionGroupRoundManager>();
        }
    }
}
