namespace WaPesLeague.Constants
{
    public static class Cache
    {
        public const string AllSimplePlatforms = "AllPlatforms";
        public const string AllReservations = "AllReservations";
        public const string AllUserStatInfoLines = "AllUserStatInfoLines";
        public const string ActiveMixGroupsRoleOpenings = "ActiveMixGroupsRoleOpenings";

        private const string BasePlatformId = "{0}_PlatformId";
        private const string BaseServerId = "{0}_ServerId";
        private const string UserPlatform = "UserPlatform_{0}_{1}";
        private const string UserMember = "UserMember_{0}_{1}";
        private const string ServerRole = "ServerRole_{0}_{1}";

        public static string GetUserIdByPlatformKey(int platformId, string externalId)
            => string.Format(UserPlatform, platformId, externalId);

        public static string GetUserIdByUserAndServerKey(ulong userId, ulong serverId)
            => string.Format(UserMember, userId.ToString(), serverId.ToString());

        public static string GetServerRoleByDiscordRoleIdAndServerId(ulong discordRoleId, int serverId)
            => string.Format(ServerRole, discordRoleId.ToString(), serverId.ToString());

        public static string GetPlatformId(string platformName)
            => string.Format(BasePlatformId, platformName);

        public static string GetServerId(ulong discordServerId)
            => string.Format(BaseServerId, discordServerId.ToString());
    }
}
