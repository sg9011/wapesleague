using WaPesLeague.Data.Helpers;

namespace WaPesLeague.Business.Dto.Mix
{
    public class UserPositionGroupStatDto
    {
        public string PostionGroup { get; set; }
        public int Order { get; set; }
        public double MinutesPlayed { get; set; }
        public string ReadableTime => MinutesPlayed.DoubleMinutesToTime();
        public int ReservationCount { get; set; }
        public int Rank { get; set; }

        public UserPositionGroupStatDto(string positionGroup, int order, double minutesPlayed, int reservationCount = 0)
        {
            Order = order;
            PostionGroup = positionGroup;
            MinutesPlayed = minutesPlayed;
            ReservationCount = reservationCount;
        }

        public void AddSessionPlaytime(double minutesPlayed)
        {
            MinutesPlayed += minutesPlayed;
            ReservationCount++;
        }
    }
}
