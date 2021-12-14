using System.Collections.Generic;
using WaPesLeague.Business.Dto.Server;
using WaPesLeague.Constants.Resources;

namespace WaPesLeague.Business.Helpers
{
    public static class ServerOptionsHelper
    {
        public static void MapOptionsToDto(this UpdateServerSettingsDto dto, string optionsText, ErrorMessages errorMessages)
        {
            var options = BaseOptionsHelper.SplitStringToOptions(optionsText);

            dto.DefaultTeamAName = options.GetValueForParams(ServerOptionsTags.TeamAParam()) ?? dto.DefaultTeamAName;
            dto.DefaultTeamBName = options.GetValueForParams(ServerOptionsTags.TeamBParam()) ?? dto.DefaultTeamBName;
            dto.DefaultTeamAOpen = options.GetValueForParams(ServerOptionsTags.TeamAIsOpenParam()).OptionStringToBool(errorMessages, dto.DefaultTeamAOpen);
            dto.DefaultTeamBOpen = options.GetValueForParams(ServerOptionsTags.TeamBIsOpenParam()).OptionStringToBool(errorMessages, dto.DefaultTeamBOpen);

            dto.DefaultSessionRecurringWithAClosedTeam = options.GetValueForParams(ServerOptionsTags.RecurringWithAClosedTeamParam()).OptionStringToBool(errorMessages, dto.DefaultSessionRecurringWithAClosedTeam);
            dto.DefaultSessionRecurringWithAllOpen = options.GetValueForParams(ServerOptionsTags.RecurringWithAllOpenParam()).OptionStringToBool(errorMessages, dto.DefaultSessionRecurringWithAllOpen);

            dto.DefaultAutoCreateExtraSessionsWithAClosedTeam = options.GetValueForParams(ServerOptionsTags.AutoCreateExtraSessionsWhenAClosedTeamParam()).OptionStringToBool(errorMessages, dto.DefaultAutoCreateExtraSessionsWithAClosedTeam);
            dto.DefaultAutoCreateExtraSessionsWhenAllTeamsOpen = options.GetValueForParams(ServerOptionsTags.AutoCreateExtraSessionsWhenAllTeamsOpenParam()).OptionStringToBool(errorMessages, dto.DefaultAutoCreateExtraSessionsWhenAllTeamsOpen);

            dto.DefaultStartTime = options.GetValueForParams(ServerOptionsTags.StartTimeParam()).OptionStringToDecimal(dto.DefaultStartTime, errorMessages);
            dto.DefaultHoursToOpenRegistrationBeforeStart = options.GetValueForParams(ServerOptionsTags.HoursToOpenBeforeRegistrationParam()).OptionStringToDecimal(dto.DefaultHoursToOpenRegistrationBeforeStart, errorMessages);
            dto.DefaultSessionDuration = options.GetValueForParams(ServerOptionsTags.SessionDurationParam()).OptionStringToDecimal(dto.DefaultSessionDuration, errorMessages);

            dto.DefaultSessionExtraInfo = options.GetValueForParams(ServerOptionsTags.SessionExtraInfoParam()) ?? dto.DefaultSessionExtraInfo;
            dto.DefaultSessionPassword = options.GetValueForParams(ServerOptionsTags.SessionPasswordParam()) ?? dto.DefaultSessionPassword;

            dto.UsePasswordForSessions = options.GetValueForParams(ServerOptionsTags.UsePasswordParam()).OptionStringToBool(errorMessages, dto.UsePasswordForSessions);
            dto.UseServerForSessions = options.GetValueForParams(ServerOptionsTags.UseServerParam()).OptionStringToBool(errorMessages, dto.UseServerForSessions);
            dto.ShowPESSideSelectionInfo = options.GetValueForParams(ServerOptionsTags.ShowPESSideSelectionInfoParam()).OptionStringToBool(errorMessages, dto.ShowPESSideSelectionInfo);

            dto.TimeZoneName = options.GetValueForParams(ServerOptionsTags.TimeZoneParam()) ?? dto.TimeZoneName;
            dto.Language = options.GetValueForParams(ServerOptionsTags.LanguageParam())?.Trim().ToUpper() ?? dto.Language;
            dto.AllowActiveSwapCommand = options.GetValueForParams(ServerOptionsTags.AllowActiveSwapCommandInfoParam()).OptionStringToBool(errorMessages, dto.AllowActiveSwapCommand);
            dto.AllowInactiveSwapCommand = options.GetValueForParams(ServerOptionsTags.AllowInactiveSwapCommandInfoParam()).OptionStringToBool(errorMessages, dto.AllowInactiveSwapCommand);
        }

        public static class ServerOptionsTags
        {
            public static List<string> RecurringWithAClosedTeamParam()
            {
                return new List<string>()
                {
                    "RecClosed", "RecurringClosed"
                };
            }

            public static List<string> RecurringWithAllOpenParam()
            {
                return new List<string>()
                {
                    "RecOpen", "RecurringOpen"
                };
            }

            public static List<string> AutoCreateExtraSessionsWhenAClosedTeamParam()
            {
                return new List<string>()
                {
                    "AutoExtendClosed", "AutoClosed", "AutoCreateClosed",
                    "ExtClosed", "ExtendClosed", "CreateClosed"
                };
            }

            public static List<string> AutoCreateExtraSessionsWhenAllTeamsOpenParam()
            {
                return new List<string>()
                {
                    "AutoExtendOpen", "AutoOpen", "AutoCreateOpen",
                    "ExtOpen", "ExtendOpen", "CreateOpen"
                };
            }

            public static List<string> StartTimeParam()
            {
                return new List<string>()
                {
                    "StartTime", "Start", "S", "ST"
                };
            }

            public static List<string> HoursToOpenBeforeRegistrationParam()
            {
                return new List<string>()
                {
                    "Registration", "Reg", "R", "HoursToRegistrationBeforeStart"
                };
            }

            public static List<string> SessionDurationParam()
            {
                return new List<string>()
                {
                    "SessionDuration", "Duration", "SD"
                };
            }

            public static List<string> SessionExtraInfoParam()
            {
                return new List<string>()
                {
                    "ExtraInfo", "SessionInfo", "Info"
                };
            }

            public static List<string> SessionPasswordParam()
            {
                return new List<string>()
                {
                    "Password", "P", "PWD", "PW", "SessionPassword"
                };
            }

            public static List<string> UsePasswordParam()
            {
                return new List<string>()
                {
                    "UsePassword", "UP", "UsePWD", "UsePW",
                    "ShowPassword", "SP", "ShowPWD", "ShowPW"
                };
            }

            public static List<string> UseServerParam()
            {
                return new List<string>()
                {
                    "ShowServer", "SS", "UseServer"
                };
            }

            public static List<string> ShowPESSideSelectionInfoParam()
            {
                return new List<string>()
                {
                    "ShowPESSideSelection"
                };
            }

            public static List<string> AllowActiveSwapCommandInfoParam()
            {
                return new List<string>()
                {
                    "AllowActiveSwapCommand", "AllowActiveSwap", "AllowActiveSwitch", "AllowActiveSwitchCommand"
                };
            }

            public static List<string> AllowInactiveSwapCommandInfoParam()
            {
                return new List<string>()
                {
                    "AllowInactiveSwapCommand", "AllowInactiveSwap", "AllowInactiveSwitch", "AllowInactiveSwitchCommand"
                };
            }

            public static List<string> TimeZoneParam()
            {
                return new List<string>()
                {
                    "TZ", "TimeZone", "FusoHorário", "FusoHorário"
                };
            }

            public static List<string> LanguageParam()
            {
                return new List<string>()
                {
                    "Language", "Lang", "L", "Língua", "Lingua", "Idioma"
                };
            }

            public static List<string> TeamAParam()
            {
                return new List<string>()
                {
                    "TeamAName", "AName", "A", "TeamA", "AN", "NA",
                    "Team1Name", "1Name", "1", "Team1", "1N", "N1"
                };
            }

            public static List<string> TeamBParam()
            {
                return new List<string>()
                {
                    "TeamBName", "BName", "B", "TeamB", "BN", "NB",
                    "Team2Name", "2Name", "2", "Team2", "2N", "N2"
                };
            }

            public static List<string> TeamAIsOpenParam()
            {
                return new List<string>()
                {
                    "TeamAIsOpen", "AIsOpen", "AOpen", "TeamAOpen",
                    "Team1IsOpen", "1IsOpen", "1Open", "Team1Open",
                };
            }

            public static List<string> TeamBIsOpenParam()
            {
                return new List<string>()
                {
                    "TeamBIsOpen", "BIsOpen", "BOpen", "TeamBOpen",
                    "Team2IsOpen", "2IsOpen", "2Open", "Team2Open",
                };
            }
        }
    }
}
