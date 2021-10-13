namespace WaPesLeague.Business.Dto.Mix
{
    public class UserStatInfoLineDto
    {
        public int UserId { get; set; }
        public int PositionGroupId { get; set; }
        public string DbPositionGroupName { get; set; }

        public int PlayTimeSeconds { get; set; }
        public int ServerId { get; set; }
        public int MixSessionId { get; set; }

        public UserStatInfoLineDto(int userId, int positionGroupId, int serverId, int playTomeSeconds, int mixSessionId)
        {
            UserId = userId;
            PositionGroupId = positionGroupId;
            ServerId = serverId;
            PlayTimeSeconds = playTomeSeconds;
            MixSessionId = mixSessionId;
        }
    }
}
