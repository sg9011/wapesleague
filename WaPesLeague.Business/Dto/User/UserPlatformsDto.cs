using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Text;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;

namespace WaPesLeague.Business.Dto.User
{
    public class UserPlatformsDto
    {
        public int UserId { get; set; }
        public IReadOnlyCollection<PlatformUserDto> Platforms { get; set; }

        public string ToDiscordString(GeneralMessages generalMessages)
        {
            var message = new StringBuilder();
            message.AppendLine($"{generalMessages.UserPlatforms.GetValueForLanguage()}:");
            foreach (var p in Platforms)
            {
                message.AppendLine(p.ToDiscordString());
            }

            return message.ToString();
        }
    }
}
