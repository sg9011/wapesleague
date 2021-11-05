﻿using System.Threading.Tasks;
using WaPesLeague.Business.Dto.Server;
using WaPesLeague.Business.Helpers;
using WaPesLeague.Data.Entities.Discord;

namespace WaPesLeague.Business.Workflows.Interfaces
{
    public interface IServerWorkflow
    {
        public Task<Server> GetOrCreateServerAsync(ulong discordServerId, string discordServerName);
        public Task<DiscordWorkflowResult> UpdateAsync(UpdateServerSettingsDto updateServerSettingsDto);
        public Task<DiscordWorkflowResult> GetTimeAsync(ulong discordServerId, string discordServerName);
        public Task HandleServerEventsAndActionsAsync();
    }
}
