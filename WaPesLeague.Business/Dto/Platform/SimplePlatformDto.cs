using Microsoft.Extensions.Localization;
using System.Text;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;

namespace WaPesLeague.Business.Dto.Platform
{
    public class SimplePlatformDto
    {
        public int PlatformId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string ToDiscordString(GeneralMessages generalMessages, bool addEmptyTrailingLine = true)
        {

            var discordString = new StringBuilder();
            discordString.AppendLine(string.Format(generalMessages.PlatformName.GetValueForLanguage(), Name));
            discordString.AppendLine($"\t{generalMessages.Description.GetValueForLanguage()}: {Description}");
            discordString.AppendLine($"\t{generalMessages.Code.GetValueForLanguage()}: {Name}");
            if (addEmptyTrailingLine)
                discordString.AppendLine();

            return discordString.ToString();
        }
    }
}
