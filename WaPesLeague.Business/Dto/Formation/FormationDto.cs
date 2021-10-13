using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;

namespace WaPesLeague.Business.Dto.Formation
{
    public class FormationDto
    {
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public IReadOnlyCollection<string> Tags { get; set; }
        public IReadOnlyCollection<string> Positions { get; set; }

        public string ToDiscordString(GeneralMessages generalMessages)
        {
            var tags = Tags?.Any() ?? false
                ? $"\t({string.Join(", ", Tags)})\n"
                : "\n";
            var positions = Positions?.Any() ?? false
                ? $"\n\t{generalMessages.Positions.GetValueForLanguage()}: ({string.Join(", ", Positions)})"
                : "";
            return $"{Name}: {tags}\t{generalMessages.IsDefaultFormation.GetValueForLanguage()}: {IsDefault}{positions}";
        }

    }
}
