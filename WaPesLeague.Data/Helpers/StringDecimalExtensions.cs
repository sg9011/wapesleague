using System;

namespace WaPesLeague.Data.Helpers
{
    public static class StringDecimalExtensions
    {
        public static decimal ToRealDecimal(this string timeString)
        {
            if (timeString.Length == 2)
            {
                var value = decimal.Parse(timeString);
                if (value >= 24m)
                    throw new ArgumentOutOfRangeException($"this is an invalid time {timeString}, max time = 23h59m");
                return value;
            }
            decimal hour = 0m;
            var minutes = "0";
            if (timeString.Length == 3)
            {
                hour = decimal.Parse(timeString.Substring(0, 1));
                minutes = timeString.Substring(1, 2);

            }

            if (timeString.Length == 4)
            {
                hour = decimal.Parse(timeString.Substring(0, 2));
                minutes = timeString.Substring(2, 2);
            }
            var decimalMinutes = decimal.Parse(minutes);
            if (decimalMinutes >= 60)
            {
                throw new ArgumentOutOfRangeException($"this is an invalid time {timeString}, there cannot be 60 minutes or more in an hour");
            }

            return hour + (decimalMinutes / 60);
        }
    }
}
