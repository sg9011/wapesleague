using System;

namespace WaPesLeague.Data.Helpers
{
    public class Time
    {
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Seconds { get; set; }

        public Time()
        {
        }

        public Time(decimal decimalNumber)
        {
            var restToConverToMinutes = decimalNumber % 1;

            var scaledToMinutes = (restToConverToMinutes * 60);
            var restFromScalingToMinutes = scaledToMinutes % 1;
            var minutes = restFromScalingToMinutes > 0.5m
                ? scaledToMinutes + (1 - restFromScalingToMinutes)
                : scaledToMinutes - restFromScalingToMinutes;

            Hour = (int)(decimalNumber - restToConverToMinutes);
            Minute = (int)minutes;
            Seconds = 0;
        }

        public Time(DateTime date)
        {
            Hour = date.Hour;
            Minute = date.Minute;
            Seconds = date.Second;
        }

        public string ToDiscordString()
        {
            return $"{Hour:00}:{Minute:00}";
        }

        public string ToDiscordStringWithSeconds()
        {
            return $"{Hour:00}:{Minute:00}:{Seconds:00}";
        }

        public string ToDiscordChannelString()
        {
            return $"{Hour:00}{Minute:00}";
        }
    }
}
