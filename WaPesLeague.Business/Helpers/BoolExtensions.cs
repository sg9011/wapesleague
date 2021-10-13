using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;

namespace WaPesLeague.Business.Helpers
{
    public static class BoolExtensions
    {
        public static string ToDiscordString(this bool boolValue, GeneralMessages generalMessages)
        {
            return boolValue
                ? generalMessages.Yes.GetValueForLanguage()
                : generalMessages.No.GetValueForLanguage();
        }
    }
}
