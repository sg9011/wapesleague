using System;
using System.Collections.Generic;
using System.Linq;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;
using WaPesLeague.Data.Entities.Association;
using WaPesLeague.Data.Entities.Match;
using WaPesLeague.Data.Entities.Mix;
using WaPesLeague.Data.Entities.Platform.Constants;

namespace WaPesLeague.Data.Entities.User
{
    public class User
    {
        public int UserId { get; set; }
        public Guid UserGuid { get; set; }
        public string DiscordName { get; set; }
        public string DiscordDiscriminator { get; set; }
        public string ExternalId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ExtraInfo { get; set; }
        public string Email { get; set; }

        public virtual List<MixPositionReservation> MixPositionReservations { get; set; }
        //public virtual List<UserSocialMedia> SocialMedias { get; set; }
        //public virtual List<UserPictureType> PictureTypes { get; set; }
        public virtual List<UserMetadata> UserMetadatas { get; set; }
        public virtual List<UserPlatform> PlatformUsers { get; set; }
        public virtual List<UserMember> UserMembers { get; set; }
        public virtual List<MixSession> OwnerOfSessions { get; set; }
        public virtual List<AssociationTenantPlayer> AssociationTenantPlayers { get; set; }
        public virtual List<MatchTeam> MatchTeamsConfirmedByUser { get; set; }
        public virtual List<MixUserPositionSessionStat> PositionSessionStats { get; set; }

        //Always assume it is filtered by discord serverId
        public string ToGetPlatformDiscordSting(GeneralMessages generalMessages)
        {
            var discordUser = UserMembers?.FirstOrDefault();
            var discordDisplayName = string.IsNullOrWhiteSpace(discordUser?.DiscordNickName)
                ? discordUser?.DiscordUserName ?? generalMessages.NoNameFound.GetValueForLanguage()
                : discordUser?.DiscordNickName;
            //ToDo Escape Discord characters Bold, Italic etc (* _ )

            var psnUserName = PlatformUsers?.FirstOrDefault(p => string.Equals(p.Platform.Name, PlatformConstants.PlayStationNetwork, StringComparison.InvariantCultureIgnoreCase))?.UserName;
            psnUserName = string.IsNullOrWhiteSpace(psnUserName)
                ? ""
                : $" ({psnUserName})";
            return $"{discordDisplayName}{psnUserName}";
        }
    }
}