using AutoMapper;
using DSharpPlus.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WaPesLeague.Business.Dto.Mix;
using WaPesLeague.Business.Helpers;
using WaPesLeague.Business.Workflows.Interfaces;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;
using WaPesLeague.Data.Entities.Mix;
using WaPesLeague.Data.Helpers;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Business.Workflows
{
    public class MixSessionWorkflow : BaseWorkflow<MixSessionWorkflow>, IMixSessionWorkflow
    {
        private readonly IMixGroupRoleOpeningWorkflow _mixGroupRoleOpeningWorkflow;
        private readonly IServerRoleWorkflow _serverRoleWorkflow;
        private readonly IMixChannelManager _mixChannelManager;
        private readonly IMixSessionManager _mixSessionManager;
        private readonly IMixPositionManager _mixPositionManager;
        private readonly IMixPositionReservationManager _mixPositionReservationManager;
        private readonly IPositionManager _positionManager;
        private readonly IMixTeamManager _mixTeamManager;
        private readonly IMixTeamRoleOpeningManager _mixTeamRoleOpeningManager;
        

        private readonly Base.Bot.Bot _bot;

        public MixSessionWorkflow(IMixGroupRoleOpeningWorkflow mixGroupRoleOpeningWorkflow, IServerRoleWorkflow serverRoleWorkflow,
            IMixChannelManager mixChannelManager,  IMixSessionManager mixSessionManager, IMixPositionManager mixPositionManager, IMixPositionReservationManager mixPositionReservationManager,
            IPositionManager positionManager, IMixTeamManager mixTeamManager, IMixTeamRoleOpeningManager mixTeamRoleOpeningManager, Base.Bot.Bot bot,
            IMemoryCache cache, IMapper mapper, ILogger<MixSessionWorkflow> logger, ErrorMessages errorMessages, GeneralMessages generalMessages)
            : base(cache, mapper, logger, errorMessages, generalMessages)
        {
            _mixGroupRoleOpeningWorkflow = mixGroupRoleOpeningWorkflow;
            _serverRoleWorkflow = serverRoleWorkflow;
            _mixChannelManager = mixChannelManager;
            _mixSessionManager = mixSessionManager;
            _mixPositionManager = mixPositionManager;
            _mixPositionReservationManager = mixPositionReservationManager;
            _positionManager = positionManager;
            _mixTeamManager = mixTeamManager;
            _mixTeamRoleOpeningManager = mixTeamRoleOpeningManager;

            _bot = bot;
        }   

        public async Task<DiscordWorkflowResult> CreateSessionAsync(int serverId, ulong discordChannelId, DateTime? startDate = null, bool useExactStartDate = false)
        {
            var mixChannel = await _mixChannelManager.GetActiveMixChannelByDiscordChannelIdAndInternalServerId(serverId, discordChannelId.ToString());
            if (mixChannel == null)
                return new DiscordWorkflowResult(string.Format(ErrorMessages.NoChannelFoundForMixSession.GetValueForLanguage(), discordChannelId, serverId), false);
            if (mixChannel.MixSessions.Any(ms => ms.DateClosed == null))
                return new DiscordWorkflowResult(ErrorMessages.OtherSessionWithinChannel.GetValueForLanguage(), false);
            
            var dbNow = DateTimeHelper.GetDatabaseNow();
            var start = startDate ?? DateTimeHelper.CreateDbDateTimeForDecimalTimeInApplicationZone(mixChannel.MixGroup.Start, mixChannel.MixGroup.Server.TimeZoneName);
            //If start is more than 3 hours in the past then bring it back to the future :-D
            var counter = 0;
            while (!useExactStartDate && start < dbNow.AddHours(-3) && counter < 100) //avoid very long loop
            {
                counter++;
                start = start.AddDays(1);
            }    

            var registrationTime = start.AddHours(-(double)mixChannel.MixGroup.HoursToOpenRegistrationBeforeStart);
            if (registrationTime.Second > 30)
            {
                registrationTime = registrationTime.AddSeconds(60 - registrationTime.Second);
            }
            var mixSession = new MixSession()
            {
                MixChannelId = mixChannel.MixChannelId,
                DateStart = start,
                DateRegistrationOpening = registrationTime,
                DateToClose = start.AddHours((double)mixChannel.MixGroup.MaxSessionDurationInHours),
                CrashCount = 0,
                MatchCount = 0,
                DateLastUpdated = dbNow,
                Password = mixChannel.MixGroup.Server.DefaultSessionPassword,
                MixTeams = mixChannel.MixChannelTeams.Select(mct =>
                {
                    return new MixTeam()
                    {
                        Name = mct.MixChannelTeamName,
                        PositionsLocked = !mct.IsOpen,
                        Tags = mct.Tags.Select(t => new MixTeamTag() { Tag = t.Tag }).ToList(),
                        Formation = mct.IsOpen
                            ? mct.DefaultFormation.Where(mp => mct.IsOpen).Select(cp => new MixPosition()
                                {
                                    PositionId = cp.PositionId,
                                    DateStart = dbNow,
                                    DateEnd = null,
                                }).ToList()
                            : new List<MixPosition>()
                    };
                }).ToList()
            };

            mixSession = await _mixSessionManager.AddAsync(mixSession);
            var result = new DiscordWorkflowResult(GeneralMessages.AppliedChanges.GetValueForLanguage(), true);
            return new DiscordWorkflowResult(GeneralMessages.AppliedChanges.GetValueForLanguage(), true);
        }

        public async Task<DiscordWorkflowResult> SignInAsync(SignInDto dto)
        {
            var validator = new SignInDtoValidator(_mixChannelManager, _mixSessionManager, _mixGroupRoleOpeningWorkflow, this, ErrorMessages, GeneralMessages);
            var validationResults = await validator.ValidateAsync(dto);
            if (validationResults.Errors.Any())
                return HandleValidationResults(validationResults);

            var mixChannelId = await _mixChannelManager.GetActiveMixChannelIdByDiscordIds(dto.DiscordServerId.ToString(), dto.DiscordChannelId.ToString());
            if (mixChannelId == null)
                return new DiscordWorkflowResult(string.Format(ErrorMessages.NoChannelFoundForDiscordIds.GetValueForLanguage(), dto.DiscordServerId, dto.DiscordChannelId), false);
            var mixSession = await _mixSessionManager.GetCurrentMixSessionByChannelIdAsync(mixChannelId.Value);

            var teamToSignInFor = mixSession.MixTeams.SingleOrDefault(mt => mt.HasNameOrTag(dto.Team));
            if (teamToSignInFor == null)
                return new DiscordWorkflowResult($"{ErrorMessages.NoTeamFound.GetValueForLanguage()}: {dto.Team}", false);

            var teamToSignInForRoleOpening = teamToSignInFor.MixTeamRoleOpenings.FirstOrDefault();
            if (teamToSignInForRoleOpening != null && !dto.ActorRoleIds.Contains(teamToSignInForRoleOpening.ServerRole.DiscordRoleId, StringComparer.InvariantCultureIgnoreCase))
                return new DiscordWorkflowResult(ErrorMessages.RegistrationWithinValidHours.GetValueForLanguage(), false);

            MixPosition userPosition = null;
            var mixtTeamPositions = await _mixPositionManager.GetAvailablePositionsForTeamAsync(teamToSignInFor.MixTeamId);
            if (!mixtTeamPositions.Any())
            {
                userPosition = await _mixPositionManager.GetActivePositionByUserIdMixTeamIdAsync(teamToSignInFor.MixTeamId, dto.UserId);
                if (userPosition == null)
                {
                    return new DiscordWorkflowResult($"{ErrorMessages.NoOpenPositionsInTeam.GetValueForLanguage()}: {dto.Team}", false);
                }
            }

            if (userPosition == null)
                userPosition = await _mixPositionManager.GetActivePositionByUserIdMixTeamIdAsync(teamToSignInFor.MixTeamId, dto.UserId);

            var isUpdateOnCurrentPosition = userPosition != null
                && (userPosition.Position.HasCodeOrTag(dto.Position)
                    || (userPosition.Position.ParentPosition?.HasCodeOrTag(dto.Position) ?? false)
                    || userPosition.Position.ChildPositions.Any(cp => cp.HasCodeOrTag(dto.Position)));


            var positionsToSignInTo = mixtTeamPositions.FirstOrDefault(x => x.Position.HasCodeOrTag(dto.Position))
                ?? mixtTeamPositions.FirstOrDefault(x => x.Position.ParentPosition?.HasCodeOrTag(dto.Position) ?? false || x.Position.ChildPositions.Any(cp => cp.HasCodeOrTag(dto.Position)));
            if (positionsToSignInTo == null && !isUpdateOnCurrentPosition)
                return new DiscordWorkflowResult(string.Format(ErrorMessages.NoPositionFoundInTeam.GetValueForLanguage(), dto.Team, dto.Position), false);


            var currentReservation = mixSession.MixTeams
                .Where(mt => !mt.PositionsLocked)
                .SelectMany(mt => mt.Formation
                    .Where(f => f.DateEnd == null)
                    .SelectMany(f => f.Reservations
                        .Where(r => r.UserId == dto.UserId && r.DateEnd == null)))
                .FirstOrDefault();

            var mixPositionIdToSignInTo = isUpdateOnCurrentPosition ? userPosition.MixPositionId : positionsToSignInTo.MixPositionId;
            var mixTeamIdToSignInTo = isUpdateOnCurrentPosition ? userPosition.MixTeamId : positionsToSignInTo.MixTeamId;
            var isRequiredForMix = isUpdateOnCurrentPosition ? userPosition.Position.IsRequiredForMix : positionsToSignInTo.Position.IsRequiredForMix;

            var applicationNow = DateTimeHelper.GetDatabaseNow();
            var isDuplicateOfCurrentReservation = currentReservation != null && currentReservation.MixPositionId == mixPositionIdToSignInTo && string.Equals(currentReservation.ExtraInfo, dto.ExtraInfo, StringComparison.InvariantCulture);
            if (currentReservation != null && !isDuplicateOfCurrentReservation)
            {
                await HandleSignOutAsync(currentReservation, mixSession, applicationNow, false);
            }

            if (currentReservation == null || !isDuplicateOfCurrentReservation)
            {
               
                var isCaptain =
                    currentReservation != null
                    && currentReservation.IsCaptain
                    && (
                        currentReservation.MixPosition.MixTeamId == mixTeamIdToSignInTo
                        || (
                            currentReservation.MixPosition.MixTeamId != mixTeamIdToSignInTo
                            && !mixSession.MixTeams.Single(x => x.MixTeamId == mixTeamIdToSignInTo).Formation.Where(f => f.DateEnd == null).SelectMany(f => f.Reservations.Where(r => r.DateEnd == null && r.IsCaptain == true)).Any()
                            )
                        );
                var mixPostionReservation = new MixPositionReservation()
                {
                    MixPositionId = mixPositionIdToSignInTo,
                    UserId = dto.UserId,
                    DateStart = DateTimeHelper.GetDatabaseNow(),
                    IsCaptain = isCaptain,
                    DateEnd = null,
                    ExtraInfo = dto.ExtraInfo
                };

                await _mixPositionReservationManager.AddAsync(mixPostionReservation);
            }

            var result = new DiscordWorkflowResult(GeneralMessages.AppliedChanges.GetValueForLanguage(), true);
            ValidateExtraInfoForSingUpTimeInfo(result, dto.ExtraInfo);

            //This means we filled in the last spot for 1 of the teams --> so we need to make a call to check if the other team and Mixes are also full and if so, open a new mix!
            if (mixtTeamPositions.Where(mp => mp.Position.IsRequiredForMix == true).Count() == 1 && isRequiredForMix && mixSession.MixChannel.MixGroup.CreateExtraMixChannels)
            {
                result.FollowUpParameters.CheckOpenExtraChannel = true; 
                result.FollowUpParameters.MixSessionId = mixSession.MixSessionId;
                result.FollowUpParameters.MixChannelId = mixSession.MixChannelId;
                result.FollowUpParameters.MixGroupId = mixSession.MixChannel.MixGroupId;
                result.FollowUpParameters.DiscordChannelId = ulong.Parse(mixSession.MixChannel.DiscordChannelId);
            }
            return result;
        }

        private void ValidateExtraInfoForSingUpTimeInfo(DiscordWorkflowResult result, string extraInfo)
        {
            if (!string.IsNullOrWhiteSpace(extraInfo))
            {
                var regex = new Regex(@"[0-2]?[0-9][:,h](([0-5][0-9])|\s|$)");
                if (regex.IsMatch(extraInfo))
                {
                    result.FollowUpParameters.NotifyUserWithPublicMessage = GeneralMessages.ExtraInfoWithTimeInfoDickMove.GetValueForLanguage();
                }
            }
        }

        public async Task<DiscordWorkflowResult> SignOutAsync(int serverId, ulong discordChannelId, int userId)
        {
            var reservation = await _mixPositionReservationManager.GetActiveMixPositionReservationByServerIdAndDiscordChannelIdAndUserIdAsync(serverId, discordChannelId.ToString(), userId);

            if(reservation != null)
            {
                var applicationNow = DateTimeHelper.GetDatabaseNow();
                var mixSession = await _mixSessionManager.GetActiveMixSessionByServerIdAndDiscordChannelIdAsync(serverId, discordChannelId.ToString());
                await HandleSignOutAsync(reservation, mixSession, applicationNow); //check if mixSession should be returned from this method
                return new DiscordWorkflowResult(GeneralMessages.AppliedChanges.GetValueForLanguage(), true);
            }

            return new DiscordWorkflowResult(GeneralMessages.NothingChanged.GetValueForLanguage(), false);
        }

        private async Task HandleSignOutAsync(MixPositionReservation reservation, MixSession mixSession,DateTime date, bool roomOwnerCheck = true)
        {
            var reservationToClose = Mapper.Map<MixPositionReservation>(reservation);
            reservationToClose.DateEnd = date;
            await _mixPositionReservationManager.UpdateAsync(reservationToClose);

            if (roomOwnerCheck && mixSession.RoomOwnerId == reservation.UserId)
            {
                mixSession.RoomOwner = null;
                mixSession.RoomOwnerId = null;
                var mixSessionToUpdate = Mapper.Map<MixSession>(mixSession);
                await _mixSessionManager.UpdateAsync(mixSessionToUpdate);
            }
        }

        public async Task<DiscordWorkflowResult> ShowAsync(int serverId, ulong discordChannelId)
        {
            var mixSession = await _mixSessionManager.GetActiveMixSessionByServerIdAndDiscordChannelIdAsync(serverId, discordChannelId.ToString());
            if (mixSession != null)
            {
                
                var mixGroupRoleOpenings = (await _mixGroupRoleOpeningWorkflow.GetMixGroupRoleOpeningsFromCacheOrDbAsync()).Where(mgro => mgro.MixGroupId == mixSession.MixChannel.MixGroupId).ToList();
                //var dbNow = DateTimeHelper.GetDatabaseNow();
                //var mixTeamIds = mixSession.MixTeams.Select(mt => mt.MixTeamId).ToList();
                //var mixTeamRoleOpenings = (await _mixTeamRoleOpeningWorkflow.GetMixTeamRoleOpeningsFromCacheOrDbAsync()).Where(mtro => mixTeamIds.Contains(mtro.MixTeamId) && dbNow < mtro.End).ToList();
                return MapMixSessionWithRelatedDataToDiscordString(mixSession, mixSession.MixChannel.MixGroup.Server.TimeZoneName, mixGroupRoleOpenings);
            } 

            return new DiscordWorkflowResult(ErrorMessages.NoActiveSessionFoundInChannel.GetValueForLanguage(), false);
        }
        public async Task<DiscordWorkflowResult> ShowSidesAsync(int serverId, ulong discordChannelId)
        {
            var mixSession = await _mixSessionManager.GetActiveMixSessionByServerIdAndDiscordChannelIdAsync(serverId, discordChannelId.ToString());
            if (mixSession != null)
            {
                var sb = new StringBuilder();
                var showPlayerCount = true;
                var totalPlayers = 0;
                foreach (var mixTeam in mixSession.MixTeams.OrderBy(mt => mt.Tags.OrderBy(t => t.Tag).FirstOrDefault().Tag).ThenBy(mt => mt.Name))
                {
                    if (mixTeam.PositionsLocked && !mixTeam.LockedTeamPlayerCount.HasValue)
                        showPlayerCount = false;
                    totalPlayers += mixTeam.PositionsLocked
                        ? mixTeam.LockedTeamPlayerCount ?? 0
                        : mixTeam.Formation.Count(x => x.Reservations.Any(r => r.DateEnd == null));
                }
                if (showPlayerCount)
                {
                    sb.AppendLine($"{totalPlayers} {GeneralMessages.PlayersSubscribed.GetValueForLanguage()}");
                }
                sb.AppendLine();
                var orderedMixTeams = mixSession.MixTeams.OrderBy(mt => mt.Tags.OrderBy(t => t.Tag).FirstOrDefault().Tag).ThenBy(mt => mt.Name).ToList();
                sb.AppendLine(HandlePESSideSelectionInfo(mixSession, orderedMixTeams));

                return new DiscordWorkflowResult(sb.ToString());
            }

            return new DiscordWorkflowResult(ErrorMessages.NoActiveSessionFoundInChannel.GetValueForLanguage(), false);
        }

        public async Task<DiscordWorkflowResult> SetPasswordAsync(int serverId, ulong discordChannelId, string password)
        {
            var activeMixSession = await _mixSessionManager.GetActiveMixSessionByServerIdAndDiscordChannelIdAsync(serverId, discordChannelId.ToString());
            if (activeMixSession == null)
                return new DiscordWorkflowResult(ErrorMessages.NoActiveSessionFoundInChannel.GetValueForLanguage(), false);

            activeMixSession.Password = password;
            var mixSessionToSave = Mapper.Map<MixSession>(activeMixSession);
            await _mixSessionManager.UpdateAsync(mixSessionToSave);

            return new DiscordWorkflowResult(GeneralMessages.AppliedChanges.GetValueForLanguage(), true);
        }

        public async Task<DiscordWorkflowResult> SetServerAsync(int serverId, ulong discordChannelId, string serverName)
        {
            var activeMixSession = await _mixSessionManager.GetActiveMixSessionByServerIdAndDiscordChannelIdAsync(serverId, discordChannelId.ToString());
            if (activeMixSession == null)
                return new DiscordWorkflowResult(ErrorMessages.NoActiveSessionFoundInChannel.GetValueForLanguage(), false);

            activeMixSession.Server = serverName;
            var mixSessionToSave = Mapper.Map<MixSession>(activeMixSession);
            await _mixSessionManager.UpdateAsync(mixSessionToSave);

            return new DiscordWorkflowResult(GeneralMessages.AppliedChanges.GetValueForLanguage(), true);
        }

        public async Task<DiscordWorkflowResult> SetRoomNameAsync(int serverId, ulong discordChannelId, string roomName)
        {
            var activeMixSession = await _mixSessionManager.GetActiveMixSessionByServerIdAndDiscordChannelIdAsync(serverId, discordChannelId.ToString());
            if (activeMixSession == null)
                return new DiscordWorkflowResult(ErrorMessages.NoActiveSessionFoundInChannel.GetValueForLanguage(), false);

            activeMixSession.GameRoomName = roomName;
            var mixSessionToSave = Mapper.Map<MixSession>(activeMixSession);
            await _mixSessionManager.UpdateAsync(mixSessionToSave);

            return new DiscordWorkflowResult(GeneralMessages.AppliedChanges.GetValueForLanguage(), true);
        }

        public async Task<DiscordWorkflowResult> SetRoomOwnerAsync(int serverId, ulong discordChannelId, int userId)
        {
            var activeMixSession = await _mixSessionManager.GetActiveMixSessionByServerIdAndDiscordChannelIdAsync(serverId, discordChannelId.ToString());
            if (activeMixSession == null)
                return new DiscordWorkflowResult(ErrorMessages.NoActiveSessionFoundInChannel.GetValueForLanguage(), false);

            if (activeMixSession.RoomOwnerId == userId)
                return new DiscordWorkflowResult(GeneralMessages.NothingChanged.GetValueForLanguage(), false);

            activeMixSession.RoomOwnerId = userId;
            var mixSessionToSave = Mapper.Map<MixSession>(activeMixSession);
            await _mixSessionManager.UpdateAsync(mixSessionToSave);
            
            var roomOwnerTeam = activeMixSession.MixTeams.FirstOrDefault(mt => !mt.PositionsLocked && mt.Formation.Any(mp => mp.Reservations.Any(r => r.DateEnd == null && r.UserId == userId)));
            if (roomOwnerTeam != null)
            {
                await HandleChangeOfCaptain(roomOwnerTeam, userId);
            }

            return new DiscordWorkflowResult(GeneralMessages.AppliedChanges.GetValueForLanguage(), true);
        }

        public async Task<DiscordWorkflowResult> SetCaptainAsync(int serverId, ulong discordChannelId, int userId)
        {
            var activeMixSession = await _mixSessionManager.GetActiveMixSessionByServerIdAndDiscordChannelIdAsync(serverId, discordChannelId.ToString());
            if (activeMixSession == null)
                return new DiscordWorkflowResult(ErrorMessages.NoActiveSessionFoundInChannel.GetValueForLanguage(), false);
            var notLockedTeams = activeMixSession.MixTeams.Where(mt => !mt.PositionsLocked).ToList();

            if (userId == activeMixSession.RoomOwnerId)
                return new DiscordWorkflowResult(GeneralMessages.NothingChanged.GetValueForLanguage(), false);

            var teamOfOwner = activeMixSession.RoomOwnerId.HasValue
                ? notLockedTeams.FirstOrDefault(mt => mt.Formation.Any(p => p.Reservations.Any(r => r.DateEnd == null && r.UserId == activeMixSession.RoomOwnerId)))
                : null;

            var userReservation = notLockedTeams.SelectMany(mt => mt.Formation.Select(f => f.Reservations.FirstOrDefault(r => r.DateEnd == null && r.UserId == userId))).FirstOrDefault(x => x != null);
            if (userReservation == null)
                return new DiscordWorkflowResult(ErrorMessages.CaptainNotSubscribed.GetValueForLanguage(), false);

            if (userReservation.IsCaptain)
                return new DiscordWorkflowResult(GeneralMessages.NothingChanged.GetValueForLanguage(), false);

            if (userReservation.MixPosition.MixTeamId == teamOfOwner?.MixTeamId)
                return new DiscordWorkflowResult(ErrorMessages.CaptainInOwnerTeam.GetValueForLanguage(), false);
            await HandleChangeOfCaptain(userReservation.MixPosition.MixTeam, userId);

            return new DiscordWorkflowResult(GeneralMessages.AppliedChanges.GetValueForLanguage(), true);
        }

        public async Task<DiscordWorkflowResult> SetLockedTeamPlayerCount(int serverId, ulong discordChannelId, int playerCount, string teamCode = null)
        {
            var activeMixSession = await _mixSessionManager.GetActiveMixSessionByServerIdAndDiscordChannelIdAsync(serverId, discordChannelId.ToString());
            if (activeMixSession == null)
                return new DiscordWorkflowResult(ErrorMessages.NoActiveSessionFoundInChannel.GetValueForLanguage(), false);
            MixTeam mixTeamToEdit = null;
            var lockedTeams = activeMixSession.MixTeams.Where(x => x.PositionsLocked).ToList();
            
            if (lockedTeams.Count() > 1)
            {
                teamCode = teamCode?.Trim();
                if (string.IsNullOrWhiteSpace(teamCode))
                    return new DiscordWorkflowResult(string.Format(ErrorMessages.PlayerCountWithMultipleClosedTeamsWithoutTeamCode.GetValueForLanguage(), Bot.Prefix, playerCount), false);

                mixTeamToEdit = lockedTeams.FirstOrDefault(mt => mt.HasNameOrTag(teamCode));
                if (mixTeamToEdit == null)
                    return new DiscordWorkflowResult($"{ErrorMessages.NoLockedTeamFoundForCode.GetValueForLanguage()}: {teamCode}", false);
            }
            if (lockedTeams.Count == 1)
                mixTeamToEdit = lockedTeams.Single();

            if (mixTeamToEdit == null)
                return new DiscordWorkflowResult(GeneralMessages.NothingChanged.GetValueForLanguage(), false);

            mixTeamToEdit.LockedTeamPlayerCount = playerCount;
            var mixTeamToSave = Mapper.Map<MixTeam>(mixTeamToEdit);
            await _mixTeamManager.UpdateAsync(mixTeamToSave);

            return new DiscordWorkflowResult(GeneralMessages.AppliedChanges.GetValueForLanguage(), true);
        }

        public async Task<DiscordWorkflowResult> CleanRoomAsync(int serverId, ulong discordChannelId, ulong requestedBy)
        {
            var notifyRequests = new List<NotifyRequest>();
            var applicationNow = DateTimeHelper.GetDatabaseNow();
            var activeMixSession = await _mixSessionManager.GetActiveMixSessionByServerIdAndDiscordChannelIdAsync(serverId, discordChannelId.ToString());
            if (activeMixSession == null)
                return new DiscordWorkflowResult(ErrorMessages.NoActiveSessionFoundInChannel.GetValueForLanguage(), false);
            var reservationsToUpdate = new List<MixPositionReservation>();

            activeMixSession.Password = null;
            activeMixSession.Server = null;
            activeMixSession.GameRoomName = null;
            activeMixSession.RoomOwnerId = null;
            activeMixSession.MixTeams.ForEach(mt =>
                mt.Formation.ForEach(mp =>
                    mp.Reservations.ForEach(r => {
                        r.DateEnd = applicationNow;
                        reservationsToUpdate.Add(r);
                    })
                )
            );
            var mixSessionToSave = Mapper.Map<MixSession>(activeMixSession);
            await _mixSessionManager.UpdateAsync(mixSessionToSave);
            foreach(var reservation in reservationsToUpdate)
            {
                try
                {
                    var userMember = reservation.User?.UserMembers?.FirstOrDefault();
                    if (userMember != null)
                    {
                        var userDiscordId = ulong.Parse(userMember.DiscordUserId);
                        if (userDiscordId != requestedBy)
                            notifyRequests.Add(new NotifyRequest(requestedBy, ulong.Parse(userMember.DiscordUserId), ulong.Parse(activeMixSession.MixChannel.DiscordChannelId), ulong.Parse(activeMixSession.MixChannel.MixGroup.Server.DiscordServerId)));
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Something went wrong while collecting info for the clean room notifications");
                }
                var reservationToUpdate = Mapper.Map<MixPositionReservation>(reservation);
                await _mixPositionReservationManager.UpdateAsync(reservationToUpdate);
            }

            return new DiscordWorkflowResult(GeneralMessages.AppliedChanges.GetValueForLanguage(), true, notifyRequests: notifyRequests);
        }

        public async Task<DiscordWorkflowResult> UpdatePositionAsync(ChangeMixSessionPositionDto dto)
        {
            var validator = new ChangeMixSessionPositionDtoValidator(_mixChannelManager, _positionManager, ErrorMessages);
            var validationResults = await validator.ValidateAsync(dto);
            if (validationResults.Errors.Any())
                return HandleValidationResults(validationResults);

            var dbNow = DateTimeHelper.GetDatabaseNow();

            var possiblePositions = await _mixPositionManager.GetActivePositionsByDiscordIdsAndTeamAndPositionAsync(dto.DiscordServerId.ToString(), dto.DiscordChannelId.ToString(), dto.Team, dto.OldPosition);
            
            if (!possiblePositions.Any())
                return new DiscordWorkflowResult("No position found to change", false);
            var positionToChange = possiblePositions.FirstOrDefault(p => p.Reservations.Any(r => r.UserId == dto.UserId)) ?? possiblePositions.FirstOrDefault();
            var newPosition = await _positionManager.GetPostionByTagOrCodeAsync(dto.NewPosition, dto.ServerId);
            if (newPosition == null)
                return new DiscordWorkflowResult(string.Format(ErrorMessages.NewPositionNotFound.GetValueForLanguage(), dto.NewPosition), false);

            if (positionToChange.PositionId == newPosition.PositionId)
                return new DiscordWorkflowResult(GeneralMessages.SwitchToSamePosition.GetValueForLanguage(), false);


            var reservation = positionToChange.Reservations.SingleOrDefault();
            if (reservation != null)
            {
                var reservationToEnd = Mapper.Map<MixPositionReservation>(reservation);
                await _mixPositionReservationManager.UpdateAsync(reservationToEnd);
            }

            //close the current position
            var positionToClose = Mapper.Map<MixPosition>(positionToChange);
            positionToClose.DateEnd = dbNow;
            await _mixPositionManager.UpdateAsync(positionToClose);

            var positionToOpen = Mapper.Map<MixPosition>(positionToChange);
            positionToOpen.DateStart = dbNow;
            positionToOpen.PositionId = newPosition.PositionId;
            positionToOpen.MixPositionId = 0;
            if (reservation != null)
            {
                var newReservation = Mapper.Map<MixPositionReservation>(reservation);
                newReservation.MixPositionReservationId = 0;
                newReservation.DateStart = dbNow;
                newReservation.MixPositionId = 0;
                positionToOpen.Reservations = new List<MixPositionReservation>() { newReservation };
            }

            await _mixPositionManager.AddAsync(positionToOpen);

            return new DiscordWorkflowResult(GeneralMessages.AppliedChanges.GetValueForLanguage(), true);
        }
        public async Task<DiscordWorkflowResult> SwapAsync(int serverId, ulong discordChannelId, int user1Id, int user2Id, ulong requestedBy, List<string> roleIdsPlayer1, List<string> roleIdsPlayer2, List<string> actorRoleIds)
        {
            var notifyRequests = new List<NotifyRequest>();
            var activeMixSession = await _mixSessionManager.GetActiveMixSessionByServerIdAndDiscordChannelIdAsync(serverId, discordChannelId.ToString());
            if (activeMixSession == null)
                return new DiscordWorkflowResult(ErrorMessages.NoActiveSessionFoundInChannel.GetValueForLanguage(), false);

            var reservations = activeMixSession.MixTeams.SelectMany(mt => mt.Formation.SelectMany(f => f.Reservations)).ToList();
            var player1Reservation = reservations.FirstOrDefault(r => r.UserId == user1Id);
            var player2Reservation = reservations.FirstOrDefault(r => r.UserId == user2Id);

            if (player1Reservation == null && player2Reservation == null)
                return new DiscordWorkflowResult(ErrorMessages.SwapInactiveUsersError.GetValueForLanguage(), false);

            if (player1Reservation != null && player2Reservation != null && !activeMixSession.MixChannel.MixGroup.Server.AllowActiveSwapCommand)
                return new DiscordWorkflowResult(ErrorMessages.SwapActiveUsersNotAllowedError.GetValueForLanguage(), false);

            if ((player1Reservation == null || player2Reservation == null) && !activeMixSession.MixChannel.MixGroup.Server.AllowInactiveSwapCommand)
                return new DiscordWorkflowResult(ErrorMessages.SwapInactiveUsersNotAllowedError.GetValueForLanguage(), false);

            if (player1Reservation == null || player2Reservation == null)
            {
                
                //validate roles of player without reservation
                var reservationToSignOut = player1Reservation ?? player2Reservation;
                var rolesToCheck = player1Reservation == null
                    ? roleIdsPlayer1
                    : roleIdsPlayer2;

                if (!await ValidateWithinValidHours(activeMixSession.MixChannel.MixGroup.Server.DiscordServerId, discordChannelId.ToString(), rolesToCheck))
                    return new DiscordWorkflowResult(ErrorMessages.RegistrationWithinValidHours.GetValueForLanguage(), false);

                var mixTeamRoleOpeningOfPlayerToSignOut = reservationToSignOut.MixPosition.MixTeam.MixTeamRoleOpenings.FirstOrDefault();
                if (mixTeamRoleOpeningOfPlayerToSignOut != null && !actorRoleIds.Contains(mixTeamRoleOpeningOfPlayerToSignOut.ServerRole.DiscordRoleId, StringComparer.InvariantCultureIgnoreCase))
                    return new DiscordWorkflowResult(ErrorMessages.RegistrationWithinValidHours.GetValueForLanguage(), false);

                var userIdToSignIn = player1Reservation == null
                    ? user1Id
                    : user2Id;
                await HandleSignOutAsync(reservationToSignOut, activeMixSession, DateTimeHelper.GetDatabaseNow(), true);

                var mixPostionReservation = new MixPositionReservation()
                {
                    MixPositionId = reservationToSignOut.MixPositionId,
                    UserId = userIdToSignIn,
                    DateStart = DateTimeHelper.GetDatabaseNow(),
                    IsCaptain = false,
                    DateEnd = null,
                    ExtraInfo = null
                };

                try
                {
                    var userMemberToSingOut = reservationToSignOut.User?.UserMembers?.FirstOrDefault();
                    if (userMemberToSingOut != null && ulong.TryParse(userMemberToSingOut.DiscordUserId, out ulong signOutDiscordId))
                    {
                        if (signOutDiscordId != requestedBy)
                            notifyRequests.Add(new NotifyRequest(requestedBy, signOutDiscordId, discordChannelId, ulong.Parse(activeMixSession.MixChannel.MixGroup.Server.DiscordServerId)));
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Something went wrong while gathering sign out notification info for a swap");
                }

                await _mixPositionReservationManager.AddAsync(mixPostionReservation);
            }

            if (player1Reservation != null && player2Reservation != null)
            {
                var dbNow = DateTimeHelper.GetDatabaseNow();

                if (player1Reservation.MixPosition.MixTeamId != player2Reservation.MixPosition.MixTeamId)
                {
                    var currentPlayer1TeamRoleOpening = player1Reservation.MixPosition.MixTeam.MixTeamRoleOpenings.FirstOrDefault();
                    var currentPlayer2TeamRoleOpening = player2Reservation.MixPosition.MixTeam.MixTeamRoleOpenings.FirstOrDefault();
                    if ((currentPlayer1TeamRoleOpening != null && !actorRoleIds.Contains(currentPlayer1TeamRoleOpening.ServerRole.DiscordRoleId, StringComparer.InvariantCultureIgnoreCase))
                        || currentPlayer2TeamRoleOpening != null && !actorRoleIds.Contains(currentPlayer2TeamRoleOpening.ServerRole.DiscordRoleId, StringComparer.InvariantCultureIgnoreCase))
                    {
                        return new DiscordWorkflowResult(ErrorMessages.RegistrationWithinValidHours.GetValueForLanguage(), false);
                    }
                }

                await HandleSignOutAsync(player1Reservation, activeMixSession, dbNow, false);
                await HandleSignOutAsync(player2Reservation, activeMixSession, dbNow, false);

                var ownerId = activeMixSession.RoomOwnerId;

                dbNow = DateTimeHelper.GetDatabaseNow();
                var user1ShouldBeCaptain = ShouldBeCaptain(user1Id, ownerId, player1Reservation.IsCaptain, player1Reservation.MixPosition.MixTeamId, player2Reservation.MixPosition.MixTeamId);
                var user2ShouldBeCaptain = ShouldBeCaptain(user2Id, ownerId, player2Reservation.IsCaptain, player2Reservation.MixPosition.MixTeamId, player1Reservation.MixPosition.MixTeamId);
        
                var mixPositionReservation1 = new MixPositionReservation()
                {
                    MixPositionId = player2Reservation.MixPositionId,
                    UserId = user1Id,
                    DateStart = dbNow,
                    IsCaptain = user1ShouldBeCaptain,
                    DateEnd = null,
                    ExtraInfo = GetExtraInfo(player1Reservation, player2Reservation)
                };
                var mixPositionReservation2 = new MixPositionReservation()
                {
                    MixPositionId = player1Reservation.MixPositionId,
                    UserId = user2Id,
                    DateStart = dbNow,
                    IsCaptain = user2ShouldBeCaptain,
                    DateEnd = null,
                    ExtraInfo = GetExtraInfo(player2Reservation, player1Reservation)
                };

                await _mixPositionReservationManager.AddAsync(mixPositionReservation1);
                await _mixPositionReservationManager.AddAsync(mixPositionReservation2);

                if (user1ShouldBeCaptain && player1Reservation.MixPosition.MixTeamId != player2Reservation.MixPosition.MixTeamId && !player2Reservation.IsCaptain)
                {
                    var currentCaptains = player2Reservation.MixPosition.MixTeam.Formation.SelectMany(mp => mp.Reservations.Where(r => r.DateEnd == null && r.IsCaptain == true)).ToList();
                    await CloseOtherCaptainReservations(currentCaptains, dbNow);
                }
                if (user2ShouldBeCaptain && player1Reservation.MixPosition.MixTeamId != player2Reservation.MixPosition.MixTeamId && !player1Reservation.IsCaptain)
                {
                    var currentCaptains = player1Reservation.MixPosition.MixTeam.Formation.SelectMany(mp => mp.Reservations.Where(r => r.DateEnd == null && r.IsCaptain == true)).ToList();
                    await CloseOtherCaptainReservations(currentCaptains, dbNow);
                }
            }

            return new DiscordWorkflowResult(GeneralMessages.AppliedChanges.GetValueForLanguage(), true);
        }

        public async Task<bool> ValidateWithinValidHours(string discordServerId, string discordChannelId, List<string> roleIds)
        {
            var time = DateTimeHelper.GetDatabaseNow();

            var mixGroupRegistrationOpening = await _mixSessionManager.HasOpenMixSessionByDiscordIds(discordServerId, discordChannelId, time);
            if (mixGroupRegistrationOpening == null)
                return false;

            var filteredMixGroupRoleOpenings = (await _mixGroupRoleOpeningWorkflow.GetMixGroupRoleOpeningsFromCacheOrDbAsync())
                ?.Where(m => m.MixGroupId == mixGroupRegistrationOpening.MixGroupId && roleIds.Contains(m.ServerRole.DiscordRoleId, StringComparer.InvariantCultureIgnoreCase));

            return filteredMixGroupRoleOpenings?.Any() ?? false
                ? filteredMixGroupRoleOpenings.All(op => mixGroupRegistrationOpening.RegistrationTime.AddMinutes(op.Minutes) < time)
                : mixGroupRegistrationOpening.RegistrationTime < time;
        }
        public async Task HandleNotificationsOfMixSessionsAsync()
        {
            var rangeStartTime = DateTimeHelper.GetDatabaseNow().AddMinutes(5);
            var rangeEndTime = rangeStartTime.AddMinutes(10);

            var activeSessionsThatWillStartWithinRange = await _mixSessionManager.GetMixSessionsThatWillStartBetweenDatesAsync(rangeStartTime, rangeEndTime);

            if (activeSessionsThatWillStartWithinRange.Any(ms => ms.MixTeams.Any(mt => mt.Formation.Any(mp => mp.Reservations.Any()))))
            {
                var counter = 0;
                var unAuthorizationErrorCount = 0;
                var serverExceptionCount = 0;
                var otherExceptionCount = 0;
                var discordClient = await _bot.GetConnectedDiscordClientAsync();
                foreach (var mixPositionReservationsGroup in activeSessionsThatWillStartWithinRange.SelectMany(ms => ms.MixTeams.SelectMany(mt => mt.Formation.SelectMany(mp => mp.Reservations))).GroupBy(r => r.MixPosition.MixTeam.MixSession.MixChannel.MixGroup.ServerId))
                {
                    try
                    {
                        var internalServerId = mixPositionReservationsGroup.Key;
                        var server = mixPositionReservationsGroup.First().MixPosition.MixTeam.MixSession.MixChannel.MixGroup.Server;
                        var discordServerId = ulong.Parse(server.DiscordServerId);
                        if (discordServerId > 0)
                        {
                            var serverGuild = await discordClient.GetGuildAsync(discordServerId);
                            foreach (var mixPositionReservation in mixPositionReservationsGroup)
                            {
                                counter++;
                                var mixChannel = mixPositionReservation.MixPosition.MixTeam.MixSession.MixChannel;
                                var userMember = mixPositionReservation.User.UserMembers.FirstOrDefault(um => um.ServerId == internalServerId);
                                if (userMember != null)
                                {
                                    try
                                    {
                                        var discordMember = await serverGuild.GetMemberAsync(ulong.Parse(userMember.DiscordUserId));
                                        await discordMember.SendMessageAsync(string.Format(GeneralMessages.MixWillStartNotification.GetValueForLanguage(), mixChannel.DiscordChannelId));
                                        Logger.LogInformation($"Send notification {counter} to UserMemberId: {userMember.UserMemberId}, {userMember.DiscordNickName}");
                                    }
                                    catch (ServerErrorException serverEx)
                                    {
                                        serverExceptionCount++;
                                        var message = string.Concat(serverEx.Message, "\nInnerException: ", serverEx.InnerException?.Message ?? string.Empty);
                                        Logger.LogError(serverEx, $"Failed to send notification {counter} (serverEx count: {serverExceptionCount}) to UserMemberId: {userMember.UserMemberId}, {userMember.DiscordNickName}\nServerException: {message}\nRestResponseCode: {serverEx.WebResponse?.ResponseCode}\nRestResponse: {serverEx.WebResponse?.Response ?? string.Empty}\nJsonMessage: {serverEx.JsonMessage ?? string.Empty}");
                                    }
                                    catch (UnauthorizedException authEx)
                                    {
                                        unAuthorizationErrorCount++;
                                        var message = string.Concat(authEx.Message, "\nInnerException: ", authEx.InnerException?.Message ?? string.Empty);
                                        Logger.LogError(authEx, $"Failed to send notification {counter} (unAuthorizedEx count: {unAuthorizationErrorCount}) to UserMemberId: {userMember.UserMemberId}, {userMember.DiscordNickName}\nUnauthorizedException: {message}\nRestResponseCode: {authEx.WebResponse?.ResponseCode}\nRestResponse: {authEx.WebResponse?.Response ?? string.Empty}\nJsonMessage: {authEx.JsonMessage ?? string.Empty}");
                                    }
                                    catch (Exception ex)
                                    {
                                        otherExceptionCount++;
                                        var message = string.Concat(ex.Message, "\nInnerException: ", ex.InnerException?.Message ?? string.Empty);
                                        Logger.LogError(ex, $"Failed to send notification {counter} (otherEx count: {otherExceptionCount}) to UserMemberId: {userMember.UserMemberId}, {userMember.DiscordNickName}\nException: {message}");
                                    }
                                    finally
                                    {
                                        await Task.Delay(TimeSpan.FromMilliseconds(200));
                                    }
                                }
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Logger.LogError(ex, $"Something went wrong while sending a notification to the user that the session will soon start. {ex.Message}");
                    }
                }
            }
        }

        public async Task<DiscordWorkflowResult> OpenTeamAsync(int serverId, ulong discordChannelId, ulong? discordRoleId, string roleName, int? minutes)
        {
            var activeMixSession = await _mixSessionManager.GetActiveMixSessionByServerIdAndDiscordChannelIdAsync(serverId, discordChannelId.ToString());
            if (activeMixSession == null)
                return new DiscordWorkflowResult(ErrorMessages.NoActiveSessionFoundInChannel.GetValueForLanguage(), false);

            if (activeMixSession.MixTeams.Where(mt => mt.PositionsLocked == true).Count() != 1)
                return new DiscordWorkflowResult(string.Format(ErrorMessages.OnlySessionsWith1ClosedTeamCanBeOpened.GetValueForLanguage(), activeMixSession.MixTeams.Where(mt => mt.PositionsLocked == false).Count()), false);

            var dbNow = DateTimeHelper.GetDatabaseNow();
            var openMixTeam = activeMixSession.MixTeams.First(mt => mt.PositionsLocked == false);

            var closedMixTeam = activeMixSession.MixTeams.First(mt => mt.PositionsLocked == true);
            closedMixTeam.PositionsLocked = false;
            closedMixTeam.LockedTeamPlayerCount = null;
            await _mixTeamManager.UpdateAsync(closedMixTeam); //ToDo Check
            var positionsToUpdate = new List<MixPosition>();
            foreach (var mixPosition in openMixTeam.Formation)
            {
                //clone the position and add this team Id to it and remove all the reservation that are on it.
                var mixPositionToAdd = Mapper.Map<MixPosition>(mixPosition);
                mixPositionToAdd.MixTeamId = closedMixTeam.MixTeamId;
                mixPositionToAdd.MixPositionId = 0;
                mixPositionToAdd.Reservations = new List<MixPositionReservation>();
                mixPositionToAdd.DateStart = dbNow;
                positionsToUpdate.Add(mixPositionToAdd);

            }
            await _mixPositionManager.AddMultipleAsync(positionsToUpdate);

            if (discordRoleId != null && minutes > 0 && !string.IsNullOrWhiteSpace(roleName))
            {
                var serverRole = await _serverRoleWorkflow.GetOrCreateServerRoleByDiscordRoleIdAndServerAsync(discordRoleId.Value, roleName, serverId);
                var mixTeamRoleOpening = new MixTeamRoleOpening()
                {
                    ServerRoleId = serverRole.ServerRoleId,
                    MixTeamId = closedMixTeam.MixTeamId,
                    Start = dbNow,
                    End = dbNow.AddMinutes(minutes.Value)
                };
                await _mixTeamRoleOpeningManager.AddAsync(mixTeamRoleOpening);
            }

            return new DiscordWorkflowResult(GeneralMessages.AppliedChanges.GetValueForLanguage(), true);
        }

        private DiscordWorkflowResult MapMixSessionWithRelatedDataToDiscordString(MixSession mixSession, string timeZone, List<MixGroupRoleOpening> roleOpeningsForMixGroup)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Bot.BotLine);
            sb.AppendLine();

            if (!string.IsNullOrWhiteSpace(mixSession.MixChannel.MixGroup.ExtraInfo))
                sb.AppendLine($"{mixSession.MixChannel.MixGroup.ExtraInfo}\n");

            var roomOwnerDiscordName = mixSession.RoomOwner?.ToGetPlatformDiscordSting(GeneralMessages);
            
            var registrationDateTime = DateTimeHelper.ConvertDateTimeToApplicationTimeZone(mixSession.DateRegistrationOpening, timeZone);
            var registrationTime = new Time(registrationDateTime);
            var startTime = new Time(DateTimeHelper.ConvertDateTimeToApplicationTimeZone(mixSession.DateStart, timeZone));
            var registrationTimeString = new StringBuilder(registrationTime.ToDiscordString());
            if (roleOpeningsForMixGroup.Any())
            {
                var message = new StringBuilder();
                foreach(var roleOpeningGrouping in roleOpeningsForMixGroup.GroupBy(ro => ro.Minutes))
                {
                    var roles = string.Join(" & ", roleOpeningGrouping.Select(x => x.ServerRole.Name));
                    var time = new Time(registrationDateTime.AddMinutes(roleOpeningGrouping.Key));
                    registrationTimeString.Append($" ({string.Join(" & ", roleOpeningGrouping.Select(x => x.ServerRole.Name))} {time.ToDiscordString()}) ");
                }
            }
            sb.AppendLine($":writing_hand: : {registrationTimeString}");
            sb.AppendLine($":stopwatch: : {startTime.ToDiscordString()}");
            sb.AppendLine();
            var roomName = string.IsNullOrWhiteSpace(mixSession.GameRoomName)
                ? string.Format(GeneralMessages.SetRoomNameCommand.GetValueForLanguage(), Bot.Prefix)
                : mixSession.GameRoomName;
            var owner = string.IsNullOrWhiteSpace(roomOwnerDiscordName)
                ? string.Format(GeneralMessages.SetOwnerCommand.GetValueForLanguage(), Bot.Prefix)
                : roomOwnerDiscordName;
            sb.AppendLine($":hotel: : {roomName}");
            sb.AppendLine($":crown: : {owner}");
            if (mixSession.MixChannel.MixGroup.Server.UseServerForSessions)
            {
                var serverName = string.IsNullOrWhiteSpace(mixSession.Server)
                ? string.Format(GeneralMessages.SetServerCommand.GetValueForLanguage(), Bot.Prefix)
                : mixSession.Server;
                sb.AppendLine($":desktop: **: {serverName}**");
            } 

            if (mixSession.MixChannel.MixGroup.Server.UsePasswordForSessions)
            {
                var password = string.IsNullOrWhiteSpace(mixSession.Password)
                    ? string.Format(GeneralMessages.SetPasswordCommand.GetValueForLanguage(), Bot.Prefix)
                    : mixSession.Password;
                sb.AppendLine($":closed_lock_with_key: **: {password}**");
            }

            var orderedMixTeams = mixSession.MixTeams.OrderBy(mt => mt.Tags.OrderBy(t => t.Tag).FirstOrDefault().Tag).ThenBy(mt => mt.Name).ToList();

            sb.AppendLine();
            var showPlayerCount = true;
            var totalPlayers = 0;
            foreach (var mixTeam in mixSession.MixTeams.OrderBy(mt => mt.Tags.OrderBy(t => t.Tag).FirstOrDefault().Tag).ThenBy(mt => mt.Name))
            {
                if (mixTeam.PositionsLocked && !mixTeam.LockedTeamPlayerCount.HasValue)
                    showPlayerCount = false;
                totalPlayers += mixTeam.PositionsLocked
                    ? mixTeam.LockedTeamPlayerCount ?? 0
                    : mixTeam.Formation.Count(x => x.Reservations.Any(r => r.DateEnd == null));

                var tagsString = mixTeam.Tags.Any() && !mixTeam.PositionsLocked
                    ? $" - {GeneralMessages.Aliases.GetValueForLanguage()}: {string.Join(", ", mixTeam.Tags.Select(t => t.Tag).OrderBy(t => t))}"
                    : "";

                var mixTreamRoleOpeningString = string.Empty;
                var mixTeamRoleOpening = mixTeam.MixTeamRoleOpenings.FirstOrDefault();
                if (mixTeamRoleOpening != null)
                {
                    var time = new Time(DateTimeHelper.ConvertDateTimeToApplicationTimeZone(mixTeamRoleOpening.End, timeZone));
                    mixTreamRoleOpeningString =$" **{string.Format(GeneralMessages.OnlyRoleCanSignInUntil.GetValueForLanguage(), mixTeamRoleOpening.ServerRole.Name, time.ToDiscordStringWithSeconds())}**";
                }
                sb.AppendLine(string.Format(Bot.TeamLine, $"**{mixTeam.Name}**{tagsString}{mixTreamRoleOpeningString}"));
                sb.AppendLine();
                if (mixTeam.PositionsLocked)
                {
                    var playerCountString = mixTeam.LockedTeamPlayerCount.HasValue && mixTeam.LockedTeamPlayerCount > 0
                        ? string.Format(GeneralMessages.ShowPlayerCount.GetValueForLanguage(), mixTeam.LockedTeamPlayerCount)
                        : mixSession.MixChannel.MixGroup.Server.ShowPESSideSelectionInfo
                            ? string.Format(GeneralMessages.ShowPESSideSelectionCommand.GetValueForLanguage(), Bot.Prefix)
                            : "";
                    sb.AppendLine($"{GeneralMessages.LockedTeamPCPart1.GetValueForLanguage()}{playerCountString}\n");
                }
                else
                {
                    var positionsGrouped = mixTeam.Formation.GroupBy(p => p.Position.PositionGroup.Order).OrderBy(g => g.Key);
                    foreach (var posGroup in positionsGrouped)
                    {
                        foreach (var pos in posGroup.OrderBy(p => p.Position.Order))
                        {
                            var reservation = pos.Reservations?.FirstOrDefault();
                            var icons = string.Empty;
                            if (reservation != null && reservation.UserId == mixSession.RoomOwnerId)
                                icons = DiscordEmoji.Owner;
                            if (reservation != null && reservation.IsCaptain)
                                icons += DiscordEmoji.Captain;

                            sb.AppendLine($"**{pos.Position.Display()}.** {icons}{reservation?.ToDiscordSting(GeneralMessages)}");
                        }
                        sb.AppendLine();
                    }
                }
            }
            if (showPlayerCount)
            {
                sb.AppendLine($"{totalPlayers} {GeneralMessages.PlayersSubscribed.GetValueForLanguage()}");
            }
            sb.Append(HandlePESSideSelectionInfo(mixSession, orderedMixTeams));

            sb.AppendLine(Bot.BotLine);

            return new DiscordWorkflowResult(sb.ToString(), true);
        }

        private string HandlePESSideSelectionInfo(MixSession mixSession, List<MixTeam> orderedMixTeams)
        {
            var sb = new StringBuilder();
            if (mixSession.MixChannel.MixGroup.Server.ShowPESSideSelectionInfo
                && mixSession.RoomOwnerId != null
                && orderedMixTeams.Where(x => x.PositionsLocked).All(mt => mt.LockedTeamPlayerCount != null && mt.LockedTeamPlayerCount > 0)
                && orderedMixTeams.Where(x => !x.PositionsLocked).All(mt => mt.Formation.Where(f => f.Reservations.Any(r => r.DateEnd == null)).Count() > 2))
            {
                sb.AppendLine();
                var firstSideTeam = orderedMixTeams.FirstOrDefault();
                var secondSideTeam = orderedMixTeams.Skip(1).FirstOrDefault();
                var roomOwnerId = mixSession.RoomOwnerId.Value;

                var firstSideCount = firstSideTeam.PositionsLocked
                    ? firstSideTeam.LockedTeamPlayerCount.Value
                    : firstSideTeam.Formation.Count(x => x.Reservations.Any(r => r.DateEnd == null));
                var secondSideCount = secondSideTeam.PositionsLocked
                    ? secondSideTeam.LockedTeamPlayerCount.Value
                    : secondSideTeam.Formation.Count(x => x.Reservations.Any(r => r.DateEnd == null));

                if (Math.Abs(firstSideCount - secondSideCount) > 1)
                {
                    if (firstSideCount >= secondSideCount)
                    {
                        sb.AppendLine($"**{firstSideTeam.Name} {GeneralMessages.All.GetValueForLanguage()} {Bot.Left}**");
                        sb.AppendLine($"**{secondSideTeam.Name} {GeneralMessages.AllMiddleExceptCaptain.GetValueForLanguage()} {Bot.Right} :{secondSideTeam.GetCaptainName()}**\n");
                    }
                    else
                    {
                        sb.AppendLine($"**{firstSideTeam.Name} {GeneralMessages.AllMiddleExceptCaptain.GetValueForLanguage()} {Bot.Right} :{firstSideTeam.GetCaptainName()}**");
                        sb.AppendLine($"**{secondSideTeam.Name} {GeneralMessages.All.GetValueForLanguage()} {Bot.Left}**\n");
                    }

                    return sb.ToString();
                }
                var firstTeamHasRoomOwner = !firstSideTeam.PositionsLocked && firstSideTeam.Formation.Any(f => f.Reservations.Any(r => r.DateEnd == null && r.UserId == roomOwnerId));
                var secondTeamHasRoomOwner = !firstTeamHasRoomOwner && !secondSideTeam.PositionsLocked && secondSideTeam.Formation.Any(f => f.Reservations.Any(r => r.DateEnd == null && r.UserId == roomOwnerId));
                if (!firstTeamHasRoomOwner && !secondTeamHasRoomOwner)
                {
                    firstTeamHasRoomOwner = firstSideTeam.PositionsLocked;
                    secondTeamHasRoomOwner = !firstTeamHasRoomOwner;
                }
                //9 vs 9 --> side selected in team order, and team of room owner should just follow him
                //10(owner) vs 9 --> team with 10 should all go left
                //10 vs 9(owner) --> team with 10 should stay middle except captain
                if (firstSideCount >= secondSideCount)
                {
                    if (firstTeamHasRoomOwner)
                    {
                        sb.AppendLine($"**{firstSideTeam.Name} {GeneralMessages.All.GetValueForLanguage()} {Bot.Left}**");
                        sb.AppendLine($"**{secondSideTeam.Name} {GeneralMessages.AllMiddleExceptCaptain.GetValueForLanguage()} {Bot.Right} :{secondSideTeam.GetCaptainName()}**\n");
                    }
                    else
                    {
                        sb.AppendLine($"**{firstSideTeam.Name} {GeneralMessages.AllMiddleExceptCaptain.GetValueForLanguage()} {Bot.Left} :{firstSideTeam.GetCaptainName()}**");
                        sb.AppendLine($"**{secondSideTeam.Name} {GeneralMessages.All.GetValueForLanguage()} {Bot.Right}**\n");
                    }
                    return sb.ToString();
                }

                //9(owner) vs 10 --> team with 9 should all go right
                //9 vs 10((owner) --> team with 10 should all go left
                if (firstSideCount < secondSideCount)
                {
                    if (firstTeamHasRoomOwner)
                    {
                        sb.AppendLine($"**{firstSideTeam.Name} {GeneralMessages.All.GetValueForLanguage()} {Bot.Right}**");
                        sb.AppendLine($"**{secondSideTeam.Name} {GeneralMessages.AllMiddleExceptCaptain.GetValueForLanguage()} {Bot.Left} :{secondSideTeam.GetCaptainName()}**\n");
                    }
                    else
                    {
                        sb.AppendLine($"**{firstSideTeam.Name} {GeneralMessages.AllMiddleExceptCaptain.GetValueForLanguage()} {Bot.Right} :{firstSideTeam.GetCaptainName()}**");
                        sb.AppendLine($"**{secondSideTeam.Name} {GeneralMessages.All.GetValueForLanguage()} {Bot.Left}**\n");
                    }
                    return sb.ToString();
                }

                return sb.ToString();
            }
            if (mixSession.MixChannel.MixGroup.Server.ShowPESSideSelectionInfo
                && orderedMixTeams.Where(x => !x.PositionsLocked).All(mt => mt.Formation.Where(f => f.Reservations.Any(r => r.DateEnd == null)).Count() > 2))
            {
                sb.AppendLine();
                var allTeamsLocked = mixSession.MixTeams.All(mt => mt.PositionsLocked);
                var fillInTeamInfoMessage = mixSession.MixTeams.Any(mt => mt.PositionsLocked)
                    ? $"{string.Format(GeneralMessages.SideSelectionPlayerCountCommand.GetValueForLanguage(), Bot.Prefix)}{(allTeamsLocked ? $" {GeneralMessages.TeamCode.GetValueForLanguage()}" : "")}**"
                    : string.Empty;
                sb.AppendLine($"{string.Format(GeneralMessages.ProvideInfoForBotSideSelection.GetValueForLanguage(), Bot.Prefix)}\n{fillInTeamInfoMessage}");
                return sb.ToString();
            }
            return sb.ToString();
        }



        private async Task HandleChangeOfCaptain(MixTeam userMixTeam, int userId)
        {
            var applicationNow = DateTimeHelper.GetDatabaseNow();
            var currentCaptains = userMixTeam.Formation.SelectMany(mp => mp.Reservations.Where(r => r.DateEnd == null && r.IsCaptain == true && r.UserId != userId)).ToList();
            await CloseOtherCaptainReservations(currentCaptains, applicationNow);

            var userReservation = userMixTeam.Formation.SelectMany(mp => mp.Reservations.Where(r => r.DateEnd == null && r.UserId == userId)).FirstOrDefault();
            var userReservationToClose = Mapper.Map<MixPositionReservation>(userReservation);
            userReservationToClose.DateEnd = applicationNow;
            await _mixPositionReservationManager.UpdateAsync(userReservationToClose);
            var userReservationToAdd = Mapper.Map<MixPositionReservation>(userReservation);
            userReservationToAdd.IsCaptain = true;
            userReservationToAdd.DateStart = applicationNow.AddMilliseconds(1);
            userReservationToAdd.MixPositionReservationId = 0;
            await _mixPositionReservationManager.AddAsync(userReservationToAdd);
        }

        private bool ShouldBeCaptain(int userId, int? ownerId, bool isCaptainNow, int currentOwnReservationTeamId, int currentOtherReservationTeamId)
        {
            return userId == ownerId || (isCaptainNow && currentOwnReservationTeamId == currentOtherReservationTeamId);
        }

        private string GetExtraInfo(MixPositionReservation currentReservation, MixPositionReservation currentOtherUserReservation)
        {
            return currentReservation.MixPosition.PositionId == currentOtherUserReservation.MixPosition.PositionId
                        || (currentReservation.MixPosition.Position.ParentPosition != null && currentReservation.MixPosition.Position.ParentPosition?.PositionId == currentOtherUserReservation.MixPosition.Position.ParentPosition?.PositionId)
                        ? currentReservation.ExtraInfo
                        : null;
        }

        private async Task CloseOtherCaptainReservations(List<MixPositionReservation> captainReservationsToClose, DateTime dbNow)
        {
            foreach (var currentCaptainUserReservation in captainReservationsToClose)
            {
                var captainReservationToClose = Mapper.Map<MixPositionReservation>(currentCaptainUserReservation);
                captainReservationToClose.DateEnd = dbNow;
                await _mixPositionReservationManager.UpdateAsync(captainReservationToClose);
                var oldCaptainReservationToAdd = Mapper.Map<MixPositionReservation>(currentCaptainUserReservation);
                oldCaptainReservationToAdd.IsCaptain = false;
                oldCaptainReservationToAdd.DateStart = dbNow.AddMilliseconds(1);
                oldCaptainReservationToAdd.MixPositionReservationId = 0;
                await _mixPositionReservationManager.AddAsync(oldCaptainReservationToAdd);
            }
        }
    }
}
