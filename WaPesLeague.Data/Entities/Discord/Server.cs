using System.Collections.Generic;
using System.Globalization;
using WaPesLeague.Data.Entities.Formation;
using WaPesLeague.Data.Entities.Mix;
using WaPesLeague.Data.Entities.Position;
using WaPesLeague.Data.Entities.User;

namespace WaPesLeague.Data.Entities.Discord
{
    public class Server
    {
        public int ServerId { get; set; }
        public string DiscordServerId { get; set; }
        public string ServerName { get; set; }
        public bool IsActive { get; set; }
        public bool DefaultSessionRecurringWithAClosedTeam { get; set; }
        public bool DefaultSessionRecurringWithAllOpen { get; set; }
        public bool DefaultAutoCreateExtraSessionsWhenAllTeamsOpen { get; set; }
        public bool DefaultAutoCreateExtraSessionsWithAClosedTeam { get; set; }
        public decimal DefaultStartTime { get; set; }
        public decimal DefaultHoursToOpenRegistrationBeforeStart { get; set; }
        public decimal DefaultSessionDuration { get; set; }
        public string DefaultSessionExtraInfo { get; set; }
        public string DefaultSessionPassword { get; set; }
        public bool UsePasswordForSessions { get; set; }
        public bool UseServerForSessions { get; set; }
        public bool ShowPESSideSelectionInfo { get; set; }
        public string TimeZoneName { get; set; }
        public string Language { get; set; }
        public bool AllowActiveSwapCommand { get; set; }
        public bool AllowInactiveSwapCommand { get; set; }

        public virtual List<UserMember> Members { get; set; }
        public virtual List<MixGroup> MixGroups { get; set; }
        public virtual List<ServerFormation> ServerFormations { get; set; }
        public virtual List<ServerTeam> DefaultTeams { get; set; }
        public virtual List<PositionTag> PositionTags { get; set; }

        //public CultureInfo GetCultureInfo => new CultureInfo(Language ?? Constants.Bot.SupportedLanguages.English);
        public void SetDefaultThreadCurrentCulture()
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(Language ?? Constants.Bot.SupportedLanguages.English);
        }
    }
}
