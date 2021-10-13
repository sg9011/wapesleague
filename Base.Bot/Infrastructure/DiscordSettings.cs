using System.Collections.Generic;

namespace Base.Bot.Infrastructure
{
    public class DiscordSettings
    {
        public string Token { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public List<string> CommandPrefixes { get; set; }
    }
}
