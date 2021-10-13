using System.Collections.Generic;
using System.Linq;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;

namespace WaPesLeague.Business.Dto.Position
{
    public class PositionDto
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public IReadOnlyCollection<string> Tags { get; set; }
        public string ParentPositionCode { get; set; }
        public string PositionGroup { get; set; }
        public int PositionGroupOrder { get; set; }
        public int PositionOrder { get; set; }
        public bool IsRequiredForMix { get; set; }

        public string ToDiscordString(GeneralMessages generalMessages)
        {
            var parentPostionstring = string.IsNullOrWhiteSpace(ParentPositionCode)
                ? ""
                : $"\n\t{generalMessages.ParentPosition.GetValueForLanguage()}: {ParentPositionCode}";
            var tags = Tags?.Any() ?? false
                ? $"\t({string.Join(", ", Tags)})\n"
                : "\n";

            return $"{Code}: {Description}{tags}\t{generalMessages.Group.GetValueForLanguage()}: {PositionGroup}{parentPostionstring}";
        }
    }
}
