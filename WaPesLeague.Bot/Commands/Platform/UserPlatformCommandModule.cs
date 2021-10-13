using AutoMapper;
using Base.Bot.Commands;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WaPesLeague.Bot.Commands.Base;
using WaPesLeague.Business.Dto;
using WaPesLeague.Business.Workflows.Interfaces;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;

namespace WaPesLeague.Bot.Commands.Platform
{
    public class UserPlatformCommandModule : BaseMixBotModule<UserPlatformCommandModule>
    {
        private readonly IUserWorkflow _userWorkflow;
        private readonly IMapper _mapper;


        public UserPlatformCommandModule(IUserWorkflow userWorkflow, IMapper mapper, IServerWorkflow serverWorkflow, ILogger<UserPlatformCommandModule> logger, ErrorMessages errorMessages, GeneralMessages generalMessages)
            : base(serverWorkflow, logger, errorMessages, generalMessages)
        {
            _userWorkflow = userWorkflow;
            _mapper = mapper;
        }

        /// <summary>
        /// Get the UserPlatforms this does not include discord
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [Command("GetMyPlatforms"), Aliases("GetPlatformIds", "MyPlatformIds", "GetMyPSN", "PSN", "PSNID", "PlataformaIDs", "MiPlataformaIDs", "CualEsMiPSN")]
        [Description("Get your ids within the platforms")]
        public async Task GetMyPlatforms(CommandContext ctx)
        {
            try
            {
                if (ctx.Guild != null)
                    _ = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

                var discordProperties = new DiscordCommandProperties(ctx);
                var userId = await _userWorkflow.GetOrCreateUserIdByDiscordId(_mapper.Map<DiscordCommandPropsDto>(discordProperties));
                var userPlatforms = await _userWorkflow.GetUserPlatformsByUserIdAsync(userId);

                await ctx.RespondAsync(userPlatforms.ToDiscordString(GeneralMessages));
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("SetMyPlatform"), Aliases("SetMyPlatformId", "SetMyPSNID", "SetMyPSN", "EstablecerMiPlataforma", "EstablecerMiPSNID", "EstablecerMiPSN")]
        [Description("Set your platform username")]
        public async Task SetMyPlatform(CommandContext ctx, [Description("The platform Name/Code.")] string platformName, [Description("Your username on the platform, leave it blank if you want to remove it.")] string userName = null)
        {
            try
            {
                if (ctx.Guild != null)
                    _ = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

                var discordProperties = new DiscordCommandProperties(ctx);
                var userId = await _userWorkflow.GetOrCreateUserIdByDiscordId(_mapper.Map<DiscordCommandPropsDto>(discordProperties));

                var message = await _userWorkflow.SetUserPlatformAsync(userId, platformName, userName);

                await ctx.RespondAsync(message);
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("SetUserPlatform"), Aliases("SetUserPlatformId", "SetUserPSNID", "SetUserPSN")]
        [Description("Set the platform username of someone else")]
        public async Task SetUserPlatform(CommandContext ctx, [Description("The DiscordMember you want to edit.")] DiscordMember member, [Description("The platform Name/Code.")] string platformName, [Description("Your username on the platform, leave it blank if you want to remove it.")] string userName = null)
        {
            try
            {
                if (ctx.Guild == null)
                {
                    await ctx.RespondAsync(ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }
                _ = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

                if (!await ValidateHasAdvancedRolesAsync(ctx))
                    return;

                var discordProperties = new DiscordCommandProperties(ctx, member);
                var userId = await _userWorkflow.GetOrCreateUserIdByDiscordId(_mapper.Map<DiscordCommandPropsDto>(discordProperties));

                var message = await _userWorkflow.SetUserPlatformAsync(userId, platformName, userName);

                await ctx.RespondAsync(message);
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }
    }
}
