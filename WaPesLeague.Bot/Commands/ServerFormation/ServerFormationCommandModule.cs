using AutoMapper;
using Base.Bot.Commands;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;
using WaPesLeague.Bot.Commands.Base;
using WaPesLeague.Business.Dto;
using WaPesLeague.Business.Dto.Formation;
using WaPesLeague.Business.Workflows.Interfaces;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;

namespace WaPesLeague.Bot.Commands.ServerFormation
{
    public class ServerFormationCommandModule : BaseMixBotModule<ServerFormationCommandModule>
    {
        private readonly IPositionWorkflow _positionWorkflow;
        private readonly IServerFormationWorkflow _serverFormationWorkflow;
        private readonly IMapper _mapper;

        public ServerFormationCommandModule(IPositionWorkflow positinoWorkflow, IServerFormationWorkflow serverFormationWorkflow, IMapper mapper
            , IServerWorkflow serverWorkflow, ILogger<ServerFormationCommandModule> logger, ErrorMessages errorMessages, GeneralMessages generalMessages)
            : base(serverWorkflow, logger, errorMessages, generalMessages)
        {
            _positionWorkflow = positinoWorkflow;
            _serverFormationWorkflow = serverFormationWorkflow;
            _mapper = mapper;
        }

        [Command("GetPositions"), Aliases("Positions", "Posicao", "Posição", "Posiciones")]
        [Description("Get a list of the known postions.")]
        public async Task GetPositions(CommandContext ctx)
        {
            try
            {
                Data.Entities.Discord.Server server = null;
                if (ctx.Guild != null)
                    server = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);
                TriggerTypingFireAndForget(ctx);

                var positions = await _positionWorkflow.GetAllPositionsOrderedAsync(server?.ServerId);
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(GeneralMessages.ListPositions.GetValueForLanguage());
                stringBuilder.AppendLine();
                foreach (var position in positions)
                {
                    stringBuilder.AppendLine(position.ToDiscordString(GeneralMessages));
                    stringBuilder.AppendLine();
                }
                var result = stringBuilder.ToString();
                var length = result.Length;
                await ctx.RespondAsync(result.Substring(0, Math.Min(length, 2000)));
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("AddPositionTag"), Aliases("AddPosTag")]
        [Description("Add position tag.")]
        public async Task AddPositionTag(CommandContext ctx, [Description("Position code.")] string positionCode, [Description("Tag to add.")] string tag, [RemainingText] string remainingText)
        {
            try
            {
                if (ctx.Guild == null)
                {
                    await ReplyToFailedRequest(ctx.Message, ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }
                var server = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

                if (!await ValidateHasTopTierRolesAsync(ctx))
                    return;

                if (string.Equals(positionCode, tag, StringComparison.InvariantCulture)) {
                    await ReplyToFailedRequest(ctx.Message, GeneralMessages.NothingChanged.GetValueForLanguage());
                    return;
                }

                TriggerTypingFireAndForget(ctx);

                var result = await _positionWorkflow.AddPositionTagAsync(positionCode, tag, server.ServerId);
                await ctx.RespondAsync(result.Message);
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("RemovePositionTag"), Aliases("RemovePosTag")]
        [Description("Remove position tag.")]
        public async Task RemovePositionTag(CommandContext ctx, [Description("Position tag.")] string tag, [RemainingText] string remainingText)
        {
            try
            {
                if (ctx.Guild == null)
                {
                    await ReplyToFailedRequest(ctx.Message, ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }
                var server = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

                if (!await ValidateHasTopTierRolesAsync(ctx))
                    return;

                TriggerTypingFireAndForget(ctx);

                var result = await _positionWorkflow.RemovePositionTagAsync(tag, server.ServerId);
                await ctx.RespondAsync(result.Message);
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("SetPositionDisplayTag"), Aliases("SetPosDisplayName", "SetPosDisplay")]
        [Description("Set position display name.")]
        public async Task SetPositionDisplay(CommandContext ctx, [Description("Position code.")] string positionCode, [Description("Display name.")] string displayValue, [RemainingText] string remainingText)
        {
            try
            {
                if (ctx.Guild == null)
                {
                    await ReplyToFailedRequest(ctx.Message, ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }
                var server = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

                if (!await ValidateHasTopTierRolesAsync(ctx))
                    return;

                TriggerTypingFireAndForget(ctx);

                var result = await _positionWorkflow.SetPositionDisplayAsync(positionCode, displayValue, server.ServerId);
                await ctx.RespondAsync(result.Message);
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }



        [Command("GetFormations"), Aliases("Formations", "Formação", "Formacao", "Alineación", "Alineacion")]
        [Description("Get a list of the known formations.")]
        public async Task Formations(CommandContext ctx)
        {
            try
            {
                if (ctx.Guild != null)
                    _ = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

                var props = new DiscordCommandProperties(ctx);
                var formations = await _serverFormationWorkflow.GetAllServerFormationsAsync(_mapper.Map<DiscordCommandPropsDto>(props));
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(GeneralMessages.ListFormations.GetValueForLanguage());
                stringBuilder.AppendLine();
                foreach (var formation in formations)
                {
                    stringBuilder.AppendLine(formation.ToDiscordString(GeneralMessages));
                    stringBuilder.AppendLine();
                }

                await ctx.RespondAsync(stringBuilder.ToString());
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("AddFormation")]
        [Description("Add a formation")]
        public async Task AddFormations(CommandContext ctx, [Description("The Name you want to give to this new formation.")] string formationName, [Description("A list of the positions within the formation, seperated by a whitespace.")][RemainingText] string positions)
        {
            try
            {
                if (ctx.Guild == null)
                {
                    await ReplyToFailedRequest(ctx.Message, ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }
                var server = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

                if (!await ValidateHasTopTierRolesAsync(ctx))
                    return;
                TriggerTypingFireAndForget(ctx);
                var props = new DiscordCommandProperties(ctx);

                var addFormationDto = new AddServerFormationDto(formationName, positions, props.ServerId, props.ServerName, server.ServerId);
                var result = await _serverFormationWorkflow.AddServerFormationAsync(addFormationDto);

                await ctx.RespondAsync(result.Message);
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("SetDefaultServerFormation"), Aliases("DefaultServerFormation")]
        [Description("Set Default Server Fortmation")]
        public async Task SetDefaultServerFormationAsync(CommandContext ctx, [Description("The Name of the formation that you want to be the default formation on the server.")] string formationName)
        {
            try
            {
                if (ctx.Guild == null)
                {
                    await ReplyToFailedRequest(ctx.Message, ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }
                _ = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);
                if (!await ValidateHasTopTierRolesAsync(ctx))
                    return;
                var props = new DiscordCommandProperties(ctx);

                var result = await _serverFormationWorkflow.SetDefaultServerFormationAsync(formationName, _mapper.Map<DiscordCommandPropsDto>(props));

                await ctx.RespondAsync(result.Message);
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }
    }
}
