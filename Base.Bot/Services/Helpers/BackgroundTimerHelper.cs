using System;

namespace Base.Bot.Services.Helpers
{
    public static class BackgroundTimerHelper
    {
        public static TimeSpan TimeSpanInMilliSecondsTillNextRun(int milliSecondsBetweenRuns, DateTime previousRun)
        {
            var dif = previousRun.AddMilliseconds(milliSecondsBetweenRuns).Subtract(DateTime.Now);
            return TimeSpan.FromMilliseconds(dif.TotalMilliseconds > 0 ? dif.TotalMilliseconds : 1);
        }
    }
}
