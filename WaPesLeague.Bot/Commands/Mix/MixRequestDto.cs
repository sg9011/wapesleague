using Base.Bot.Commands;
using System.Collections.Generic;
using WaPesLeague.Business.Dto.Mix;

namespace WaPesLeague.Bot.Commands.Mix
{
    public class MixRequestDto //Poubelle
    {
        public MixRequestType RequestType { get; set; }
        public DiscordCommandProperties DiscordCommandProps { get; set; }
        public string Team { get; set; }
        public string Position { get; set; }
        public string ExtraInfo { get; set; }
        public string Password { get; set; }
        public string RoomName { get; set; }
        public string GameServer { get; set; }
        public int PlayerCount { get; set; }
        public string OldPosition { get; set; }
        public string NewPosition { get; set; }
        public ulong? RoleId { get; set; }
        public string RoleName { get; set; }
        public int? Minutes { get; set; }
        public List<string> RoleIdsPlayer1 { get; set; }
        public List<string> RoleIdsPlayer2 { get; set; }
        public List<string> ActorRoleIds { get; set; }
        public DiscordCommandProperties Player1 { get; set; }
        public DiscordCommandProperties Player2 { get; set; }
        public CreateMixRoomGroupDto CreateMixRoomGroupDto { get; set; } 

        public Data.Entities.Discord.Server Server { get; private set; }

        public MixRequestDto()
        {
            RoleIdsPlayer1 = new List<string>();
            RoleIdsPlayer2 = new List<string>();
            ActorRoleIds = new List<string>();
        }

        public MixRequestDto(MixRequestType requestType, DiscordCommandProperties discordCommandProps, Data.Entities.Discord.Server server) : this()
        {
            RequestType = requestType;
            DiscordCommandProps = discordCommandProps;
            Server = server;
        }

        public string ToLogString()
        {
            return "MixRequestDto: \n" +
                $"RequestType: {RequestType} \n" +
                $"UserId: {DiscordCommandProps?.UserId} ,ChannelId: {DiscordCommandProps?.ChannelId} \n" +
                $"Team: {Team ?? "null"} \n" +
                $"Position: {Position ?? "null"} \n" +
                $"ExtraInfo: {ExtraInfo ?? "null"} \n" +
                $"RoomName: {RoomName ?? "null"} \n" +
                $"GameServer: {GameServer ?? "null"} \n" +
                $"PlayerCount: {PlayerCount} \n" +
                $"OldPosition: {OldPosition ?? "null"} \n" +
                $"NewPosition: {NewPosition ?? "null"} \n" +
                $"RoleId: {RoleId?.ToString() ?? "null"} \n" +
                $"RoleName: {RoleName ?? "null"} \n" +
                $"Minutes: {Minutes?.ToString() ?? "null"} ";
        }
    }
}
