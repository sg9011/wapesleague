using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WaPesLeague.Business.Workflows.Interfaces;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;
using WaPesLeague.Data.Entities.Discord;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Business.Workflows
{
    public class ServerRoleWorkflow : BaseWorkflow<ServerRoleWorkflow>, IServerRoleWorkflow
    {
        private readonly IServerRoleManager _serverRoleManager;

        public ServerRoleWorkflow(IServerRoleManager serverRoleManager,
            IMemoryCache cache, IMapper mapper, ILogger<ServerRoleWorkflow> logger, ErrorMessages errorMessages, GeneralMessages generalMessages)
            : base(cache, mapper, logger, errorMessages, generalMessages)
        {
            _serverRoleManager = serverRoleManager;
        }

        public async Task<ServerRole> GetOrCreateServerRoleByDiscordRoleIdAndServerAsync(ulong discordRoleId, string roleName, int serverId)
        {
            var serverRole = await GetServerRoleByDiscordRoleAndServerIdFromCacheOrDbAsync(discordRoleId, serverId, roleName);
            if (serverRole == null)
            {
                serverRole = new ServerRole()
                {
                    Name = roleName,
                    DiscordRoleId = discordRoleId.ToString(),
                    ServerId = serverId
                };
                serverRole = await _serverRoleManager.AddAsync(serverRole);
                MemoryCache.Set(Cache.GetServerRoleByDiscordRoleIdAndServerId(discordRoleId, serverId).ToUpperInvariant(), serverRole, TimeSpan.FromHours(49));
            }

            return serverRole;
        }

        private async Task<ServerRole> GetServerRoleByDiscordRoleAndServerIdFromCacheOrDbAsync(ulong discordRoleId, int serverId, string roleName)
        {
            if (!MemoryCache.TryGetValue(Cache.GetServerRoleByDiscordRoleIdAndServerId(discordRoleId, serverId).ToUpperInvariant(), out ServerRole cacheEntry))
            {
                var serverRole = await _serverRoleManager.GetServerRoleByDiscordRoleIdAndServerIdAsync(discordRoleId.ToString(), serverId.ToString());
                if (serverRole != null)
                {
                    if (ServerRoleChanged(serverRole, roleName))
                    {
                        serverRole.Name = roleName;
                        serverRole = await _serverRoleManager.UpdateAsync(serverRole);
                    }
                    cacheEntry = serverRole;
                    MemoryCache.Set(Cache.GetServerRoleByDiscordRoleIdAndServerId(discordRoleId, serverId).ToUpperInvariant(), serverRole, TimeSpan.FromHours(49));
                }
            }

            return cacheEntry;
        }

        private static bool ServerRoleChanged(ServerRole serverRole, string roleName)
        {
            return !string.IsNullOrWhiteSpace(roleName) && !string.Equals(serverRole.Name, roleName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
