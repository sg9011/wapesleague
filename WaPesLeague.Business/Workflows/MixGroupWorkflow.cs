using AutoMapper;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Business.Dto;
using WaPesLeague.Business.Dto.Mix;
using WaPesLeague.Business.Helpers;
using WaPesLeague.Business.Workflows.Interfaces;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;
using WaPesLeague.Data.Entities.Mix;
using WaPesLeague.Data.Helpers;
using WaPesLeague.Data.Managers.Interfaces;
using Bot = Base.Bot.Bot;

namespace WaPesLeague.Business.Workflows
{
    public class MixGroupWorkflow : BaseWorkflow<MixGroupWorkflow>, IMixGroupWorkflow
    {
        private readonly IMixChannelManager _mixChannelManager;
        private readonly IServerFormationManager _serverFormationManager;
        private readonly IMixGroupManager _mixGroupManager;
        private readonly IMixGroupRoleOpeningManager _mixGroupRoleOpeningManager;
        private readonly IMixChannelWorkflow _mixChannelWorkflow;
        private readonly IMixSessionWorkflow _mixSessionWorkflow;
        private readonly IServerRoleWorkflow _serverRoleWorkflow;
        private readonly IServerWorkflow _serverWorkflow;
        private readonly IMixSessionManager _mixSessionManager;
        private readonly IUserMemberManager _userMemberManager;
        private readonly IServerManager _serverManager;

        private readonly Bot _bot;

        public MixGroupWorkflow(IMixChannelManager mixChannelManager, IServerFormationManager serverFormationManager, IMixGroupManager mixGroupManager, IMixGroupRoleOpeningManager mixGroupRoleOpeningManager,
            IMixChannelWorkflow mixChannelWorkflow, IMixSessionWorkflow mixSessionWorkflow, IServerWorkflow serverWorkflow, IServerRoleWorkflow serverRoleWorkflow, IMixSessionManager mixSessionManager, Bot bot,
            IUserMemberManager userMemberManager, IServerManager serverManager,
            IMemoryCache cache, IMapper mapper, ILogger<MixGroupWorkflow> logger, ErrorMessages errorMessages, GeneralMessages generalMessages)
            : base (cache, mapper, logger, errorMessages, generalMessages)
        {
            _mixChannelManager = mixChannelManager;
            _serverFormationManager = serverFormationManager;
            _mixGroupManager = mixGroupManager;
            _mixGroupRoleOpeningManager = mixGroupRoleOpeningManager;
            _mixChannelWorkflow = mixChannelWorkflow;
            _mixSessionWorkflow = mixSessionWorkflow;
            _serverRoleWorkflow = serverRoleWorkflow;
            _serverWorkflow = serverWorkflow;
            _mixSessionManager = mixSessionManager;
            _userMemberManager = userMemberManager;
            _serverManager = serverManager;
            _bot = bot;
        }
        public async Task<DiscordWorkflowResult> CreateMixGroupAsync(CreateMixRoomGroupDto dto)
        {
            var validator = new CreateMixRoomDtoValidator(_mixChannelManager, _serverFormationManager, ErrorMessages, GeneralMessages);
            var validationResults = await validator.ValidateAsync(dto);
            if (validationResults.Errors.Any())
                return HandleValidationResults(validationResults);

            var server = await _serverWorkflow.GetOrCreateServerAsync(dto.DiscordServerId, dto.DiscordServerName);

            var mixGroup = new MixGroup()
            {
                ServerId = server.ServerId,
                BaseChannelName = dto.Name,
                Recurring = dto.Recurring,
                CreateExtraMixChannels = dto.CreateExtraMixChannels,
                ExtraInfo = dto.ExtraInfo,
                Start = dto.StartTime,
                HoursToOpenRegistrationBeforeStart = dto.HoursToOpenRegistrationBeforeStart,
                MaxSessionDurationInHours = dto.MaxSessionDurationInHours,
                IsActive = true
            };

            var createdMixGroup = await _mixGroupManager.AddAsync(mixGroup);
            return await _mixChannelWorkflow.CreateChannelForMixGroupAsync(createdMixGroup, dto);
        }

        public async Task<DiscordWorkflowResult> DeActivateMixGroupAsync(DiscordCommandPropsDto props)
        {
            var mixChannel = await _mixChannelManager.GetActiveMixChannelByDiscordIds(props.ServerId.ToString(), props.ChannelId.ToString());
            if (mixChannel == null)
                return new DiscordWorkflowResult(ErrorMessages.NothingToClose.GetValueForLanguage());
            await _mixGroupManager.DeActivateMixGroupAsync(mixChannel.MixGroupId);
            var mixGroupDeactivated = await _mixGroupManager.DeActivateMixGroupAsync(mixChannel.MixGroupId);
            if (!mixGroupDeactivated)
                return new DiscordWorkflowResult(ErrorMessages.FailedToCloseMixGroup.GetValueForLanguage(), false);

            var mixSessions = await _mixSessionManager.GetActiveMixSessionsByMixGroupIdAsync(mixChannel.MixGroupId);
            var notifyRequests = new List<NotifyRequest>();
            foreach (var ms in mixSessions)
            {
                var mixSessionEnded = await _mixSessionManager.EndCurrentSessionAsync(ms.MixSessionId);
                if (mixSessionEnded.Success)
                {
                    await _mixChannelManager.DisableChannelAsync(ms.MixChannelId);
                    try
                    {
                        var serverId = await _serverManager.GetServerIdByDiscordServerId(props.ServerId);
                        if (serverId.HasValue)
                        {
                            var users = await _userMemberManager.GetUserMembersByUserIdsAndServerId(mixSessionEnded.SingedOutUsers, serverId.Value);
                            if (users.Any())
                            {
                                notifyRequests.AddRange(users
                                    .Where(u => ulong.Parse(u.DiscordUserId) != props.RequestedByUserId)
                                    .Select(u => new NotifyRequest(props.RequestedByUserId, ulong.Parse(u.DiscordUserId), props.ChannelId, props.ServerId))
                                );
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Logger.LogError(ex, "Something went wrong while collecting info to send sign out notification in the DeActivateMixGroupAsync");
                    }

                }
            }

            return new DiscordWorkflowResult(GeneralMessages.ClosedMixGroup.GetValueForLanguage(), true, notifyRequests: notifyRequests);
        }

        public async Task HandleAutoCloseAndRecreateOfMixSesisons()
        {
            var notifyRequests = new List<NotifyRequest>();
            var mixSessionsToClose = await _mixSessionManager.GetMixSessionsToCloseAsync();

            if (!mixSessionsToClose.Any())
                return;

            var discordClient = await _bot.GetConnectedDiscordClientAsync();

            foreach (var mixSessionToClose in mixSessionsToClose)
            {
                if (mixSessionToClose.MixChannel.MixGroup.Recurring == false && mixSessionToClose.MixChannel.MixGroup.IsActive == true)
                {
                    await _mixGroupManager.DeActivateMixGroupAsync(mixSessionToClose.MixChannel.MixGroupId);
                }
                var closedInternal = await _mixSessionManager.EndCurrentSessionAsync(mixSessionToClose.MixSessionId);
                if (closedInternal.Success)
                {
                    try
                    {
                        var users = await _userMemberManager.GetUserMembersByUserIdsAndServerId(closedInternal.SingedOutUsers, mixSessionToClose.MixChannel.MixGroup.Server.ServerId);
                        if (users.Any())
                        {
                            notifyRequests.AddRange(users
                                .Select(u => new NotifyRequest(discordClient.CurrentUser.Id, ulong.Parse(u.DiscordUserId), ulong.Parse(mixSessionToClose.MixChannel.DiscordChannelId), ulong.Parse(mixSessionToClose.MixChannel.MixGroup.Server.DiscordServerId)))
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Something went wrong while collecting info to send sign out notification in the DeActivateMixGroupAsync");
                    }

                    DiscordChannel discordChannel;
                    try
                    {
                        discordChannel = await discordClient.GetChannelAsync(ulong.Parse(mixSessionToClose.MixChannel.DiscordChannelId));

                        if (mixSessionToClose.MixChannel.MixGroup.Recurring && mixSessionToClose.MixChannel.MixGroup.IsActive && mixSessionToClose.MixChannel.Order > MixChannel.StartOrderNumber)
                        {
                            var allPermissions = discordChannel.PermissionOverwrites.ToList();
                            foreach (var rolePermissions in allPermissions.Where(r => r.Type == OverwriteType.Role))
                            {
                                if (rolePermissions.Allowed.HasPermission(Permissions.AccessChannels))
                                {
                                    var roleAccessPermissions = rolePermissions.Allowed.Revoke(Permissions.AccessChannels);
                                    var roleDeniedPermissions = rolePermissions.Denied.Grant(Permissions.AccessChannels);
                                    await discordChannel.AddOverwriteAsync(await rolePermissions.GetRoleAsync(), deny: roleDeniedPermissions, allow: roleAccessPermissions);
                                }
                            }
                        }

                        await discordChannel.SendMessageAsync(GeneralMessages.ClosedSession.GetValueForLanguage());

                    }
                    catch (NotFoundException ex)
                    {
                        await _mixChannelManager.DisableChannelAsync(mixSessionToClose.MixChannelId);
                        if (!await _mixGroupManager.HasActiveMixChannelsAsync(mixSessionToClose.MixChannel.MixGroupId))
                            await _mixGroupManager.DeActivateMixGroupAsync(mixSessionToClose.MixChannel.MixGroupId);
                        Logger.LogError(ex, $"Channel {mixSessionToClose.MixChannel.DiscordChannelId} was not found");
                        continue;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, $"End Channel {mixSessionToClose.MixChannel.DiscordChannelId} threw an error");
                        continue; 
                    }

                    //if previous did not fail...
                    if (mixSessionToClose.MixChannel.MixGroup.Recurring && mixSessionToClose.MixChannel.MixGroup.IsActive && mixSessionToClose.MixChannel.Order == MixChannel.StartOrderNumber)
                    {
                        try
                        {
                            //This startDate will be correct as long as the bot didn't have an almost full day off (there is a fix for this scenario up to 100 days in the createSessionAsync Method)
                            var result = await _mixSessionWorkflow.CreateSessionAsync(mixSessionToClose.MixChannel.MixGroup.ServerId, ulong.Parse(mixSessionToClose.MixChannel.DiscordChannelId), mixSessionToClose.DateStart.AddDays(1));
                            if (result.Success == true)
                            {
                                result = await _mixSessionWorkflow.ShowAsync(mixSessionToClose.MixChannel.MixGroup.ServerId, ulong.Parse(mixSessionToClose.MixChannel.DiscordChannelId));
                            }
                            await discordChannel.SendMessageAsync(result.Message);
                        }
                        catch (Exception ex)
                        {
                            await discordChannel.SendMessageAsync($"{ErrorMessages.CreateSessionError.GetValueForLanguage()}\n{ex.Message}");
                        }
                    }
                }
            }

            if (notifyRequests?.Any() ?? false)
            {
                foreach (var groupedServerSignOutNotifications in notifyRequests.GroupBy(x => x.RequestedInServer))
                {
                    var server = await discordClient.GetGuildAsync(groupedServerSignOutNotifications.Key);
                    foreach (var groupServerRequestedByNotification in groupedServerSignOutNotifications.GroupBy(x => x.RequestedBy))
                    {
                        foreach (var notifyRequest in groupServerRequestedByNotification)
                        {
                            try
                            {
                                var userMember = await server.GetMemberAsync(notifyRequest.RequestedFor);
                                await userMember.SendMessageAsync(string.Format(GeneralMessages.NotifySignedOut.GetValueForLanguage(), notifyRequest.RequestedInChannel, notifyRequest.RequestedBy, Constants.Bot.Prefix));
                            }
                            catch (ServerErrorException serverEx)
                            {
                                var message = string.Concat(serverEx.Message, "\nInnerException: ", serverEx.InnerException?.Message ?? string.Empty);
                                Logger.LogError(serverEx, $"Failed to send sign out notification to UserMemberId: {notifyRequest.RequestedFor}\nServerException: {message}\nRestResponseCode: {serverEx.WebResponse?.ResponseCode}\nRestResponse: {serverEx.WebResponse?.Response ?? string.Empty}\nJsonMessage: {serverEx.JsonMessage ?? string.Empty}");
                            }
                            catch (UnauthorizedException authEx)
                            {
                                var message = string.Concat(authEx.Message, "\nInnerException: ", authEx.InnerException?.Message ?? string.Empty);
                                Logger.LogError(authEx, $"Failed to send sign out notification to UserMemberId: {notifyRequest.RequestedFor}\nUnauthorizedException: {message}\nRestResponseCode: {authEx.WebResponse?.ResponseCode}\nRestResponse: {authEx.WebResponse?.Response ?? string.Empty}\nJsonMessage: {authEx.JsonMessage ?? string.Empty}");
                            }
                            catch (Exception ex)
                            {
                                var message = string.Concat(ex.Message, "\nInnerException: ", ex.InnerException?.Message ?? string.Empty);
                                Logger.LogError(ex, $"Failed to send sign out notification(otherEx count to UserMemberId: {notifyRequest.RequestedFor}\nException: {message}");
                            }
                            finally
                            {
                                await Task.Delay(TimeSpan.FromMilliseconds(100));
                            }
                        }
                    }
                }
            }
        }

        public async Task<DiscordWorkflowResult> RoleRegistrationAsync(DiscordCommandPropsDto props, ulong discordRoleId, string roleName, int serverId, int minutes)
        {
            var serverRole = await _serverRoleWorkflow.GetOrCreateServerRoleByDiscordRoleIdAndServerAsync(discordRoleId, roleName, serverId);
            var mixGroup = await _mixGroupManager.GetActiveMixGroupByDiscordServerAndChannelIdAsync(props.ChannelId.ToString(), props.ServerId.ToString());
            var changes = false;

            if (mixGroup == null)
                return new DiscordWorkflowResult(ErrorMessages.NoMixGroupFoundForSession.GetValueForLanguage(), false);

            var currentRoleOpening = mixGroup.MixGroupRoleOpenings.FirstOrDefault(mgro => mgro.ServerRoleId == serverRole.ServerRoleId);
            if (currentRoleOpening != null)
            {
                if (minutes == 0)
                {
                    changes = true;
                    await _mixGroupRoleOpeningManager.DeActivateAsync(currentRoleOpening.MixGroupRoleOpeningId);
                }
                else if (minutes != currentRoleOpening.Minutes)
                {
                    changes = true;
                    await _mixGroupRoleOpeningManager.DeActivateAsync(currentRoleOpening.MixGroupRoleOpeningId);
                    var mixGroupRoleOpening = new MixGroupRoleOpening()
                    {
                        ServerRoleId = serverRole.ServerRoleId,
                        MixGroupId = mixGroup.MixGroupId,
                        IsActive = true,
                        Minutes = minutes,
                        DateCreated = DateTimeHelper.GetDatabaseNow()
                    };
                    await _mixGroupRoleOpeningManager.AddAsync(mixGroupRoleOpening);
                }
            }
            else if (minutes != 0)
            {
                changes = true;
                var mixGroupRoleOpening = new MixGroupRoleOpening()
                {
                    ServerRoleId = serverRole.ServerRoleId,
                    MixGroupId = mixGroup.MixGroupId,
                    IsActive = true,
                    Minutes = minutes,
                    DateCreated = DateTimeHelper.GetDatabaseNow()
                };
                await _mixGroupRoleOpeningManager.AddAsync(mixGroupRoleOpening);
            }

            if (changes)
            {
                MemoryCache.Set(Cache.ActiveMixGroupsRoleOpenings.ToUpperInvariant(), await _mixGroupRoleOpeningManager.GetActiveMixGroupRoleOpenings(), TimeSpan.FromHours(121));
            }

            return new DiscordWorkflowResult(GeneralMessages.AppliedChanges.GetValueForLanguage(), true);
        }
    }
}
