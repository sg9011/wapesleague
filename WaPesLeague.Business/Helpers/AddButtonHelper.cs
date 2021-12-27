using System.Collections.Generic;
using WaPesLeague.Business.Dto.Server;
using WaPesLeague.Constants.Resources;

namespace WaPesLeague.Business.Helpers
{
    public static class AddButtonHelper
    {
        public static void MapOptionsToDto(this AddServerButtonDto dto, string optionsText, ErrorMessages errorMessages)
        {
            var options = BaseOptionsHelper.SplitStringToOptions(optionsText, true);

            dto.Message = options.GetValueForParams(AddServerButtonTags.MessageParam());
            dto.URL = options.GetValueForParams(AddServerButtonTags.URLParam());
            if (dto.URL != null)
            {
                if (!dto.URL.StartsWith("http") && !dto.URL.StartsWith("discord"))
                {
                    dto.URL = $"https://{dto.URL}";
                }
            }

            dto.UseRateOverwrite = options.GetValueForParams(AddServerButtonTags.UseRateParam()).OptionStringPercentageToDecimal(null, errorMessages);
        }

        public static class AddServerButtonTags
        {
            public static List<string> MessageParam()
            {
                return new List<string>()
                {
                    "M", "MSG", "Message", "Text", "Bericht"
                };
            }

            public static List<string> URLParam()
            {
                return new List<string>()
                {
                    "URL", "Link", "Lien"
                };
            }

            public static List<string> UseRateParam()
            {
                return new List<string>()
                {
                    "UsageRate", "Rate", "Percentage", "Use", "%", "UseRate", "Usage"
                };
            }
        }
    }
}
