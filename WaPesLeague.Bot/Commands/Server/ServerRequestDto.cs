using Base.Bot.Commands;

namespace WaPesLeague.Bot.Commands.Server
{
    public class ServerRequestDto
    {
        public ServerRequestType RequestType { get; set; }
        public DiscordCommandProperties DiscordCommandProps { get; set; }
        public Data.Entities.Discord.Server Server { get; private set; }

        public string Options { get; set; }
        public int? ServerButtonId { get; set; }

        public ServerRequestDto(ServerRequestType requestType, DiscordCommandProperties discordCommandProps, Data.Entities.Discord.Server server)
        {
            RequestType = requestType;
            DiscordCommandProps = discordCommandProps;
            Server = server;
        }

        public string ToLogString()
        {
            return "ServerRequestDto: \n" +
                $"RequestType: {RequestType} \n" +
                $"UserId: {DiscordCommandProps?.UserId} ,ChannelId: {DiscordCommandProps?.ChannelId} \n" +
                $"ServerId: {Server?.ServerId.ToString() ?? "null"} \n" +
                $"Options: {Options ?? "null"} \n" +
                $"ServerButtonId: {ServerButtonId?.ToString() ?? "null"}";


        }
    }
}
