using System.Collections.Generic;

namespace WaPesLeague.Constants
{
    public static class Bot
    {
        public const string BotLine = "======== BOT MIX ========";
        public const string TeamLine = "--- {0} ---";
        public const string ChannelNameConnector = "-";
        public const string Left = ":arrow_left:";
        public const string Right = ":arrow_right:";
        public const string Prefix = ".";

        public static class SupportedLanguages
        {
            public const string English = "EN";
            public const string Portuguese = "PT";
            public const string Spannish = "ES";

            public static IReadOnlyCollection<string> GetAll()
            {
                return new List<string>()
                {
                    English,
                    Portuguese,
                    Spannish
                };
            }

            public static string ReadableListOfLanguages()
            {
                return $"{English}\n" +
                    $"{Portuguese}\n" +
                    $"{Spannish}";
            }
        }
    }
}
