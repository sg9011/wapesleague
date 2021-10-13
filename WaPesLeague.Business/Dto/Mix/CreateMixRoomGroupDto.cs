using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace WaPesLeague.Business.Dto.Mix
{
    public class CreateMixRoomGroupDto
    {
        public ulong DiscordServerId { get; set; }
        public string DiscordServerName { get; set; }
        public ulong DiscordChannelId { get; set; }
        public string Name { get; set; }
        public decimal StartTime { get; set; }
        public decimal HoursToOpenRegistrationBeforeStart { get; set; }
        public bool Recurring { get; set; }
        public bool RecurringWithAllOpen { get; set; }
        public bool RecurringWith1OrMoreClosed { get; set; }
        public bool CreateExtraMixChannels { get; set; }
        public bool CreateExtraMixChannelsWithAllOpen { get; set; }
        public bool CreateExtraMixChannelsWith1OrMoreClosed { get; set; }
        public decimal MaxSessionDurationInHours { get; set; }
        public string ExtraInfo { get; set; }

        public string TeamAName { get; set; }
        public string TeamAFormation { get; set; }
        public List<string> TeamATags { get; set; }
        public bool AIsOpen { get; set; }

        public string TeamBName { get; set; }
        public string TeamBFormation { get; set; }
        public List<string> TeamBTags { get; set; }
        public bool BIsOpen { get; set; }

        public DiscordCommandPropsDto DiscordCommandPropsDto { get; set; }

        public CreateMixRoomGroupDto(DiscordCommandPropsDto propsDto, Data.Entities.Discord.Server serverDefaults)
        {
            DiscordServerId = propsDto.ServerId;
            DiscordServerName = propsDto.ServerName;
            DiscordChannelId = propsDto.ChannelId;

            StartTime = serverDefaults.DefaultStartTime;
            HoursToOpenRegistrationBeforeStart = serverDefaults.DefaultHoursToOpenRegistrationBeforeStart;
            MaxSessionDurationInHours = serverDefaults.DefaultSessionDuration;

            RecurringWith1OrMoreClosed = serverDefaults.DefaultSessionRecurringWithAClosedTeam;
            RecurringWithAllOpen = serverDefaults.DefaultSessionRecurringWithAllOpen;
            CreateExtraMixChannelsWith1OrMoreClosed = serverDefaults.DefaultAutoCreateExtraSessionsWithAClosedTeam;
            CreateExtraMixChannelsWithAllOpen = serverDefaults.DefaultAutoCreateExtraSessionsWhenAllTeamsOpen;

            ExtraInfo = serverDefaults.DefaultSessionExtraInfo;


            TeamAName = serverDefaults.DefaultTeams[0].Name;
            AIsOpen = serverDefaults.DefaultTeams[0].IsOpen;
            TeamATags = serverDefaults.DefaultTeams[0].Tags.Select(t => t.Tag).ToList();
            TeamAFormation = serverDefaults.ServerFormations.FirstOrDefault(sf => sf.IsDefault).Name;

            TeamBName = serverDefaults.DefaultTeams[1].Name;
            BIsOpen = serverDefaults.DefaultTeams[1].IsOpen;
            TeamBTags = serverDefaults.DefaultTeams[1].Tags.Select(t => t.Tag).ToList();
            TeamBFormation = serverDefaults.ServerFormations.FirstOrDefault(sf => sf.IsDefault).Name;

            DiscordCommandPropsDto = propsDto;
        }
    }
}
