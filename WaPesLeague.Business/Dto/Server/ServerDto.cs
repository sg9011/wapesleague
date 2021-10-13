using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaPesLeague.Business.Helpers;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;
using WaPesLeague.Data.Helpers;

namespace WaPesLeague.Business.Dto.Server
{
    public class ServerDto
    {
        public string ServerName { get; set; }
        public bool IsActive { get; set; }
        public bool DefaultSessionRecurringWithAClosedTeam { get; set; }
        public bool DefaultSessionRecurringWithAllOpen { get; set; }
        public bool DefaultAutoCreateExtraSessionsWhenAllTeamsOpen { get; set; }
        public bool DefaultAutoCreateExtraSessionsWithAClosedTeam { get; set; }
        public Time DefaultStartTime { get; set; }
        public Time DefaultHoursToOpenRegistrationBeforeStart { get; set; }
        public Time DefaultSessionDuration { get; set; }
        public string DefaultSessionExtraInfo { get; set; }

        public bool UsePasswordForSessions { get; set; }
        public string DefaultSessionPassword { get; set; }

        public bool UseServerForSessions { get; set; }
        public bool ShowPESSideSelectionInfo { get; set; }


        public string DefaultFormation { get; set; }
        public IReadOnlyCollection<ServerTeamDto> DefaultTeams { get; set; }
        public string TimeZoneName { get; set; }
        public string Language { get; set; }
        public bool AllowActiveSwapCommand { get; set; }
        public bool AllowInactiveSwapCommand { get; set; }

        public string ToDiscordString(GeneralMessages generalMessages)
        {
            var stringBuilder = new StringBuilder();
            foreach(var team in DefaultTeams)
            {
                stringBuilder.Append(team.ToDiscordString(generalMessages));
            }
            var defaulSessionPassword = string.IsNullOrWhiteSpace(DefaultSessionPassword) ? generalMessages.NoValueDefined.GetValueForLanguage() : DefaultSessionPassword;
            return string.Format(generalMessages.ServerInfo.GetValueForLanguage(),
                ServerName,DefaultStartTime.ToDiscordString(),
                DefaultHoursToOpenRegistrationBeforeStart.ToDiscordString(),
                DefaultSessionDuration.ToDiscordString(),
                DefaultSessionExtraInfo,
                DefaultFormation,
                DefaultSessionRecurringWithAClosedTeam.ToDiscordString(generalMessages),
                DefaultSessionRecurringWithAllOpen.ToDiscordString(generalMessages),
                DefaultAutoCreateExtraSessionsWithAClosedTeam.ToDiscordString(generalMessages),
                DefaultAutoCreateExtraSessionsWhenAllTeamsOpen.ToDiscordString(generalMessages),
                UsePasswordForSessions.ToDiscordString(generalMessages),
                defaulSessionPassword,
                UseServerForSessions.ToDiscordString(generalMessages),
                ShowPESSideSelectionInfo.ToDiscordString(generalMessages),
                AllowActiveSwapCommand.ToDiscordString(generalMessages),
                AllowInactiveSwapCommand.ToDiscordString(generalMessages),
                stringBuilder,
                TimeZoneName,
                Language,
                Bot.Prefix);
        }
    }

    public class ServerTeamDto
    {
        public string Name { get; set; }
        public bool IsOpen { get; set; }
        public List<string> Tags { get; set; }

        public string ToDiscordString(GeneralMessages generalMessages)
        {
            var isOpenString = IsOpen.ToDiscordString(generalMessages);
            var tagsString = Tags.Any()
                ? $" - ({string.Join(", ", Tags)})"
                : "";
            return $"\n\t{generalMessages.TeamName.GetValueForLanguage()}: {Name}, {generalMessages.IsOpen.GetValueForLanguage()}: {isOpenString}{tagsString}";
        }
    }
}
