using Microsoft.EntityFrameworkCore;
using WaPesLeague.Data.Entities.Association.Configurations;
using WaPesLeague.Data.Entities.Discord;
using WaPesLeague.Data.Entities.Discord.Configurations;
using WaPesLeague.Data.Entities.FileImport.Configurations;
using WaPesLeague.Data.Entities.Formation;
using WaPesLeague.Data.Entities.Formation.Configurations;
using WaPesLeague.Data.Entities.Formation.Seeders;
using WaPesLeague.Data.Entities.Match.Configurations;
using WaPesLeague.Data.Entities.Match.Seeders;
using WaPesLeague.Data.Entities.Mix;
using WaPesLeague.Data.Entities.Mix.Configurations;
using WaPesLeague.Data.Entities.Platform;
using WaPesLeague.Data.Entities.Platform.Configurations;
using WaPesLeague.Data.Entities.Platform.Seeders;
using WaPesLeague.Data.Entities.Position;
using WaPesLeague.Data.Entities.Position.Configurations;
using WaPesLeague.Data.Entities.Position.Seeders;
using WaPesLeague.Data.Entities.User;
using WaPesLeague.Data.Entities.User.Configurations;

namespace WaPesLeague.Data
{
    public class WaPesDbContext : DbContext
    {
        public WaPesDbContext(DbContextOptions<WaPesDbContext> options)
            : base (options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<UserPlatform> UserPlatforms { get; set; }
        public DbSet<UserMember> UserMembers { get; set; }

        public DbSet<Server> Servers { get; set; }
        public DbSet<ServerFormation> ServerFormations { get; set; }
        public DbSet<ServerFormationTag> ServerFormationTags { get; set; }
        public DbSet<ServerFormationPosition> ServerFormationPositions { get; set; }
        public DbSet<ServerTeam> ServerTeams { get; set; }
        public DbSet<ServerTeamTag> ServerTeamTags { get; set; }
        public DbSet<ServerRole> ServerRoles { get; set; }
        public DbSet<ServerEvent> ServerEvents { get; set; }


        public DbSet<MixGroup> MixGroups { get; set; }
        public DbSet<MixChannel> MixChannels { get; set; }
        public DbSet<MixChannelTeam> MixChannelTeams { get; set; }
        public DbSet<MixChannelTeamPosition> MixChannelTeamPositions { get; set; }
        public DbSet<MixChannelTeamTag> MixChannelTeamTags { get; set; }
        public DbSet<MixGroupRoleOpening> MixGroupRoleOpenings { get; set; }

        public DbSet<MixSession> MixSessions { get; set; }
        public DbSet<MixTeam> MixTeams { get; set; }
        public DbSet<MixTeamTag> MixTeamTags { get; set; }
        public DbSet<MixTeamRoleOpening> MixTeamRoleOpenings { get; set; }
        public DbSet<MixPosition> MixPositions { get; set; }
        public DbSet<MixPositionReservation> MixPositionReservations { get; set; }
        public DbSet<MixUserPositionSessionStat> MixUserPositionSessionStats { get; set; }

        public DbSet<Formation> Formations { get; set; }
        public DbSet<FormationPosition> FormationPositions { get; set; }
        public DbSet<FormationTag> FormationTags { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<PositionGroup> PositionGroups { get; set; }
        public DbSet<PositionTag> PositionTags { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserPlatformConfiguration());
            modelBuilder.ApplyConfiguration(new UserMemberConfiguration());

            modelBuilder.ApplyConfiguration(new ServerConfiguration());
            modelBuilder.ApplyConfiguration(new ServerTeamConfiguration());
            modelBuilder.ApplyConfiguration(new ServerTeamTagConfiguration());
            modelBuilder.ApplyConfiguration(new ServerFormationConfiguration());
            modelBuilder.ApplyConfiguration(new ServerFormationPositionConfiguration());
            modelBuilder.ApplyConfiguration(new ServerFormationTagConfiguration());
            modelBuilder.ApplyConfiguration(new ServerRoleConfiguration());
            modelBuilder.ApplyConfiguration(new ServerEventConfiguration());

            modelBuilder.ApplyConfiguration(new PlatformConfiguration());

            #region mix
            modelBuilder.ApplyConfiguration(new MixGroupConfiguration());
            modelBuilder.ApplyConfiguration(new MixGroupRoleOpeningConfiguration());

            modelBuilder.ApplyConfiguration(new MixChannelConfiguration());
            modelBuilder.ApplyConfiguration(new MixChannelTeamConfiguration());
            modelBuilder.ApplyConfiguration(new MixChannelTeamPositionConfiguration());
            modelBuilder.ApplyConfiguration(new MixChannelTeamTagConfiguration());

            modelBuilder.ApplyConfiguration(new MixSessionConfiguration());
            modelBuilder.ApplyConfiguration(new MixTeamConfiguration());
            modelBuilder.ApplyConfiguration(new MixTeamTagConfiguration());
            modelBuilder.ApplyConfiguration(new MixTeamRoleOpeningConfiguration());

            modelBuilder.ApplyConfiguration(new MixPositionConfiguration());
            modelBuilder.ApplyConfiguration(new MixPositionReservationConfiguration());
            modelBuilder.ApplyConfiguration(new MixUserPositionSessionStatConfiguration());
            #endregion

            modelBuilder.ApplyConfiguration(new PositionConfiguration());
            modelBuilder.ApplyConfiguration(new PositionGroupConfiguration());

            modelBuilder.ApplyConfiguration(new FormationConfiguration());
            modelBuilder.ApplyConfiguration(new FormationPositionConfiguration());
            modelBuilder.ApplyConfiguration(new FormationTagConfiguration());

            #region match
            modelBuilder.ApplyConfiguration(new MatchConfiguration());
            modelBuilder.ApplyConfiguration(new MatchTeamConfiguration());
            modelBuilder.ApplyConfiguration(new MatchTeamStatConfiguration());
            modelBuilder.ApplyConfiguration(new MatchTeamStatTypeConfiguration());

            modelBuilder.ApplyConfiguration(new MatchTeamPlayerConfiguration());
            modelBuilder.ApplyConfiguration(new MatchTeamPlayerEventConfiguration());
            modelBuilder.ApplyConfiguration(new MatchTeamPlayerStatConfiguration());
            modelBuilder.ApplyConfiguration(new MatchTeamPlayerStatTypeConfiguration());
            #endregion

            #region association
            modelBuilder.ApplyConfiguration(new AssociationTenantConfiguration());
            modelBuilder.ApplyConfiguration(new AssociationConfiguration());
            modelBuilder.ApplyConfiguration(new AssociationLeagueGroupConfiguration());
            modelBuilder.ApplyConfiguration(new AssociationLeagueSeasonConfiguration());
            modelBuilder.ApplyConfiguration(new DivisionConfiguration());
            modelBuilder.ApplyConfiguration(new DivisionRoundConfiguration());
            modelBuilder.ApplyConfiguration(new DivisionGroupConfiguration());
            modelBuilder.ApplyConfiguration(new DivisionGroupRoundConfiguration());

            modelBuilder.ApplyConfiguration(new AssociationTenantPlayerConfiguration());
            
            modelBuilder.ApplyConfiguration(new AssociationTeamConfiguration());
            modelBuilder.ApplyConfiguration(new AssociationTeamPlayerConfiguration());
            #endregion

            modelBuilder.ApplyConfiguration(new FileImportTypeConfiguration());
            modelBuilder.ApplyConfiguration(new FileImportConfiguration());
            modelBuilder.ApplyConfiguration(new FileImportRecordConfiguration());


            modelBuilder.SeedPositions();
            modelBuilder.SeedFormations();
            modelBuilder.SeedPlatforms();

            modelBuilder.SeedMatchTeamStatTypes();
            modelBuilder.SeedMatchTeamPlayerStatTypes();
        }
    }
}
