using Base.Bot.Commands;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Business.Workflows.Interfaces;
using WaPesLeague.Data.Helpers;
using WaPesLeague.Business.Helpers;
using DSharpPlus;
using WaPesLeague.Bot.Commands.Base;
using Microsoft.Extensions.Logging;
using WaPesLeague.Constants.Resources;
using WaPesLeague.Constants;

namespace WaPesLeague.Bot.Commands.Help
{
    public class HelpCommandModule : BaseMixBotModule<HelpCommandModule>
    {
        public HelpCommandModule(IServerWorkflow serverWorkflow, ILogger<HelpCommandModule> logger, ErrorMessages errorMessages, GeneralMessages generalMessages)
            : base(serverWorkflow, logger, errorMessages, generalMessages)
        {
        }

        [Command("XXXHide"), Hidden]
        [Description("Hide channel")]
        public async Task HideChannelAsync(CommandContext ctx)
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

                var allPermissions = ctx.Channel.PermissionOverwrites.ToList();
                foreach(var rolePermissions in allPermissions.Where(r => r.Type == OverwriteType.Role))
                {
                    if (rolePermissions.Allowed.HasPermission(Permissions.AccessChannels))
                    {
                        var roleAccessPermissions = rolePermissions.Allowed.Revoke(Permissions.AccessChannels);
                        var roleDeniedPermissions = rolePermissions.Denied.Grant(Permissions.AccessChannels);
                        await ctx.Channel.AddOverwriteAsync(await rolePermissions.GetRoleAsync(), deny: roleDeniedPermissions, allow: roleAccessPermissions);
                    }
                }
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("XXXUnHide"), Hidden]
        [Description("UnHide channel")]
        public async Task UnHideChannelAsync(CommandContext ctx)
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

                var allPermissions = ctx.Channel.PermissionOverwrites.ToList();
                foreach (var rolePermissions in allPermissions.Where(r => r.Type == OverwriteType.Role))
                {
                    if (rolePermissions.Denied.HasPermission(Permissions.AccessChannels))
                    {
                        var roleAccessPermissions = rolePermissions.Allowed.Grant(Permissions.AccessChannels);
                        var roleDeniedPermissions = rolePermissions.Denied.Revoke(Permissions.AccessChannels);
                        await ctx.Channel.AddOverwriteAsync(await rolePermissions.GetRoleAsync(), deny: roleDeniedPermissions, allow: roleAccessPermissions);
                    }
                }

                await ctx.RespondAsync("Channel is shown again");
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("MixBot"), Aliases("MixBotHelp")]
        [Description("Get List of commands and a little description.")]
        public async Task HelpMixBot(CommandContext ctx)
        {
            try
            {
                if (ctx.Guild != null)
                    _ = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

                if (!await ValidateHasAdvancedRolesAsync(ctx))
                    return;

                var props = new DiscordCommandProperties(ctx);
                var server = await _serverWorkflow.GetOrCreateServerAsync(props.ServerId, props.ServerName);
                var embed = new DiscordEmbedBuilder()
                {
                    Color = new DiscordColor("#FF0000"),
                    Title = GeneralMessages.MixBotHelpTitle.GetValueForLanguage(),
                    Description = string.Format(GeneralMessages.MixBotHelpDescription.GetValueForLanguage(), Constants.Bot.Prefix),
                    Timestamp = DateTimeHelper.GetDatabaseNow()

                };
                embed.AddField(GeneralMessages.SignInTitle.GetValueForLanguage(), string.Format(GeneralMessages.SignInDescription.GetValueForLanguage(), Constants.Bot.Prefix));
                embed.AddField(GeneralMessages.SignUserInTitle.GetValueForLanguage(), string.Format(GeneralMessages.SignUserInDescription.GetValueForLanguage(), Constants.Bot.Prefix));
                embed.AddField(GeneralMessages.SignOutTitle.GetValueForLanguage(), string.Format(GeneralMessages.SignOutDescription.GetValueForLanguage(), Constants.Bot.Prefix));
                embed.AddField(GeneralMessages.SignUserOutDescription.GetValueForLanguage(), string.Format(GeneralMessages.SignUserOutDescription.GetValueForLanguage(), Constants.Bot.Prefix));

                embed.AddField(GeneralMessages.ShowTitle.GetValueForLanguage(), string.Format(GeneralMessages.ShowDescription.GetValueForLanguage(), Constants.Bot.Prefix));
                embed.AddField(GeneralMessages.CleanTitle.GetValueForLanguage(), string.Format(GeneralMessages.CleanDescription.GetValueForLanguage(), Constants.Bot.Prefix));

                embed.AddField(GeneralMessages.SetPasswordTitle.GetValueForLanguage(), string.Format(GeneralMessages.SetPasswordDescription.GetValueForLanguage(), Constants.Bot.Prefix));
                embed.AddField(GeneralMessages.SetServerTitle.GetValueForLanguage(), string.Format(GeneralMessages.SetServerDescription.GetValueForLanguage(), Constants.Bot.Prefix));
                embed.AddField(GeneralMessages.SetRoomTitle.GetValueForLanguage(), string.Format(GeneralMessages.SetRoomDescription.GetValueForLanguage(), Constants.Bot.Prefix));
                embed.AddField(GeneralMessages.SetRoomOwnerTitle.GetValueForLanguage(), string.Format(GeneralMessages.SetRoomOwnerDescription.GetValueForLanguage(), Constants.Bot.Prefix));
                embed.AddField(GeneralMessages.UpdatePositionTitle.GetValueForLanguage(), string.Format(GeneralMessages.UpdatePositionDescription.GetValueForLanguage(), Constants.Bot.Prefix));

                embed.AddField(GeneralMessages.MyStatsTitle.GetValueForLanguage(), string.Format(GeneralMessages.MyStatsDescription.GetValueForLanguage(), Constants.Bot.Prefix));

                embed.AddField(GeneralMessages.GetPlatformsTitle.GetValueForLanguage(), string.Format(GeneralMessages.GetPlatformsDescription.GetValueForLanguage(), Constants.Bot.Prefix));
                embed.AddField(GeneralMessages.GetPlatformTitle.GetValueForLanguage(), string.Format(GeneralMessages.GetPlatformDescription.GetValueForLanguage(), Constants.Bot.Prefix));
                
                embed.AddField(GeneralMessages.GetMyPlatformsTitle.GetValueForLanguage(), string.Format(GeneralMessages.GetMyPlatformsDescription.GetValueForLanguage(), Constants.Bot.Prefix));
                embed.AddField(GeneralMessages.SetMyPlatformTitle.GetValueForLanguage(), string.Format(GeneralMessages.SetMyPlatformDescription.GetValueForLanguage(), Constants.Bot.Prefix));
                embed.AddField(GeneralMessages.SetUserPlatformTitle.GetValueForLanguage(), string.Format(GeneralMessages.SetUserPlatformDescription.GetValueForLanguage(), Constants.Bot.Prefix));
                
                embed.AddField(GeneralMessages.GetPositionsTitle.GetValueForLanguage(), string.Format(GeneralMessages.GetPositionsDescription.GetValueForLanguage(), Constants.Bot.Prefix));
                embed.AddField(GeneralMessages.GetFormationsTitle.GetValueForLanguage(), string.Format(GeneralMessages.GetFormationsDescription.GetValueForLanguage(), Constants.Bot.Prefix));
                embed.AddField(GeneralMessages.AddFormationTitle.GetValueForLanguage(), string.Format(GeneralMessages.AddFormationDescription.GetValueForLanguage(), Constants.Bot.Prefix));
                embed.AddField(GeneralMessages.SetDefaultServerFormationTitle.GetValueForLanguage(), string.Format(GeneralMessages.SetDefaultServerFormationDescription.GetValueForLanguage(), Constants.Bot.Prefix));
                
                embed.AddField(GeneralMessages.GetServerTitle.GetValueForLanguage(), string.Format(GeneralMessages.GetServerDescription.GetValueForLanguage(), Constants.Bot.Prefix));

                await ctx.RespondAsync(embed.Build());
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("CreateMixHelp"), Aliases("CreateHelp")]
        [Description("Get Create Mix bot help.")]
        public async Task HelpCreateMix(CommandContext ctx)
        {
            try
            {
                if (ctx.Guild == null)
                {
                    await ReplyToFailedRequest(ctx.Message, ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }
                _ = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

                if (!await ValidateHasAdvancedRolesAsync(ctx))
                    return;

                var roles = ctx.Member.Roles.Select(r => r.Name);
                var props = new DiscordCommandProperties(ctx);
                var server = await _serverWorkflow.GetOrCreateServerAsync(props.ServerId, props.ServerName);
                var embed = new DiscordEmbedBuilder()
                {
                    Color = new DiscordColor("#FF0000"),
                    Title = GeneralMessages.Title.GetValueForLanguage(),
                    Description = GeneralMessages.CreateMixHelpDescription.GetValueForLanguage(),
                    Timestamp = DateTimeHelper.GetDatabaseNow()
                };
                var teamA = server.DefaultTeams.Single(x => x.Tags.Any(t => string.Equals(t.Tag, "A", StringComparison.InvariantCultureIgnoreCase)));
                var teamB = server.DefaultTeams.Single(x => x.Tags.Any(t => string.Equals(t.Tag, "B", StringComparison.InvariantCultureIgnoreCase)));
                var defaultFormation = server.ServerFormations.Single(f => f.IsDefault == true).Name;

                embed.AddField(GeneralMessages.DeleteMixTitle.GetValueForLanguage(), string.Format(GeneralMessages.DeleteMixDescription.GetValueForLanguage(), Constants.Bot.Prefix));
                embed.AddField(GeneralMessages.CreateMixTitle.GetValueForLanguage(), string.Format(GeneralMessages.CreateMixDescription.GetValueForLanguage(), Constants.Bot.Prefix));
                embed.AddField(GeneralMessages.ExampleTitle.GetValueForLanguage(), string.Format(GeneralMessages.ExampleDescription.GetValueForLanguage(), Constants.Bot.Prefix));

                embed.AddField(GeneralMessages.StartTimeTitle.GetValueForLanguage(), string.Format(GeneralMessages.StartTimeDescription.GetValueForLanguage(), new Time(server.DefaultStartTime).ToDiscordString()));
                embed.AddField(GeneralMessages.RegistrationTitle.GetValueForLanguage(), string.Format(GeneralMessages.StartTimeDescription.GetValueForLanguage(), new Time(server.DefaultHoursToOpenRegistrationBeforeStart).ToDiscordString()));
                embed.AddField(GeneralMessages.MaxSessionDurationTitle.GetValueForLanguage(), string.Format(GeneralMessages.MaxSessionDurationDescription.GetValueForLanguage(), new Time(server.DefaultSessionDuration).ToDiscordString()));
                embed.AddField(GeneralMessages.IsRecurringTitle.GetValueForLanguage(), string.Format(GeneralMessages.MaxSessionDurationDescription.GetValueForLanguage(), server.DefaultSessionRecurringWithAllOpen, server.DefaultSessionRecurringWithAClosedTeam));
                embed.AddField(GeneralMessages.CreateExtraMixChannelsTitle.GetValueForLanguage(), string.Format(GeneralMessages.CreateExtraMixChannelsDescription.GetValueForLanguage(), server.DefaultAutoCreateExtraSessionsWhenAllTeamsOpen, server.DefaultAutoCreateExtraSessionsWithAClosedTeam));
                embed.AddField(GeneralMessages.ChannelNameTitle.GetValueForLanguage(), GeneralMessages.ChannelNameDescription.GetValueForLanguage());
                embed.AddField(GeneralMessages.TeamANameTitle.GetValueForLanguage(), string.Format(GeneralMessages.TeamANameDescription.GetValueForLanguage(), teamA.Name));
                embed.AddField(GeneralMessages.TeamBNameTitle.GetValueForLanguage(), string.Format(GeneralMessages.TeamBNameDescription.GetValueForLanguage(), teamB.Name));
                embed.AddField(GeneralMessages.TeamAFormationTitle.GetValueForLanguage(), string.Format(GeneralMessages.TeamAFormationDescription.GetValueForLanguage(), Constants.Bot.Prefix, defaultFormation));
                embed.AddField(GeneralMessages.TeamBFormationTitle.GetValueForLanguage(), string.Format(GeneralMessages.TeamBFormationDescription.GetValueForLanguage(), Constants.Bot.Prefix, defaultFormation));
                embed.AddField(GeneralMessages.TeamAIsOpenTitle.GetValueForLanguage(), string.Format(GeneralMessages.TeamAIsOpenDescription.GetValueForLanguage(),  teamA.IsOpen.ToDiscordString(GeneralMessages)));
                embed.AddField(GeneralMessages.TeamBIsOpenTitle.GetValueForLanguage(), string.Format(GeneralMessages.TeamBIsOpenDescription.GetValueForLanguage(), teamB.IsOpen.ToDiscordString(GeneralMessages)));

                await ctx.RespondAsync(embed.Build());
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("UpdateServerHelp"), Aliases("ServerHelp", "UpdateServerDefaultsHelp")]
        [Description("Get Help to update the server settings.")]
        public async Task UpdateServerAsync(CommandContext ctx)
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

                var roles = ctx.Member.Roles.Select(r => r.Name);
                var props = new DiscordCommandProperties(ctx);
                var server = await _serverWorkflow.GetOrCreateServerAsync(props.ServerId, props.ServerName);
                var embed = new DiscordEmbedBuilder()
                {
                    Color = new DiscordColor("#FF0000"),
                    Title = GeneralMessages.UpdateServerHelpTitle.GetValueForLanguage(),
                    Description = GeneralMessages.UpdateServerHelpDescription.GetValueForLanguage(),
                    Timestamp = DateTimeHelper.GetDatabaseNow()
                };

                embed.AddField(GeneralMessages.UpdateServerTitle.GetValueForLanguage(), string.Format(GeneralMessages.UpdateServerDescription.GetValueForLanguage(), Constants.Bot.Prefix));
                embed.AddField(GeneralMessages.ExampleTitle.GetValueForLanguage(), string.Format(GeneralMessages.UpdateServerExampleDescription.GetValueForLanguage(), Constants.Bot.Prefix));
                embed.AddField($"\t{GeneralMessages.RecurringClosedTitle.GetValueForLanguage()}", GeneralMessages.RecurringClosedDescription.GetValueForLanguage());
                embed.AddField($"\t{GeneralMessages.RecurringOpenTitle.GetValueForLanguage()}", GeneralMessages.RecurringOpenDescription.GetValueForLanguage());
                              
                embed.AddField($"\t{GeneralMessages.AutoExtendClosedTitle.GetValueForLanguage()}", GeneralMessages.AutoExtendClosedDescription.GetValueForLanguage());
                embed.AddField($"\t{GeneralMessages.AutoExtendOpenTitle.GetValueForLanguage()}", GeneralMessages.AutoExtendOpenDescription.GetValueForLanguage());
                embed.AddField($"\t{GeneralMessages.StartTimeTitle.GetValueForLanguage()}", GeneralMessages.UpdateServerStartTimeDescription.GetValueForLanguage());
                embed.AddField($"\t{GeneralMessages.RegistrationTitle.GetValueForLanguage()}", GeneralMessages.RegistrationTimeDescription.GetValueForLanguage());
                embed.AddField($"\t{GeneralMessages.MaxSessionDurationTitle.GetValueForLanguage()}", GeneralMessages.UpdateServerMaxSessionDurationDescription.GetValueForLanguage());
                               
                embed.AddField($"\t{GeneralMessages.ExtraInfoTitle.GetValueForLanguage()}", GeneralMessages.ExtraInfoDescription.GetValueForLanguage());
                embed.AddField($"\t{GeneralMessages.ShowPasswordTitle.GetValueForLanguage()}", GeneralMessages.ShowPasswordDescription.GetValueForLanguage());
                embed.AddField($"\t{GeneralMessages.PasswordTitle.GetValueForLanguage()}", GeneralMessages.PasswordDescription.GetValueForLanguage());
                embed.AddField($"\t{GeneralMessages.ShowServerTitle.GetValueForLanguage()}", GeneralMessages.ShowServerDescription.GetValueForLanguage());
                embed.AddField($"\t{GeneralMessages.ShowPESSideSelectionTitle.GetValueForLanguage()}", GeneralMessages.ShowPESSideSelectionDescription.GetValueForLanguage());
                embed.AddField($"\t{GeneralMessages.AllowActiveSwapCommandTitle.GetValueForLanguage()}", GeneralMessages.AllowActiveSwapCommandDescription.GetValueForLanguage());
                embed.AddField($"\t{GeneralMessages.AllowInactiveSwapCommandTitle.GetValueForLanguage()}", GeneralMessages.AllowInactiveSwapCommandDescription.GetValueForLanguage());
                               
                embed.AddField($"\t{GeneralMessages.TimeZoneTitle.GetValueForLanguage()}", string.Format(GeneralMessages.TimeZoneDescription.GetValueForLanguage(), Constants.Bot.Prefix));
                              
                embed.AddField($"\t{GeneralMessages.TeamANameTitle.GetValueForLanguage()}", GeneralMessages.UpdateServerTeamANameDescription.GetValueForLanguage());
                embed.AddField($"\t{GeneralMessages.TeamAOpenTitle.GetValueForLanguage()}", GeneralMessages.TeamAOpenDescription.GetValueForLanguage());
                               
                embed.AddField($"\t{GeneralMessages.TeamBNameTitle.GetValueForLanguage()}", GeneralMessages.UpdateServerTeamBNameDescription.GetValueForLanguage());
                embed.AddField($"\t{GeneralMessages.TeamBOpenTitle.GetValueForLanguage()}", GeneralMessages.TeamBOpenDescription.GetValueForLanguage());


                await ctx.RespondAsync(embed.Build());
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("Boobs"), Aliases("Titties"), Hidden]
        [Description("Silly stuff.")]
        public async Task BoobsAsync(CommandContext ctx)
        {
            try
            {
                if (ctx.Guild == null)
                {
                    await ctx.RespondAsync($"https://www.youtube.com/watch?v=W3sId1F9I6Y");
                    return;
                }

                await ctx.RespondAsync("Maybe we could talk somewhere more private. :kissing_closed_eyes:");
            }
            catch(Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }
    }
}
