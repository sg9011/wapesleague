using Base.Bot.Commands;
using WaPesLeague.Business.Dto.Mix;

namespace WaPesLeague.Bot.Commands.Mix
{
    public class MixRequestDto
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
        public DiscordCommandProperties Player1 { get; set; }
        public DiscordCommandProperties Player2 { get; set; }
        public CreateMixRoomGroupDto CreateMixRoomGroupDto { get; set; } 

        public Data.Entities.Discord.Server Server { get; private set; }

        public MixRequestDto(MixRequestType requestType, DiscordCommandProperties discordCommandProps, Data.Entities.Discord.Server server)
        {
            RequestType = requestType;
            DiscordCommandProps = discordCommandProps;
            Server = server;
        }

        public string ToLogString()
        {
            return "MixRequestDto: " +
                $"RequestType: {RequestType} " +
                $"UserId: {DiscordCommandProps?.UserId} ,ChannelId: {DiscordCommandProps?.ChannelId} " +
                $"Team: {Team ?? "null"} " +
                $"Position: {Position ?? "null"} " +
                $"ExtraInfo: {ExtraInfo ?? "null"} " +
                $"RoomName: {RoomName ?? "null"} " +
                $"GameServer: {GameServer ?? "null"} " +
                $"PlayerCount: {PlayerCount} " +
                $"OldPosition: {OldPosition ?? "null"} " +
                $"NewPosition: {NewPosition ?? "null"}";
        }
    }
}
