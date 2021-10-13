using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using System.Text;
using System;
using Base.Bot.Commands;
using WaPesLeague.Business.Workflows.Interfaces;
using WaPesLeague.Business.Dto.Platform;
using WaPesLeague.Constants;
using WaPesLeague.Bot.Commands.Base;
using Microsoft.Extensions.Logging;
using WaPesLeague.Constants.Resources;

namespace WaPesLeague.Bot.Commands.Platform
{
    public class PlatformCommandModule : BaseMixBotModule<PlatformCommandModule>
    {
        private readonly IPlatformWorkflow _platformWorkflow;
        public PlatformCommandModule(IPlatformWorkflow platformWorkflow, IServerWorkflow serverWorkflow, ILogger<PlatformCommandModule> logger, ErrorMessages errorMessages, GeneralMessages generalMessages)
            : base(serverWorkflow, logger, errorMessages, generalMessages)
        {
            _platformWorkflow = platformWorkflow;
        }

        [Command("GetPlatforms"), Aliases("MostrarPlataformas")]
        [Description("Get the available platforms")]
        public async Task GetPlatforms(CommandContext ctx)
        {
            try
            {
                if (ctx.Guild != null)
                    _ = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);
                var platforms = await _platformWorkflow.GetAllPlatformsAsync();
                var message = new StringBuilder();
                message.AppendLine(GeneralMessages.ListPlatforms.GetValueForLanguage());
                message.AppendLine();
                foreach (var p in platforms)
                {
                    message.AppendLine(p.ToDiscordString(GeneralMessages, true));
                }

                await ctx.RespondAsync(message.ToString());
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("GetPlatform"), Aliases("MostrarPlataforma")]
        [Description("Get the platform")]
        public async Task GetPlatform(CommandContext ctx, [Description("The platform you want to fetch.")][RemainingText] string platformCode = null)
        {
            try
            {
                if (ctx.Guild != null)
                    _ = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

                TriggerTypingFireAndForget(ctx);

                var name = platformCode?.Trim().Split(' ')[0];
                PlatformWithUsersDto platformdDto = null;
                if (!string.IsNullOrEmpty(name))
                {
                    var discordCommandProperties = new DiscordCommandProperties(ctx);
                    platformdDto = await _platformWorkflow.GetPlatformWithUsersAsync(name, discordCommandProperties.ServerId);
                }
                if (platformdDto == null)
                {
                    await ctx.RespondAsync(string.Format(ErrorMessages.NoPlatformFoundForCode.GetValueForLanguage(), name ?? " "));
                    await GetPlatforms(ctx);
                    return;
                }

                await ctx.RespondAsync(platformdDto.ToDiscordString(GeneralMessages));
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }
    }
}
