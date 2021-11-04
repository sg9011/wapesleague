using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Business.Dto.Mix;
using WaPesLeague.Business.Helpers;

namespace WaPesLeague.Business.Workflows.Interfaces
{
    public interface IMixSessionWorkflow
    {
        public Task<DiscordWorkflowResult> CreateSessionAsync(int serverId, ulong discordChannelId, DateTime? startDate = null, bool useExactStartDate = false);
        public Task<DiscordWorkflowResult> SignInAsync(SignInDto dto);
        public Task<DiscordWorkflowResult> SignOutAsync(int serverId, ulong discordChannelId, int userId);
        public Task<DiscordWorkflowResult> ShowAsync(int serverId, ulong discordChannelId);
        public Task<DiscordWorkflowResult> ShowSidesAsync(int serverId, ulong discordChannelId);

        public Task<DiscordWorkflowResult> SetPasswordAsync(int serverId, ulong discordChannelId, string password);
        public Task<DiscordWorkflowResult> SetServerAsync(int serverId, ulong discordChannelId, string serverName);
        public Task<DiscordWorkflowResult> SetRoomNameAsync(int serverId, ulong discordChannelId, string roomName);
        public Task<DiscordWorkflowResult> SetRoomOwnerAsync(int serverId, ulong discordChannelId, int userId);
        public Task<DiscordWorkflowResult> SetCaptainAsync(int serverId, ulong discordChannelId, int userId);
        public Task<DiscordWorkflowResult> SetLockedTeamPlayerCount(int serverId, ulong discordChannelId, int playerCount, string teamCode = null);
        public Task<DiscordWorkflowResult> OpenTeamAsync(int serverId, ulong discordChannelId, ulong? discordRoleId, string roleName, int? minutes);

        public Task<DiscordWorkflowResult> CleanRoomAsync(int serverId, ulong discordChannelId, ulong requestedBy);
        public Task<DiscordWorkflowResult> UpdatePositionAsync(ChangeMixSessionPositionDto dto);
        public Task<DiscordWorkflowResult> SwapAsync(int serverId, ulong discordChannelId, int user1Id, int user2Id, ulong requestedBy, List<string> roleIdsPlayer1, List<string> roleIdsPlayer2, List<string> actorRoleIds);
        public Task<bool> ValidateWithinValidHours(string discordServerId, string discordChannelId, List<string> roleIds);

        public Task HandleNotificationsOfMixSessionsAsync();
    }
}
