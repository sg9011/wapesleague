using System.Collections.Generic;
using System.Text;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;

namespace WaPesLeague.Business.Dto.Platform
{
    public class PlatformWithUsersDto
    {
        public string PlatformId { get; set; }
        public string Name { get; set;  }
        public string Descritption { get; set; }

        public IReadOnlyCollection<string> PlatformUserIds { get; set; }

        public string ToDiscordString(GeneralMessages generalMessages)
        {
            var message = new StringBuilder();
             
            message.AppendLine(string.Format(generalMessages.PlatformName.GetValueForLanguage(), Name));
            message.AppendLine();
            message.AppendLine(string.Format(generalMessages.LinkedUsers.GetValueForLanguage(), PlatformUserIds.Count));

            foreach (var pu in PlatformUserIds)
            {
                message.AppendLine(pu);
            }

            return message.ToString();
        }
    }
}
