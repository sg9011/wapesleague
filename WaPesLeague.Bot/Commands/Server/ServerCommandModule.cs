using AutoMapper;
using Base.Bot.Commands;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WaPesLeague.Bot.Commands.Base;
using WaPesLeague.Business.Dto;
using WaPesLeague.Business.Dto.Server;
using WaPesLeague.Business.Helpers;
using WaPesLeague.Business.Workflows.Interfaces;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;

namespace WaPesLeague.Bot.Commands.Server
{
    public class ServerCommandModule : BaseMixBotModule<ServerCommandModule>
    {
        private readonly IMapper _mapper;

        public ServerCommandModule(IServerWorkflow serverWorkflow, IMapper mapper, ILogger<ServerCommandModule> logger, ErrorMessages errorMessages, GeneralMessages generalMessages)
            : base(serverWorkflow, logger, errorMessages, generalMessages)
        {
            _mapper = mapper;
        }

        [Command("ServerTime"), Aliases("Time", "GetTime", "GetServerTime")]
        [Description("Get the server time")]
        public async Task GetServerTimeAsync(CommandContext ctx)
        {
            try
            {
                if (ctx.Guild == null)
                {
                    await ctx.RespondAsync(ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }

                var result = await _serverWorkflow.GetTimeAsync(ctx.Guild.Id, ctx.Guild.Name);

                await ReplyToRequest(ctx.Message, result.Message);
            }
            catch (Exception ex)
            {
                _ = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }

        }

        [Command("GetServer"), Aliases("DefaultsServer", "ServerDefaults")]
        [Description("Get the server settings")]
        public async Task GetServerSettings(CommandContext ctx)
        {
            try
            {
                if (ctx.Guild == null)
                {
                    await ctx.RespondAsync(ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }
                var server = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

                if (!await ValidateHasTopTierRolesAsync(ctx))
                    return;

                var dto = _mapper.Map<ServerDto>(server);

                await ctx.RespondAsync(dto.ToDiscordString(GeneralMessages));
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("UpdateServer"), Aliases("UpdateServerDefaults", "UpdateServerSettings")]
        [Description("Set the server settings")]
        public async Task UpdateServerDefaults(CommandContext ctx, [Description("A list of options that you want to change with their new value. --optionName:Value")][RemainingText] string stringOptions)
        {
            try
            {
                if (ctx.Guild == null)
                {
                    await ctx.RespondAsync(ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }
                var server = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

                if (!await ValidateHasTopTierRolesAsync(ctx))
                    return;
                if (string.IsNullOrWhiteSpace(stringOptions))
                {
                    await ctx.RespondAsync(ErrorMessages.NoOptionProvided.GetValueForLanguage());
                    return;
                }

                var discordCommandProperties = new DiscordCommandProperties(ctx);
                var updateServerSettingsDto = new UpdateServerSettingsDto(_mapper.Map<DiscordCommandPropsDto>(discordCommandProperties), server);

                try { updateServerSettingsDto.MapOptionsToDto(stringOptions, ErrorMessages);  ; }
                catch (Exception ex)
                {
                    await ctx.RespondAsync(string.Format(ErrorMessages.OptionMappingError.GetValueForLanguage(), ex.Message));
                    return;
                }

                var result = await _serverWorkflow.UpdateAsync(updateServerSettingsDto);
                if (result.Success && result.FollowUpParameters?.Server != null)
                {
                    var dto = _mapper.Map<ServerDto>(server);
                    server.SetDefaultThreadCurrentCulture();
                    await ctx.RespondAsync(dto.ToDiscordString(GeneralMessages));
                    return;
                }

                await ctx.RespondAsync(result.Message);
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("GetServerButtons"), Aliases("GetButtons", "GetBtns", "GetServerBtns")]
        [Description("Get the server buttons")]
        public async Task GetServerButtonsAsync(CommandContext ctx, [RemainingText] string textToIgnore = null)
        {
            try
            {
                if (ctx.Guild == null)
                {
                    await ctx.RespondAsync(ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }

                var server = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);
                if (!await ValidateHasTopTierRolesAsync(ctx))
                    return;

                var discordCommandProperties = new DiscordCommandProperties(ctx);
                ServerRequestQueue.Queue.Enqueue(new ServerRequestDto(ServerRequestType.GetServerButtons, discordCommandProperties, server));
            }
            catch(Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("AddServerButton"), Aliases("AddButton", "AddBtn")]
        [Description("Add a server button")]
        public async Task AddServerButtonAsync(CommandContext ctx, [RemainingText] string options = null)
        {
            try
            {
                if (ctx.Guild == null)
                {
                    await ctx.RespondAsync(ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }

                var server = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);
                if (!await ValidateHasTopTierRolesAsync(ctx))
                    return;

                var discordCommandProperties = new DiscordCommandProperties(ctx);
                ServerRequestQueue.Queue.Enqueue(new ServerRequestDto(ServerRequestType.GetServerButtons, discordCommandProperties, server)
                {
                    Options = options
                });
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("DeleteServerButton"), Aliases("DeleteButton", "DeleteBtn", "DelBtn", "RemoveButton", "RemoveBtn")]
        [Description("Delete a server button")]
        public async Task DeleteServerButtonAsync(CommandContext ctx, int buttonId)
        {
            try
            {
                if (ctx.Guild == null)
                {
                    await ctx.RespondAsync(ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }

                var server = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);
                if (!await ValidateHasTopTierRolesAsync(ctx))
                    return;

                var discordCommandProperties = new DiscordCommandProperties(ctx);
                ServerRequestQueue.Queue.Enqueue(new ServerRequestDto(ServerRequestType.DeleteButton, discordCommandProperties, server)
                {
                    ServerButtonId = buttonId
                });
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("GetTimeZones"), Aliases("TimeZones", "TimeZone", "ZonaDeHorario")]
        [Description("Get the timeZones as defined by microsoft")]
        public async Task GetTimeZones(CommandContext ctx, [Description("Get a list of existing timezones.")][RemainingText] string textToIgnore = null)
        {
            if (ctx.Guild != null)
                _ = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

            await ctx.RespondAsync(GeneralMessages.TimeZones.GetValueForLanguage());
        }
    }
}
