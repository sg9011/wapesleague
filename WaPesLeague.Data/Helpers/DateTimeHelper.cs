using System;

namespace WaPesLeague.Data.Helpers
{
    public static class DateTimeHelper
    {
        public static TimeZoneInfo ApplicationTimeZoneInfo(string timeZoneName) => TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);
        public static DateTime GetNowForApplicationTimeZone(string timeZoneName)
        {
            return ConvertDateTimeToApplicationTimeZone(DateTime.UtcNow, timeZoneName);
        }

        public static DateTime GetDatabaseNow()
        {
            return DateTime.UtcNow;
        }

        public static DateTime ConvertDateTimeToApplicationTimeZone(DateTime date, string timeZoneName)
        {
            if (date.Kind == DateTimeKind.Unspecified)
            {
                date = new DateTime(date.Ticks, DateTimeKind.Utc);
            }
            return TimeZoneInfo.ConvertTime(date, ApplicationTimeZoneInfo(timeZoneName));
        }

        public static DateTime CreateDbDateTimeForDecimalTimeInApplicationZone(decimal timeDecimal, string timeZoneName)
        {
            var timeZoneInfo = ApplicationTimeZoneInfo(timeZoneName);
            var time = new Time(timeDecimal);
            var utcNow = GetDatabaseNow();
            var offset = timeZoneInfo.GetUtcOffset(utcNow);
            var dateWithOffset = utcNow.Add(offset);
            if (dateWithOffset.Day != utcNow.Day)
                dateWithOffset = dateWithOffset.AddDays(dateWithOffset.Day > utcNow.Day ? -1 : 1);
            var dateTime = new DateTime(dateWithOffset.Year, dateWithOffset.Month, dateWithOffset.Day, time.Hour, time.Minute, time.Seconds, DateTimeKind.Utc); //Get The Actual offset on this hour. (is it 100% correct no, but it will do for now)
            var offsetToUse = timeZoneInfo.GetUtcOffset(dateTime);
            return dateTime.Add(-offsetToUse);
        }
    }
}
