using System;
using System.Linq;

namespace WaPesLeague.Business.Dto.Server
{
    public class UpdateServerSettingsDto
    {
        public int ServerId { get; set; }
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
        public string DefaultTeamAName { get; set; }
        public bool DefaultTeamAOpen { get; set; }
        public string DefaultTeamBName { get; set; }
        public bool DefaultTeamBOpen { get; set; }
        public string Language { get; set; }
        public bool AllowActiveSwapCommand { get; set; }
        public bool AllowInactiveSwapCommand { get; set; }

        public DiscordCommandPropsDto DiscordCommandPropsDto { get; set; }

        public UpdateServerSettingsDto(DiscordCommandPropsDto propsDto, Data.Entities.Discord.Server serverDefaults)
        {
            var teamA = serverDefaults.DefaultTeams.First(x => x.Tags.Any(t => string.Equals(t.Tag, "A", StringComparison.InvariantCultureIgnoreCase)));
            var teamB = serverDefaults.DefaultTeams.First(x => x.Tags.Any(t => string.Equals(t.Tag, "B", StringComparison.InvariantCultureIgnoreCase)));
            ServerId = serverDefaults.ServerId;
            DefaultSessionRecurringWithAClosedTeam = serverDefaults.DefaultSessionRecurringWithAClosedTeam;
            DefaultSessionRecurringWithAllOpen = serverDefaults.DefaultSessionRecurringWithAllOpen;
            DefaultAutoCreateExtraSessionsWhenAllTeamsOpen = serverDefaults.DefaultAutoCreateExtraSessionsWhenAllTeamsOpen;
            DefaultAutoCreateExtraSessionsWithAClosedTeam = serverDefaults.DefaultAutoCreateExtraSessionsWithAClosedTeam;
            DefaultStartTime = serverDefaults.DefaultStartTime;
            DefaultHoursToOpenRegistrationBeforeStart = serverDefaults.DefaultHoursToOpenRegistrationBeforeStart;
            DefaultSessionDuration = serverDefaults.DefaultSessionDuration;
            DefaultSessionExtraInfo = serverDefaults.DefaultSessionExtraInfo;
            DefaultSessionPassword = serverDefaults.DefaultSessionPassword;
            UsePasswordForSessions = serverDefaults.UsePasswordForSessions;
            UseServerForSessions = serverDefaults.UseServerForSessions;
            ShowPESSideSelectionInfo = serverDefaults.ShowPESSideSelectionInfo;
            TimeZoneName = serverDefaults.TimeZoneName;
            Language = serverDefaults.Language;
            AllowActiveSwapCommand = serverDefaults.AllowActiveSwapCommand;
            AllowInactiveSwapCommand = serverDefaults.AllowInactiveSwapCommand;
            DefaultTeamAName = teamA.Name;
            DefaultTeamAOpen = teamA.IsOpen;
            DefaultTeamBName = teamB.Name;
            DefaultTeamBOpen = teamB.IsOpen;

            DiscordCommandPropsDto = propsDto;
        }   
    }
}
