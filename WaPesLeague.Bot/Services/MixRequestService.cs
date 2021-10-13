using AutoMapper;
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
using WaPesLeague.Business.Dto;
using WaPesLeague.Business.Dto.Mix;
using WaPesLeague.Business.Helpers;
using WaPesLeague.Business.Workflows.Interfaces;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Bot.Services
{
    public class MixRequestService : BaseBackgroundService<MixRequestService>
    {
        private readonly IServiceProvider _provider;
        private readonly IMapper _mapper;
        private readonly GeneralMessages GeneralMessages;
        private static int _serverNotifyExceptionCounter = 0;
        private static int _unauthorizedNotifyExceptionCounter = 0;
        private static int _otherNotifyExceptionCounter = 0;

        private const int delayMilliSeconds = 500;
        public MixRequestService(ILogger<MixRequestService> logger, IServiceProvider serviceProvider, IMapper mapper, GeneralMessages generalMessages)
            : base(logger)
        {
            _mapper = mapper;
            _provider = serviceProvider;
            GeneralMessages = generalMessages;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => Logger.LogDebug("MixRequestService background service Register Call."));
            ulong counter = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                var nowAtStartOfExecution = DateTime.Now;
                counter++;
                try
                {
                    var shouldContinue = true;
                    var requestsToHandle = new List<MixRequestDto>();
                    while(shouldContinue) {
                        MixRequestQueue.Queue.TryDequeue(out var mixRequestDto);
                        if (mixRequestDto != null)
                            requestsToHandle.Add(mixRequestDto);
                        else
                            shouldContinue = false;
                    }

                    if (requestsToHandle.Any())
                    {
                        var results = new List<DiscordWorkflowResult>();
                        using (var scope = _provider.CreateScope())
                        {
                            var _bot = scope.ServiceProvider.GetRequiredService<Base.Bot.Bot>();
                            var discordClient = await _bot.GetConnectedDiscordClientAsync();
                            var signOutNotifications = new List<NotifyRequest>();

                            foreach (var mixRequestDto in requestsToHandle)
                            {
                                Logger.LogInformation($"Handle Request: {mixRequestDto.ToLogString()}");
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
                                    default:
                                        Logger.LogError(new ArgumentException($"{mixRequestDto.RequestType} is not supported in the Hosted service: MixRequestService"), $"{mixRequestDto.RequestType} is not supported in the Hosted service: MixRequestService");
                                        break;
                                }
                                result.FollowUpParameters.DiscordChannelId = mixRequestDto.DiscordCommandProps.ChannelId;
                                result.FollowUpParameters.Server = mixRequestDto.Server;
                                result.FollowUpParameters.DiscordMessageId = mixRequestDto.DiscordCommandProps.MessageId;
                                results.Add(result);
                                if (result.NotifyRequests?.Any() ?? false)
                                    signOutNotifications.AddRange(result.NotifyRequests);
                            }
                            await HandleWorkflowResults(results, scope, discordClient);
                            await HandleSignOutNotificationAsync(signOutNotifications, discordClient);
                        }
                    }
                }
                catch(ServerErrorException serverErrorException)
                {
                    Logger.LogError(serverErrorException, $"MixRequestService catched a ServerErrorException: {serverErrorException.JsonMessage ?? string.Empty}.\n\n{serverErrorException.WebResponse?.ResponseCode ?? 0}: {serverErrorException.WebResponse?.Response ?? string.Empty}\n, Counter: {counter}");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"MixRequestService catched the following error., Counter: {counter}");
                }
                if ((counter % 12000) == 0)
                {
                    Logger.LogWarning($"MixRequestService has reached loops: {counter}");
                }
                await Task.Delay(BackgroundTimerHelper.TimeSpanInMilliSecondsTillNextRun(delayMilliSeconds, nowAtStartOfExecution), stoppingToken);
            }

            Logger.LogCritical("MixRequestService will no longer repeat itself");
        }

        #region CommandQueueHandling
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
            var _mixSessionWorkflow = scope.ServiceProvider.GetRequiredService<IMixSessionWorkflow>();
            var userId = await _userWorkflow.GetOrCreateUserIdByDiscordId(_mapper.Map<DiscordCommandPropsDto>(mixRequestDto.DiscordCommandProps));

            var signInDto = new SignInDto(mixRequestDto.DiscordCommandProps.ServerId, mixRequestDto.DiscordCommandProps.ChannelId, userId, mixRequestDto.Team, mixRequestDto.Position, mixRequestDto.ExtraInfo, mixRequestDto.Server.ServerId);

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

            return await _mixSessionWorkflow.SwapAsync(mixRequestDto.Server.ServerId, mixRequestDto.DiscordCommandProps.ChannelId, player1Id, player2Id, mixRequestDto.DiscordCommandProps.RequestedByUserId);
        }

        private async Task<DiscordWorkflowResult> HandleOpenTeamAsync(MixRequestDto mixRequestDto, IServiceScope scope)
        {
            var _mixSessionWorkflow = scope.ServiceProvider.GetRequiredService<IMixSessionWorkflow>();
            return await _mixSessionWorkflow.OpenTeamAsync(mixRequestDto.Server.ServerId, mixRequestDto.DiscordCommandProps.ChannelId);
        }

        private async Task<DiscordWorkflowResult> HandleCleanMixAsync(MixRequestDto mixRequestDto, IServiceScope scope)
        {
            var _mixSessionWorkflow = scope.ServiceProvider.GetRequiredService<IMixSessionWorkflow>();
            return await _mixSessionWorkflow.CleanRoomAsync(mixRequestDto.Server.ServerId, mixRequestDto.DiscordCommandProps.ChannelId, mixRequestDto.DiscordCommandProps.RequestedByUserId);

        }
        #endregion

        #region resultHandling
        private async Task HandleWorkflowResults(List<DiscordWorkflowResult> results, IServiceScope scope, DiscordClient discordClient)
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
                        await channel1.SendMessageAsync(firstDeleteSuccess.Message);
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
                    await channel1.SendMessageAsync(mixResult.Message);

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
