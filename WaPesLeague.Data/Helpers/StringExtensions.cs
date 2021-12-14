using System.Linq;

namespace WaPesLeague.Data.Helpers
{
    public static class StringExtensions
    {
        public static string OnlyKeepNumbers09(this string input)
        {
            return new string(input.Where(c => char.IsDigit(c)).ToArray());
        }
    }
}
