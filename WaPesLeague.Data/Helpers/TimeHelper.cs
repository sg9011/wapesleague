namespace WaPesLeague.Data.Helpers
{
    public static class TimeHelper
    {
        public static string DoubleMinutesToTime(this double minutes)
        {
            minutes -= minutes % 1; //4000
            var timeMinutes = minutes % 60; //40
            var restMinutes = minutes - timeMinutes; //3960
            var restHours = (restMinutes % (60 * 24)) / 60; //this are the hours
            var days = (restMinutes - (restHours * 60)) / (60 * 24);
            var daysString = days > 0
                ? $"{days}d - "
                : "";
            return $"{daysString}{restHours}h - {timeMinutes}m";
        }
    }
}
