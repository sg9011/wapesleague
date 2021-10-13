

namespace WaPesLeague.Business.Dto.Mix
{
    public class RankUserStatItemDto
    {
        public int UserId { get; set; }
        public int MixSessions { get; set; }
        public int PlayTime { get; set; }
        public int Rank { get; set; }

        public RankUserStatItemDto(int userId, int mixSesisons, int playTime)
        {
            UserId = userId;
            MixSessions = mixSesisons;
            PlayTime = playTime;
        }
    }
}
