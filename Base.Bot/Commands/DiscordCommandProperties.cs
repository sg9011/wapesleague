using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace Base.Bot.Commands
{
    public class DiscordCommandProperties
    {
        public ulong RequestedByUserId { get; set; }
        public ulong UserId { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string Mention { get; set; }
        public ulong ServerId { get; set; }
        public string ServerName { get; set; }
        public ulong ChannelId { get; set; }
        public string ChannelName { get; set; }
        public ulong MessageId { get; set; }

        public DiscordCommandProperties(CommandContext ctx)
        {
            RequestedByUserId = ctx.User.Id;
            UserId = ctx.User.Id;
            UserName = ctx.User.Username;

            NickName = ctx.Member?.Nickname ?? ctx.Member?.Username ?? ctx.User.Username;
            Mention = ctx.Member?.Mention ?? ctx.User.Mention;

            ServerId = ctx.Guild?.Id ?? 0;
            ServerName = ctx.Guild?.Name ?? "Private Chat With the Bot";

            ChannelId = ctx.Channel?.Id ?? 0; ;
            ChannelName = ctx.Channel.Name ?? "Private Chat With the Bot";

            MessageId = ctx.Message.Id;
        }

        public DiscordCommandProperties(CommandContext ctx, DiscordMember discordMember)
        {
            RequestedByUserId = ctx.User.Id;
            UserId = discordMember.Id;
            UserName = discordMember.Username;

            NickName = discordMember?.Nickname ?? discordMember?.Username;
            Mention = discordMember?.Mention;

            ServerId = ctx.Guild.Id;
            ServerName = ctx.Guild.Name;

            ChannelId = ctx.Channel.Id;
            ChannelName = ctx.Channel.Name;

            MessageId = ctx.Message.Id;
        }
    }
}
