using System;

namespace WaPesLeague.Business.Dto.Mix
{
    public class MinimumPlayersRequiredTimeFrameDto
    {
        public int MinimumPlayersRequired { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public MinimumPlayersRequiredTimeFrameDto(int openTeams, DateTime start, DateTime end)
        {
            MinimumPlayersRequired = (openTeams * 6) - 1;
            DateStart = start;
            DateEnd = end;
        }
    }
}
