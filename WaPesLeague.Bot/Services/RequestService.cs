using AutoMapper;
using Base.Bot.Queue;
using Base.Bot.Queue.Dto;
using Base.Bot.Services;
using Base.Bot.Services.Helpers;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WaPesLeague.Bot.Commands.Mix;
using WaPesLeague.Bot.Commands.Server;
using WaPesLeague.Bot.Helpers;
using WaPesLeague.Business.Dto;
using WaPesLeague.Business.Dto.Mix;
using WaPesLeague.Business.Helpers;
using WaPesLeague.Business.Workflows.Interfaces;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;
using WaPesLeague.Data.Entities.Discord.Enums;
using WaPesLeague.Data.Helpers;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Bot.Services
{
    public class RequestService : BaseBackgroundService<RequestService>
    {
        private readonly IServiceProvider _provider;
        private readonly IMapper _mapper;
        private readonly GeneralMessages GeneralMessages;
        private readonly ErrorMessages ErrorMessages;
        private static int _serverNotifyExceptionCounter = 0;
        private static int _unauthorizedNotifyExceptionCounter = 0;
        private static int _otherNotifyExceptionCounter = 0;

        private const int delayMilliSeconds = 500;
        public RequestService(ILogger<RequestService> logger, IServiceProvider serviceProvider, IMapper mapper, GeneralMessages generalMessages, ErrorMessages errorMessages)
            : base(logger)
        {
            _mapper = mapper;
            _provider = serviceProvider;
            GeneralMessages = generalMessages;
            ErrorMessages = errorMessages;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => Logger.LogDebug("RequestService background service Register Call."));
            ulong counter = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                var nowAtStartOfExecution = DateTime.UtcNow;
                counter++;
                try
                {
                    var mixRequestsToHandle = MixRequestQueue.Queue.TryDeQueueAll();
                    var guildMembersAdded = GuildMemberAddedQueue.Queue.TryDeQueueAll();
                    var serverRequestsToHandle = ServerRequestQueue.Queue.TryDeQueueAll();
                    //var guildMembersUpdated = GuildMemberUpdatedQueue.Queue.TryDeQueueAll();

                    //if (mixRequestsToHandle.Any() || guildMembersAdded.Any() || serverRequestsToHandle.Any() || guildMembersUpdated.Any())
                    if (mixRequestsToHandle.Any() || guildMembersAdded.Any() || serverRequestsToHandle.Any())
                    {
                        var mixRequestResults = new List<DiscordWorkflowResult>();
                        var serverRequestResults = new List<DiscordWorkflowResult>();
                        using (var scope = _provider.CreateScope())
                        {
                            var _bot = scope.ServiceProvider.GetRequiredService<Base.Bot.Bot>();
                            var discordClient = await _bot.GetConnectedDiscordClientAsync();
                            var signOutNotifications = new List<NotifyRequest>();

                            foreach (var mixRequestDto in mixRequestsToHandle)
                            {
                                //Logger.LogInformation($"Handle Request: {mixRequestDto.ToLogString()}");
                                DiscordWorkflowResult result = null;
                                switch (mixRequestDto.RequestType)
                                {
                                    case MixRequestType.CreateMix:
                                        result = await HandleCreateMixAsync(mixRequestDto, scope);
                                        break;
                                    case MixRequestType.DeleteMix:
                                        result = await HandleDeleteMixAsync(mixRequestDto, scope);
                                        if (result.Success)
                                            result.FollowUpParameters.DeleteMixSuccess = true;
                                        break;
                                    case MixRequestType.SignIn:
                                        result = await HandleSignInAsync(mixRequestDto, scope);
                                        break;
                                    case MixRequestType.SignOut:
                                        result = await HandleSignOutAsync(mixRequestDto, signOutNotifications, scope);
                                        break;
                                    case MixRequestType.SetPassword:
                                        result = await HandleSetPasswordAsync(mixRequestDto, scope);
                                        break;
                                    case MixRequestType.SetServer:
                                        result = await HandleSetServerAsync(mixRequestDto, scope);
                                        break;
                                    case MixRequestType.SetRoom:
                                        result = await HandleSetRoomNameAsync(mixRequestDto, scope);
                                        break;
                                    case MixRequestType.SetRoomOwner:
                                        result = await HandleSetRoomOwnerAsync(mixRequestDto, scope);
                                        break;
                                    case MixRequestType.SetCaptain:
                                        result = await HandleSetCaptainAsync(mixRequestDto, scope);
                                        break;
                                    case MixRequestType.SetPlayerCount:
                                        result = await HandleSetPlayerCountAsync(mixRequestDto, scope);
                                        break;
                                    case MixRequestType.UpdatePosition:
                                        //ToDo add notification for the user that is affected by this action
                                        result = await HandleUpdatePositionAsync(mixRequestDto, scope);
                                        break;
                                    case MixRequestType.OpenTeam:
                                        result = await HandleOpenTeamAsync(mixRequestDto, scope);
                                        break;
                                    case MixRequestType.ShowMix:
                                        result = new DiscordWorkflowResult(string.Empty);
                                        break;
                                    case MixRequestType.CleanMix:
                                        result = await HandleCleanMixAsync(mixRequestDto, scope);
                                        break;
                                    case MixRequestType.Swap:
                                        result = await HandleSwapAsync(mixRequestDto, scope);
                                        break;
                                    case MixRequestType.RoleRegistration:
                                        result = await HandleRoleRegistrationAsync(mixRequestDto, scope);
                                        break;
                                    default:
                                        Logger.LogError(new ArgumentException($"{mixRequestDto.RequestType} is not supported in the Hosted service: RequestService"), $"{mixRequestDto.RequestType} is not supported in the Hosted service: RequestService");
                                        break;
                                }
                                result.FollowUpParameters.DiscordChannelId = mixRequestDto.DiscordCommandProps.ChannelId;
                                result.FollowUpParameters.Server = mixRequestDto.Server;
                                result.FollowUpParameters.DiscordMessageId = mixRequestDto.DiscordCommandProps.MessageId;
                                mixRequestResults.Add(result);
                                if (result.NotifyRequests?.Any() ?? false)
                                    signOutNotifications.AddRange(result.NotifyRequests);
                            }

                            foreach (var guildMemberAdded in guildMembersAdded)
                            {
                                await HandleGuildMemberAddedAsync(guildMemberAdded, scope, discordClient);
                            }

                            foreach (var serverRequestsDto in serverRequestsToHandle)
                            {
                                try
                                {
                                    DiscordWorkflowResult result = null;
                                    switch (serverRequestsDto.RequestType)
                                    {
                                        case ServerRequestType.GetServerButtons:
                                            result = await HandleGetServerButtonsAsync(serverRequestsDto, scope);
                                            break;
                                        case ServerRequestType.AddServerButton:
                                            result = await HandleAddServerButtonAsync(serverRequestsDto, scope);
                                            break;
                                        case ServerRequestType.DeleteButton:
                                            result = await HandleDeleteServerButtonAsync(serverRequestsDto, scope);
                                            break;
                                        case ServerRequestType.SetSniping:
                                            result = await HandleSetSnipingAsync(serverRequestsDto, scope);
                                            break;
                                        default:
                                            Logger.LogError(new ArgumentException($"{serverRequestsDto.RequestType} is not supported in the Hosted service: RequestService"), $"{serverRequestsDto.RequestType} is not supported in the Hosted service: RequestService");
                                            break;
                                    }
                                    result.FollowUpParameters.DiscordChannelId = serverRequestsDto.DiscordCommandProps.ChannelId;
                                    result.FollowUpParameters.Server = serverRequestsDto.Server;
                                    result.FollowUpParameters.DiscordMessageId = serverRequestsDto.DiscordCommandProps.MessageId;

                                    serverRequestResults.Add(result);
                                }
                                catch (Exception ex)
                                {
                                    Logger.LogError(ex, $"ServerRequestHandling failed for: {serverRequestsDto.ToLogString()}");
                                    var failedResult = new DiscordWorkflowResult(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message), false);
                                    failedResult.FollowUpParameters.DiscordChannelId = serverRequestsDto.DiscordCommandProps.ChannelId;
                                    failedResult.FollowUpParameters.Server = serverRequestsDto.Server;
                                    failedResult.FollowUpParameters.DiscordMessageId = serverRequestsDto.DiscordCommandProps.MessageId;
                                    serverRequestResults.Add(failedResult);
                                }
                                
                            }

                            //foreach (var guildMemberUpdated in guildMembersUpdated)
                            //{
                            //    await HandleGuildMemberUpdatedAsync(guildMemberUpdated, scope, discordClient);
                            //}

                            await HandleMixRequestWorkflowResults(mixRequestResults, scope, discordClient);
                            await HandleSignOutNotificationAsync(signOutNotifications, discordClient);

                            await HandleServerRequestWorkflowResults(serverRequestResults, scope, discordClient);
                        }
                    }
                }
                catch(ServerErrorException serverErrorException)
                {
                    Logger.LogError(serverErrorException, $"RequestService catched a ServerErrorException: {serverErrorException.JsonMessage ?? string.Empty}.\n\n{serverErrorException.WebResponse?.ResponseCode ?? 0}: {serverErrorException.WebResponse?.Response ?? string.Empty}\n, Counter: {counter}");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"RequestService catched the following error., Counter: {counter}");
                }
                if ((counter % 36000) == 0)
                {
                    Logger.LogWarning($"RequestService has reached loops: {counter}");
                }
                await Task.Delay(BackgroundTimerHelper.TimeSpanInMilliSecondsTillNextRun(delayMilliSeconds, nowAtStartOfExecution), stoppingToken);
            }

            Logger.LogCritical("RequestService will no longer repeat itself");
        }

        #region MixCommandQueueHandling
        private async Task<DiscordWorkflowResult> HandleCreateMixAsync(MixRequestDto mixRequestDto, IServiceScope scope)
        {
            var _mixGroupWorkflow = scope.ServiceProvider.GetRequiredService<IMixGroupWorkflow>();
            return await _mixGroupWorkflow.CreateMixGroupAsync(mixRequestDto.CreateMixRoomGroupDto);
        }

        private async Task<DiscordWorkflowResult> HandleDeleteMixAsync(MixRequestDto mixRequestDto, IServiceScope scope)
        {
            var _mixGroupWorkflow = scope.ServiceProvider.GetRequiredService<IMixGroupWorkflow>();
            return await _mixGroupWorkflow.DeActivateMixGroupAsync(_mapper.Map<DiscordCommandPropsDto>(mixRequestDto.DiscordCommandProps));
        }

        private async Task<DiscordWorkflowResult> HandleSignInAsync(MixRequestDto mixRequestDto, IServiceScope scope)
        {
            var _userWorkflow = scope.ServiceProvider.GetRequiredService<IUserWorkflow>();
            var _userMemberManager = scope.ServiceProvider.GetRequiredService<IUserMemberManager>();
            var _mixSessionWorkflow = scope.ServiceProvider.GetRequiredService<IMixSessionWorkflow>();
            var userId = await _userWorkflow.GetOrCreateUserIdByDiscordId(_mapper.Map<DiscordCommandPropsDto>(mixRequestDto.DiscordCommandProps));

            var dbNow = DateTimeHelper.GetDatabaseNow();
            var userMember = await _userMemberManager.GetUserMemberWithSnipersByUserIdAndServerIdAsync(userId, mixRequestDto.Server.ServerId, dbNow);

            var signInDto = new SignInDto(mixRequestDto.DiscordCommandProps.ServerId, mixRequestDto.DiscordCommandProps.ChannelId, userId, mixRequestDto.Team, mixRequestDto.Position, mixRequestDto.ExtraInfo, mixRequestDto.Server, mixRequestDto.RoleIdsPlayer1, mixRequestDto.ActorRoleIds, mixRequestDto.DiscordCommandProps.RequestedByUserId, userMember);

            return await _mixSessionWorkflow.SignInAsync(signInDto);
        }

        private async Task<DiscordWorkflowResult> HandleSignOutAsync(MixRequestDto mixRequestDto, List<NotifyRequest> signOutNotifications, IServiceScope scope)
        {
            var _userWorkflow = scope.ServiceProvider.GetRequiredService<IUserWorkflow>();
            var _mixSessionWorkflow = scope.ServiceProvider.GetRequiredService<IMixSessionWorkflow>();
            var userId = await _userWorkflow.GetOrCreateUserIdByDiscordId(_mapper.Map<DiscordCommandPropsDto>(mixRequestDto.DiscordCommandProps));

            var signOutResult = await _mixSessionWorkflow.SignOutAsync(mixRequestDto.Server.ServerId, mixRequestDto.DiscordCommandProps.ChannelId, userId);
            if (signOutResult.Success == true && mixRequestDto.DiscordCommandProps.RequestedByUserId != mixRequestDto.DiscordCommandProps.UserId)
            {
                signOutNotifications.Add(new NotifyRequest(mixRequestDto.DiscordCommandProps.RequestedByUserId, mixRequestDto.DiscordCommandProps.UserId, mixRequestDto.DiscordCommandProps.ChannelId, mixRequestDto.DiscordCommandProps.ServerId));
            }
            return signOutResult;
        }

        private async Task<DiscordWorkflowResult> HandleSetPasswordAsync(MixRequestDto mixRequestDto, IServiceScope scope)
        {
            var _mixSessionWorkflow = scope.ServiceProvider.GetRequiredService<IMixSessionWorkflow>();
            return await _mixSessionWorkflow.SetPasswordAsync(mixRequestDto.Server.ServerId, mixRequestDto.DiscordCommandProps.ChannelId, mixRequestDto.Password);
        }

        private async Task<DiscordWorkflowResult> HandleSetServerAsync(MixRequestDto mixRequestDto, IServiceScope scope)
        {
            var _mixSessionWorkflow = scope.ServiceProvider.GetRequiredService<IMixSessionWorkflow>();
            return await _mixSessionWorkflow.SetServerAsync(mixRequestDto.Server.ServerId, mixRequestDto.DiscordCommandProps.ChannelId, mixRequestDto.GameServer);
        }

        private async Task<DiscordWorkflowResult> HandleSetRoomNameAsync(MixRequestDto mixRequestDto, IServiceScope scope)
        {
            var _mixSessionWorkflow = scope.ServiceProvider.GetRequiredService<IMixSessionWorkflow>();
            return await _mixSessionWorkflow.SetRoomNameAsync(mixRequestDto.Server.ServerId, mixRequestDto.DiscordCommandProps.ChannelId, mixRequestDto.RoomName);
        }

        private async Task<DiscordWorkflowResult> HandleSetRoomOwnerAsync(MixRequestDto mixRequestDto, IServiceScope scope)
        {
            var _userWorkflow = scope.ServiceProvider.GetRequiredService<IUserWorkflow>();
            var _mixSessionWorkflow = scope.ServiceProvider.GetRequiredService<IMixSessionWorkflow>();
            var userId = await _userWorkflow.GetOrCreateUserIdByDiscordId(_mapper.Map<DiscordCommandPropsDto>(mixRequestDto.DiscordCommandProps));

            return await _mixSessionWorkflow.SetRoomOwnerAsync(mixRequestDto.Server.ServerId, mixRequestDto.DiscordCommandProps.ChannelId, userId);
        }

        private async Task<DiscordWorkflowResult> HandleSetCaptainAsync(MixRequestDto mixRequestDto, IServiceScope scope)
        {
            var _userWorkflow = scope.ServiceProvider.GetRequiredService<IUserWorkflow>();
            var _mixSessionWorkflow = scope.ServiceProvider.GetRequiredService<IMixSessionWorkflow>();
            var userId = await _userWorkflow.GetOrCreateUserIdByDiscordId(_mapper.Map<DiscordCommandPropsDto>(mixRequestDto.DiscordCommandProps));

            return await _mixSessionWorkflow.SetCaptainAsync(mixRequestDto.Server.ServerId, mixRequestDto.DiscordCommandProps.ChannelId, userId);
        }

        private async Task<DiscordWorkflowResult> HandleSetPlayerCountAsync(MixRequestDto mixRequestDto, IServiceScope scope)
        {
            var _mixSessionWorkflow = scope.ServiceProvider.GetRequiredService<IMixSessionWorkflow>();

            return await _mixSessionWorkflow.SetLockedTeamPlayerCount(mixRequestDto.Server.ServerId, mixRequestDto.DiscordCommandProps.ChannelId, mixRequestDto.PlayerCount, mixRequestDto.Team);
        }

        private async Task<DiscordWorkflowResult> HandleUpdatePositionAsync(MixRequestDto mixRequestDto, IServiceScope scope)
        {
            var _userWorkflow = scope.ServiceProvider.GetRequiredService<IUserWorkflow>();
            var _mixSessionWorkflow = scope.ServiceProvider.GetRequiredService<IMixSessionWorkflow>();
            var userId = await _userWorkflow.GetOrCreateUserIdByDiscordId(_mapper.Map<DiscordCommandPropsDto>(mixRequestDto.DiscordCommandProps));

            var changeMixSessionPositionDto = new ChangeMixSessionPositionDto(
                mixRequestDto.DiscordCommandProps.ServerId, 
                mixRequestDto.DiscordCommandProps.ChannelId, 
                userId, 
                mixRequestDto.Team, 
                mixRequestDto.OldPosition, 
                mixRequestDto.NewPosition,
                mixRequestDto.Server.ServerId);
            return await _mixSessionWorkflow.UpdatePositionAsync(changeMixSessionPositionDto);
        }

        private async Task<DiscordWorkflowResult> HandleSwapAsync(MixRequestDto mixRequestDto, IServiceScope scope)
        {
            var _userWorkflow = scope.ServiceProvider.GetRequiredService<IUserWorkflow>();
            var _mixSessionWorkflow = scope.ServiceProvider.GetRequiredService<IMixSessionWorkflow>();
            var player1Id = await _userWorkflow.GetOrCreateUserIdByDiscordId(_mapper.Map<DiscordCommandPropsDto>(mixRequestDto.Player1));
            var player2Id = await _userWorkflow.GetOrCreateUserIdByDiscordId(_mapper.Map<DiscordCommandPropsDto>(mixRequestDto.Player2));

            return await _mixSessionWorkflow.SwapAsync(mixRequestDto.Server.ServerId, mixRequestDto.DiscordCommandProps.ChannelId, player1Id, player2Id, mixRequestDto.DiscordCommandProps.RequestedByUserId, mixRequestDto.RoleIdsPlayer1, mixRequestDto.RoleIdsPlayer2, mixRequestDto.ActorRoleIds);
        }

        private async Task<DiscordWorkflowResult> HandleOpenTeamAsync(MixRequestDto mixRequestDto, IServiceScope scope)
        {
            var _mixSessionWorkflow = scope.ServiceProvider.GetRequiredService<IMixSessionWorkflow>();
            return await _mixSessionWorkflow.OpenTeamAsync(mixRequestDto.Server.ServerId, mixRequestDto.DiscordCommandProps.ChannelId, mixRequestDto.RoleId, mixRequestDto.RoleName, mixRequestDto.Minutes);
        }

        private async Task<DiscordWorkflowResult> HandleCleanMixAsync(MixRequestDto mixRequestDto, IServiceScope scope)
        {
            var _mixSessionWorkflow = scope.ServiceProvider.GetRequiredService<IMixSessionWorkflow>();
            return await _mixSessionWorkflow.CleanRoomAsync(mixRequestDto.Server.ServerId, mixRequestDto.DiscordCommandProps.ChannelId, mixRequestDto.DiscordCommandProps.RequestedByUserId);

        }

        private async Task<DiscordWorkflowResult> HandleRoleRegistrationAsync(MixRequestDto mixRequestDto, IServiceScope scope)
        {
            var _mixGroupWorkflow = scope.ServiceProvider.GetRequiredService<IMixGroupWorkflow>();
            return await _mixGroupWorkflow.RoleRegistrationAsync(_mapper.Map<DiscordCommandPropsDto>(mixRequestDto.DiscordCommandProps), mixRequestDto.RoleId.Value, mixRequestDto.RoleName, mixRequestDto.Server.ServerId, mixRequestDto.Minutes.Value);
        }
        #endregion

        #region ServerCommandQueueHandling
        private async Task<DiscordWorkflowResult> HandleGetServerButtonsAsync(ServerRequestDto serverRequestDto, IServiceScope scope)
        {
            var serverButtonWorkflow = scope.ServiceProvider.GetRequiredService<IServerButtonWorkflow>();
            return await serverButtonWorkflow.HandleGetServerButtonsAsync(serverRequestDto.Server);
        }

        private async Task<DiscordWorkflowResult> HandleAddServerButtonAsync(ServerRequestDto serverRequestDto, IServiceScope scope)
        {
            var serverButtonWorkflow = scope.ServiceProvider.GetRequiredService<IServerButtonWorkflow>();
            return await serverButtonWorkflow.HandleAddServerButtonAsync(_mapper.Map<DiscordCommandPropsDto>(serverRequestDto.DiscordCommandProps), serverRequestDto.Options);
        }

        private async Task<DiscordWorkflowResult> HandleDeleteServerButtonAsync(ServerRequestDto serverRequestDto, IServiceScope scope)
        {
            var serverButtonWorkflow = scope.ServiceProvider.GetRequiredService<IServerButtonWorkflow>();
            return await serverButtonWorkflow.HandleDeleteServerButtonAsync(serverRequestDto.Server, serverRequestDto.ServerButtonId.Value);
        }

        private async Task<DiscordWorkflowResult> HandleSetSnipingAsync(ServerRequestDto serverRequestDto, IServiceScope scope)
        {
            var serverWorkflow = scope.ServiceProvider.GetRequiredService<IServerWorkflow>();
            return await serverWorkflow.SetSnipingAsync(serverRequestDto.Server, serverRequestDto.SnipingIntervalAfterRegistrationOpeningInMinutes.Value, serverRequestDto.SnipingSignUpDelayInMinutes.Value, serverRequestDto.SnipingSignUpDelayDurationInHours.Value);
        }
        #endregion

        #region GuildMemberAddedOrUpdated
        private async Task HandleGuildMemberAddedAsync(GuildMemberAddedDto guildMemberAddedDto, IServiceScope scope, DiscordClient discordClient)
        {
            var _serverWorkflow = scope.ServiceProvider.GetRequiredService<IServerWorkflow>();
            var server =  await _serverWorkflow.GetOrCreateServerAsync(guildMemberAddedDto.DiscordServerId, guildMemberAddedDto.DiscordServerName);
            if (server.ServerEvents?.Any(se => se.EventType == EventType.MemberJoinedServer) ?? false)
            {
                foreach (var guildMemberJoinedEvent in server.ServerEvents.Where(se => se.EventType == EventType.MemberJoinedServer))
                {
                    switch (guildMemberJoinedEvent.ActionEntity)
                    {
                        case ActionEntity.Role:
                            try
                            {
                                var discordServer = await discordClient.GetGuildAsync(guildMemberAddedDto.DiscordServerId);
                                var addedMember = await discordServer.GetMemberAsync(guildMemberAddedDto.DiscordUserId);

                                switch (guildMemberJoinedEvent.ActionType)
                                {
                                    case ActionType.Add:
                                        var grantRole = discordServer.GetRole(ulong.Parse(guildMemberJoinedEvent.ActionValue));
                                        await addedMember.GrantRoleAsync(grantRole);
                                        break;
                                    default:
                                        Logger.LogError($"Invalid ActionType on the GuildMemberJoinedEvent of ServerEventId: {guildMemberJoinedEvent.ServerEventId}.");
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError(ex, $"Something went wrong while handling the guildMemberJoinedEvent for User: {guildMemberAddedDto.DiscordUserId} on ServerId: {guildMemberAddedDto.DiscordServerId}, ServerEventId: {guildMemberJoinedEvent.ServerEventId}.");
                            }
                            
                            break;
                        default:
                            Logger.LogError($"Invalid ActionEntity on the GuildMemberJoinedEvent of ServerEventId: {guildMemberJoinedEvent.ServerEventId}.");
                            break;
                    }
                }
            }
        }

        //private async Task HandleGuildMemberUpdatedAsync(GuildMemberUpdatedDto guildMemberUpdatedDto, IServiceScope scope, DiscordClient discordClient)
        //{
        //    var _serverWorkflow = scope.ServiceProvider.GetRequiredService<IServerWorkflow>();
        //    var server = await _serverWorkflow.GetOrCreateServerAsync(guildMemberUpdatedDto.DiscordServerId, guildMemberUpdatedDto.DiscordServerName);
        //    var guild = await discordClient.GetGuildAsync(guildMemberUpdatedDto.DiscordServerId);
        //    var member = await guild.GetMemberAsync(guildMemberUpdatedDto.DiscordUserId);
        //    var logs = await guild.GetAuditLogsAsync(10);
        //    var a = "zever";

        //}
        #endregion

        #region resultHandling
        private async Task HandleMixRequestWorkflowResults(List<DiscordWorkflowResult> results, IServiceScope scope, DiscordClient discordClient)
        {
            if (!results.Any())
                return;
            
            var groupedResults = results.GroupBy(r => r.Success).OrderBy(rg => rg.Key).ToList();
            var failedResults = groupedResults.Where(gr => gr.Key == false).SelectMany(x => x).OrderBy(x => x.FollowUpParameters.DiscordChannelId).ToList();
            var successResults = groupedResults.Where(gr => gr.Key == true).SelectMany(x => x).OrderBy(x => x.FollowUpParameters.DiscordChannelId).ToList();
            var channels = new List<DiscordChannel>();

            if (failedResults.Any())
            {
                foreach (var channelFailedResults in failedResults.GroupBy(x => x.FollowUpParameters.DiscordChannelId))
                {
                    var channel = await discordClient.GetChannelAsync(channelFailedResults.Key);
                    channels.Add(channel);
                    foreach (var failedResult in channelFailedResults)
                    {
                        var message = await channel.GetMessageAsync(failedResult.FollowUpParameters.DiscordMessageId);
                        var msg = await new DiscordMessageBuilder()
                            .WithContent($"{Constants.DiscordEmoji.ThumbsDownString} {failedResult.Message}")
                            .AddDiscordLinkButtonsToMessageIfNeeded(failedResult.FollowUpParameters.Server, new Random())
                            .WithReply(message.Id, true)
                            .SendAsync(channel);
                    }
                }
            }
            if (successResults.Any())
            {
                var _mixSessionWorkflow = scope.ServiceProvider.GetRequiredService<IMixSessionWorkflow>();
                var _mixSessionManager = scope.ServiceProvider.GetRequiredService<IMixSessionManager>();
                var _mixChannelWorkflow = scope.ServiceProvider.GetRequiredService<IMixChannelWorkflow>();
                var followUpCreated = new List<int>();

                foreach (var channelSuccessResults in successResults.GroupBy(x => x.FollowUpParameters.DiscordChannelId))
                {
                    var firstChannelResult = channelSuccessResults.First().FollowUpParameters;
                    var channel1 = channels.FirstOrDefault(ch => ch.Id == channelSuccessResults.Key) ?? await discordClient.GetChannelAsync(channelSuccessResults.Key);
                    
                    if (channelSuccessResults.Any(x => x.FollowUpParameters.DeleteMixSuccess == true))
                    {
                        var firstDeleteSuccess = channelSuccessResults.First(x => x.FollowUpParameters.DeleteMixSuccess == true);
                        var deleteMessage = new DiscordMessageBuilder()
                            .WithContent(firstDeleteSuccess.Message)
                            .AddDiscordLinkButtonsToMessageIfNeeded(firstDeleteSuccess.FollowUpParameters.Server, new Random());
                        await channel1.SendMessageAsync(deleteMessage);
                        continue;
                    }

                    foreach(var publicUserNotification in channelSuccessResults.Where(z => !string.IsNullOrWhiteSpace(z.FollowUpParameters.NotifyUserWithPublicMessage)))
                    {
                        var message = await channel1.GetMessageAsync(publicUserNotification.FollowUpParameters.DiscordMessageId);
                        var msg = await new DiscordMessageBuilder()
                            .WithContent(publicUserNotification.FollowUpParameters.NotifyUserWithPublicMessage)
                            .WithReply(message.Id, true)
                            .SendAsync(channel1);
                    }

                    var mixResult = await _mixSessionWorkflow.ShowAsync(firstChannelResult.Server.ServerId, channelSuccessResults.Key);
                    var replyMessage = new DiscordMessageBuilder()
                        .WithContent(mixResult.Message)
                        .AddDiscordLinkButtonsToMessageIfNeeded(firstChannelResult.Server, new Random());

                    await channel1.SendMessageAsync(replyMessage);

                    if (channelSuccessResults.Any(x => x.FollowUpParameters.CheckOpenExtraChannel == true))
                    {
                        var needsNewSession = await _mixSessionManager.CheckIfExtraMixSessionShouldBeCreatedAsync(firstChannelResult.MixGroupId);
                        if (needsNewSession)
                        {
                            followUpCreated.Add(firstChannelResult.MixGroupId);
                            await _mixChannelWorkflow.CreateFollowUpForMixSessionIdAsync(discordClient, firstChannelResult.MixGroupId);
                        }
                    }

                    if (channelSuccessResults.Any(x => x.FollowUpParameters.UpdateChannelName == true))
                    {
                        var channelResultToCreate = channelSuccessResults.First(x => x.FollowUpParameters.UpdateChannelName == true);
                        var channelName = string.IsNullOrWhiteSpace(channelResultToCreate.FollowUpParameters?.ChannelName)
                            ? "Unknown"
                            : channelResultToCreate.FollowUpParameters.ChannelName;
                        try { await channel1.ModifyAsync(c => c.Name = channelName); }
                        catch (Exception ex) { await channel1.SendMessageAsync($"We were not able to modify the channel name to: {channelName}, {ex.Message}"); }
                    }
                }
            }
        }

        private async Task HandleServerRequestWorkflowResults(List<DiscordWorkflowResult> results, IServiceScope scope, DiscordClient discordClient)
        {
            if (!results.Any())
                return;


            var channels = new List<DiscordChannel>();
            var failedResults = results.Where(r => r.Success == false).ToList();
            var successResults = results.Where(r => r.Success == true).ToList();

            if (failedResults.Any())
            {
                foreach (var channelFailedResults in failedResults.GroupBy(x => x.FollowUpParameters.DiscordChannelId))
                {
                    var channel = await discordClient.GetChannelAsync(channelFailedResults.Key);
                    channels.Add(channel);
                    foreach (var failedResult in channelFailedResults)
                    {
                        var message = await channel.GetMessageAsync(failedResult.FollowUpParameters.DiscordMessageId);
                        var msg = await new DiscordMessageBuilder()
                            .WithContent($"{Constants.DiscordEmoji.ThumbsDownString} {failedResult.Message}")
                            .AddDiscordLinkButtonsToMessageIfNeeded(failedResult.FollowUpParameters.Server, new Random())
                            .WithReply(message.Id, true)
                            .SendAsync(channel);
                    }
                }
            }

            if (successResults.Any())
            {
                foreach (var channelSuccessResults in successResults.GroupBy(x => x.FollowUpParameters.DiscordChannelId))
                {
                    var channel1 = channels.FirstOrDefault(ch => ch.Id == channelSuccessResults.Key) ?? await discordClient.GetChannelAsync(channelSuccessResults.Key);
                    foreach (var successResult in channelSuccessResults)
                    {
                        var message = await channel1.GetMessageAsync(successResult.FollowUpParameters.DiscordMessageId);
                        var msg = await new DiscordMessageBuilder()
                            .WithContent($"{Constants.DiscordEmoji.ThumbsUpString} {successResult.Message}")
                            .AddDiscordLinkButtonsToMessageIfNeeded(successResult.FollowUpParameters.Server, new Random())
                            .WithReply(message.Id, true)
                            .SendAsync(channel1);
                    }
                }
            }

        }

        private async Task HandleSignOutNotificationAsync(List<NotifyRequest> singOutNotifyRequests, DiscordClient discordClient)
        {
            try
            {
                if (singOutNotifyRequests?.Any() ?? false)
                {
                    foreach (var groupedServerSignOutNotifications in singOutNotifyRequests.GroupBy(x => x.RequestedInServer))
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
                                    _serverNotifyExceptionCounter++;
                                    var message = string.Concat(serverEx.Message, "\nInnerException: ", serverEx.InnerException?.Message ?? string.Empty);
                                    Logger.LogError(serverEx, $"Failed to send sign out notification (serverEx count: {_serverNotifyExceptionCounter}) to UserMemberId: {notifyRequest.RequestedFor}\nServerException: {message}\nRestResponseCode: {serverEx.WebResponse?.ResponseCode}\nRestResponse: {serverEx.WebResponse?.Response ?? string.Empty}\nJsonMessage: {serverEx.JsonMessage ?? string.Empty}");
                                }
                                catch (UnauthorizedException authEx)
                                {
                                    _unauthorizedNotifyExceptionCounter++;
                                    var message = string.Concat(authEx.Message, "\nInnerException: ", authEx.InnerException?.Message ?? string.Empty);
                                    Logger.LogError(authEx, $"Failed to send sign out notification(unAuthorizedEx count: {_unauthorizedNotifyExceptionCounter}) to UserMemberId: {notifyRequest.RequestedFor}\nUnauthorizedException: {message}\nRestResponseCode: {authEx.WebResponse?.ResponseCode}\nRestResponse: {authEx.WebResponse?.Response ?? string.Empty}\nJsonMessage: {authEx.JsonMessage ?? string.Empty}");
                                }
                                catch (Exception ex)
                                {
                                    _otherNotifyExceptionCounter++;
                                    var message = string.Concat(ex.Message, "\nInnerException: ", ex.InnerException?.Message ?? string.Empty);
                                    Logger.LogError(ex, $"Failed to send sign out notification(otherEx count: {_otherNotifyExceptionCounter}) to UserMemberId: {notifyRequest.RequestedFor}\nException: {message}");
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
            catch(Exception ex)
            {
                Logger.LogError(ex, "Something went wrong in HandleSignOutNotificationAsync");
            }
        }
        #endregion
    }
}
