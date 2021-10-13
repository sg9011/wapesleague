using System.Collections.Generic;
using WaPesLeague.Business.Dto.Mix;
using WaPesLeague.Data.Helpers;
using WaPesLeague.Constants;
using System.Globalization;
using Microsoft.Extensions.Localization;
using WaPesLeague.Constants.Resources;

namespace WaPesLeague.Business.Helpers
{
    public static class MixGroupOptionsHelper
    {
        public static void MapOptionsToDto(this CreateMixRoomGroupDto dto, string optionsText, ErrorMessages errorMessages, GeneralMessages generalMessages)
        {
            var options = BaseOptionsHelper.SplitStringToOptions(optionsText);

            dto.TeamAName = options.GetValueForParams(MixChannelOptionsTags.TeamAParam()) ?? dto.TeamAName;
            dto.TeamBName = options.GetValueForParams(MixChannelOptionsTags.TeamBParam()) ?? dto.TeamBName;
            dto.TeamAFormation = options.GetValueForParams(MixChannelOptionsTags.TeamAFormationParam());
            dto.TeamBFormation = options.GetValueForParams(MixChannelOptionsTags.TeamBFormationParam());
            dto.AIsOpen = options.GetValueForParams(MixChannelOptionsTags.TeamAIsOpenParam()).OptionStringToBool(errorMessages, dto.AIsOpen);
            dto.BIsOpen = options.GetValueForParams(MixChannelOptionsTags.TeamBIsOpenParam()).OptionStringToBool(errorMessages, dto.BIsOpen);

            var bothTeamsAreOpen = dto.AIsOpen && dto.BIsOpen;

            dto.Recurring = options.GetValueForParams(MixChannelOptionsTags.RecurringParam()).OptionStringToBool(errorMessages, bothTeamsAreOpen ? dto.RecurringWithAllOpen : dto.RecurringWith1OrMoreClosed);
            dto.CreateExtraMixChannels = options.GetValueForParams(MixChannelOptionsTags.CreateExtraMixChannelsParam()).OptionStringToBool(errorMessages, bothTeamsAreOpen ? dto.CreateExtraMixChannelsWithAllOpen : dto.CreateExtraMixChannelsWith1OrMoreClosed);

            dto.StartTime = options.GetValueForParams(MixChannelOptionsTags.StartTimeParam()).OptionStringToDecimal(dto.StartTime, errorMessages);
            var defaultRegistrationTime = dto.StartTime < dto.HoursToOpenRegistrationBeforeStart ? dto.StartTime + (24 - dto.HoursToOpenRegistrationBeforeStart) : dto.StartTime - dto.HoursToOpenRegistrationBeforeStart;
            var registrationTime = options.GetValueForParams(MixChannelOptionsTags.RegistrationTimeParam()).OptionStringToDecimal(defaultRegistrationTime, errorMessages);
            
            dto.MaxSessionDurationInHours = options.GetValueForParams(MixChannelOptionsTags.MaxSessionDurationInHoursParam()).OptionStringToDecimal(dto.MaxSessionDurationInHours, errorMessages);
            var differenceBetweenStartAndRegistration = dto.StartTime - registrationTime;
            if (differenceBetweenStartAndRegistration < 0)
            {
                differenceBetweenStartAndRegistration = 24m - (registrationTime - dto.StartTime);
            }
            dto.HoursToOpenRegistrationBeforeStart = differenceBetweenStartAndRegistration;

            //Because of the shit implementation this assignment should happen last
            if (bothTeamsAreOpen)
            {
                dto.Name = options.GetValueForParams(MixChannelOptionsTags.NameParam()) ?? $"{new Time(dto.StartTime).ToDiscordChannelString()}-{generalMessages.Legend.GetValueForLanguage()}-{generalMessages.Mix.GetValueForLanguage()}";
            }
            else
            {
                var closedTeamName = !dto.AIsOpen
                    ? dto.TeamAName
                    : !dto.BIsOpen
                        ? dto.TeamBName
                        : "Unreachable";
                dto.Name = options.GetValueForParams(MixChannelOptionsTags.NameParam()) ?? $"{new Time(dto.StartTime).ToDiscordChannelString()}-{closedTeamName}-vs-{generalMessages.Mix.GetValueForLanguage()}";
            }
            dto.Name = $"{DiscordEmoji.Robot}{dto.Name}";
        }
    }

    public static class MixChannelOptionsTags
    {
        public static List<string> TeamAParam()
        {
            return new List<string>()
            {
                "TeamAName", "AName", "A", "TeamA", "AN", "NA", "ANome",
                "Team1Name", "1Name", "1", "Team1", "1N", "N1", "2Nome"
            };
        }

        public static List<string> TeamBParam()
        {
            return new List<string>()
            {
                "TeamBName", "BName", "B", "TeamB", "BN", "NB", "BNome",
                "Team2Name", "2Name", "2", "Team2", "2N", "N2", "2Nome"
            };
        }

        public static List<string> TeamAFormationParam()
        {
            return new List<string>()
            {
                "TeamAFormation", "AFormation", "FormationA", "FormA", "AForm", "AF", "FA", "AFormacao", "AFormação",
                "Team1Formation", "1Formation", "Formation1", "Form1", "1Form", "1F", "F1", "1Formacao", "1Formação"
            };
        }

        public static List<string> TeamBFormationParam()
        {
            return new List<string>()
            {
                "TeamBFormation", "BFormation", "FormationB", "FormB", "BForm", "FB", "BF", "BFormacao", "BFormação",
                "Team2Formation", "2Formation", "Formation2", "Form2", "2Form", "F2", "2F","2Formacao", "2Formação"
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

        public static List<string> StartTimeParam()
        {
            return new List<string>()
            {
                "Start", "StartOpen", "S", "Begin", "B", "StartTime", "BeginTime", "Início", "Inicio"
            };
        }

        public static List<string> RegistrationTimeParam()
        {
            return new List<string>()
            {
                "Registration", "RegistrationTime", "RegistrationOpen", "RegistrationOpenTime", "Reg", "Registo", "Inscrição", "Inscricao"
            };
        }

        public static List<string> RecurringParam()
        {
            return new List<string>()
            {
                "IsRecurring", "Recurring", "Repeat", "R", "Rec", "Recorrente"
            };
        }

        public static List<string> MaxSessionDurationInHoursParam()
        {
            return new List<string>()
            {
                "LifeSpan", "ls", "Duration", "SessionDuration", "MaxDuration", "MaxSessionDuration", "MaxSessionDurationInHours",
                "Duracao", "Duração"
            };
        }

        public static List<string> CreateExtraMixChannelsParam()
        {
            return new List<string>()
            {
                "Extend", "CreateExtraChannels", "CreateExtraChannel",
                "ExtraChannels", "ExtraChannel", "AutoExtend", "OpenExtra",
                "AutoExtensão", "Auto Extensao"
            };
        }
        public static List<string> NameParam()
        {
            return new List<string>()
            {
                "Name", "ChannelName", "Channel",
                "Nome do Canal", "Nome", "Canal"
            };
        }
    }
}
