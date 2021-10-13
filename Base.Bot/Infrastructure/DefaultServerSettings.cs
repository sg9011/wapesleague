using System.Collections.Generic;

namespace Base.Bot.Infrastructure
{
    public class DefaultServerSettings
    {
        public bool DefaultSessionRecurringWithAClosedTeam { get; set; }
        public bool DefaultSessionRecurringWithAllOpen { get; set; }
        public bool DefaultAutoCreateExtraSessionsWhenAllTeamsOpen { get; set; }
        public bool DefaultAutoCreateExtraSessionsWithAClosedTeam { get; set; }
        public decimal DefaultStartTime { get; set; }
        public decimal DefaultHoursToOpenRegistrationBeforeStart { get; set; }
        public decimal DefaultSessionDuration { get; set; }
        public string DefaultSessionExtraInfo { get; set; }
        public bool DefaultUsePasswordForSessions { get; set; }
        public bool DefaultUseServerForSessions { get; set; }
        public bool DefaultShowPESSideSelectionInfo { get; set; }
        public List<BotSettingTeam> Teams { get; set; }
        public string TimeZoneName { get; set; }
        public bool AllowActiveSwapCommand { get; set; }
        public bool AllowInactiveSwapCommand { get; set; }
    }

    public class BotSettingTeam
    {
        public string Name { get; set; }
        public bool IsOpen { get; set; }
        public List<string> Tags { get; set; }

    }
}
