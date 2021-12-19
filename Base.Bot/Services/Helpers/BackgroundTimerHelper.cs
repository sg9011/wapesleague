using System;

namespace Base.Bot.Services.Helpers
{
    public static class BackgroundTimerHelper
    {
        public static TimeSpan TimeSpanInMilliSecondsTillNextRun(int milliSecondsBetweenRuns, DateTime previousRun)
        {
            var dif = previousRun.AddMilliseconds(milliSecondsBetweenRuns).Subtract(DateTime.UtcNow);
            return TimeSpan.FromMilliseconds(dif.TotalMilliseconds > 0 ? dif.TotalMilliseconds : 1);
        }

        public static TimeSpan CalculateStartingDelay(TimeSpan fixedTime)
        {
            var currentTime = DateTime.UtcNow.TimeOfDay;
            
            if (currentTime > fixedTime)
            {
                fixedTime = new TimeSpan(24 + fixedTime.Hours, 0 + fixedTime.Minutes, 0 + fixedTime.Minutes);
            }
            return fixedTime.Subtract(currentTime);
        }

    }
}
