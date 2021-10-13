using Base.Bot.Commands;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Business.Workflows.Interfaces;
using WaPesLeague.Constants.Resources;

namespace WaPesLeague.Bot.Commands.Base
{
    public abstract class BaseMixBotModule<T> : BaseBotModule<T> where T : BaseMixBotModule<T>
    {
        protected readonly IServerWorkflow _serverWorkflow;
        public BaseMixBotModule(IServerWorkflow serverWorkflow, ILogger<T> logger, ErrorMessages errorMessages, GeneralMessages generalMessages)
            : base(logger, errorMessages, generalMessages)
        {
            _serverWorkflow = serverWorkflow;
        }

        protected async Task<Data.Entities.Discord.Server> SetServerCulture(ulong discordServerId, string discordServerName)
        {
            var server = await _serverWorkflow.GetOrCreateServerAsync(discordServerId, discordServerName);
            server.SetDefaultThreadCurrentCulture();

            return server;
        }

        private static List<string> SecondTierRoles => new List<string>()
        {
            "MixBotCaptain",
            "Captains", "Captain", "Captains (Nations)", "Captains(Nations)"
        };

        private static List<string> TopTierRoles => new List<string>()
        {
            "MixBotOwner",
            "Moderators", "Moderator",
            "Admins", "Admin",
        };

        private static List<string> AdvancedRoles => TopTierRoles.Union(SecondTierRoles).ToList();

        protected async Task<bool> ValidateHasTopTierRolesAsync(CommandContext ctx)
        {
            return await ValidateRolesAsync(ctx, TopTierRoles);
        }

        protected async Task<bool> ValidateHasAdvancedRolesAsync(CommandContext ctx)
        {
            return await ValidateRolesAsync(ctx, AdvancedRoles);
        }
    }
}
