using System;

namespace WaPesLeague.Business.Dto
{
    public class DiscordCommandPropsDto
    {
        public ulong RequestedByUserId { get; set; }
        public ulong UserId { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public DateTime DiscordJoin { get; set; }
        public DateTime? ServerJoin { get; set; }
        public string Mention { get; set; }
        public ulong ServerId { get; set; }
        public string ServerName { get; set; }
        public ulong ChannelId { get; set; }
        public string ChannelName { get; set; }
        public ulong MessageId { get; set; }
    }
}
