using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Business.Dto.Mix;
using WaPesLeague.Business.Workflows.Interfaces;
using WaPesLeague.Data.Managers.Interfaces;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;
using WaPesLeague.Data.Entities.Mix;
using WaPesLeague.Data.Entities.Position.Constants;

namespace WaPesLeague.Business.Workflows
{
    public class MixStatsWorkflow : BaseWorkflow<MixStatsWorkflow>, IMixStatsWorkflow
    {
        private readonly IMixPositionReservationManager _mixPositionReservationManager;
        private readonly IMixSessionManager _mixSessionManager;
        private readonly IPositionManager _positionManager;
        private readonly IPositionGroupManager _positionGroupManager;
        private readonly IMixUserPositionSessionStatManager _mixUserPositionSessionManager;

        public MixStatsWorkflow(IMixPositionReservationManager mixPositionReservationManager, IMixSessionManager mixSessionManager, IPositionManager positionManager,
            IPositionGroupManager positionGroupManager, IMixUserPositionSessionStatManager mixUserPositionSessionManager,
            IMemoryCache cache, IMapper mapper, ILogger<MixStatsWorkflow> logger, ErrorMessages errorMessages, GeneralMessages generalMessages)
            : base(cache, mapper, logger, errorMessages, generalMessages)
        {
            _mixPositionReservationManager = mixPositionReservationManager;
            _mixSessionManager = mixSessionManager;
            _positionManager = positionManager;
            _positionGroupManager = positionGroupManager;
            _mixUserPositionSessionManager = mixUserPositionSessionManager;
        }

        public async Task<IReadOnlyCollection<UserPositionGroupStatDto>> GetUserAdvancedStats(int userId, int? serverId = null)
        {
            var returnList = new List<UserPositionGroupStatDto>();
            var all = await GetMixUserPositionStatsFromCacheOrDb(serverId);
            var userStatsPerPositionGroup = all.Where(x => x.UserId == userId).GroupBy(x => x.PositionGroupId);
            var positionGroups = await _positionGroupManager.GetAllAsync();
            foreach (var userStatPositionGroup in userStatsPerPositionGroup)
            {
                var positionGroup = positionGroups.Single(x => x.PositionGroupId == userStatPositionGroup.Key);
                var minutesPlayed = userStatPositionGroup.Sum(x => x.PlayTimeSeconds) / 60d;
                returnList.Add(new UserPositionGroupStatDto(GetPositionGroupName(positionGroup.Name), positionGroup.Order, minutesPlayed, userStatPositionGroup.Count()));
            }

            if (!returnList.Any())
                returnList.Add(new UserPositionGroupStatDto(GeneralMessages.Finish1OrMoreSessions.GetValueForLanguage(), 0, 0));

            return returnList;
        }

        public async Task HandleCalculatedNotCalculatedSessions()
        {
            var mixSessions = await _mixSessionManager.GetSessionsToCalculateStatsAsync();
            if (!mixSessions.Any())
                return;
                

            var mixSessionIds = mixSessions.Select(ms => ms.MixSessionId).ToList();
            var reservations = await _mixPositionReservationManager.GetAllReservationsForSessionsAsync(mixSessionIds);
            var positions = await _positionManager.GetAllPositionsAsync(null);

            var allMappedReservations = MapAllReservations(reservations, positions, mixSessions);
            var mixSessionIdsResolved = new List<int>();

            var returnList = new List<UserPositionGroupStatDto>();

            var mixUsersPositionsSessionsStatsToSave = new List<MixUserPositionSessionStat>();
            var sessionIdsWithReservations = allMappedReservations?.Select(x => x.MixSessionId)?.Distinct().ToList() ?? new List<int>();
            var sessionsIdsWithoutReservations = mixSessionIds.Except(sessionIdsWithReservations).ToList();
            foreach (var sessionMappedReservations in allMappedReservations.GroupBy(mr => mr.MixSessionId))
            {
                try
                {
                    var mixSession = mixSessions.Single(s => s.MixSessionId == sessionMappedReservations.Key);
                    var listFactors = GetListFactorsForSession(mixSession);
                    var defaultMinumumPlayersRequired = listFactors.Count() == 1 ? (int?)listFactors.First().MinimumPlayersRequired : null;
                    if (sessionMappedReservations.Count() <= defaultMinumumPlayersRequired)
                    {
                        mixSessionIdsResolved.Add(sessionMappedReservations.Key);
                        continue;
                    }
                        

                    foreach (var userSessionMappedReservations in sessionMappedReservations.GroupBy(smr => smr.UserId))
                    {
                        try
                        {
                            var otherReservationsOfSession = allMappedReservations.Where(mr => mr.MixSessionId == sessionMappedReservations.Key && mr.UserId != userSessionMappedReservations.Key).ToList();
                            if (otherReservationsOfSession.Count < listFactors.Min(x => x.MinimumPlayersRequired))
                                continue;

                            foreach (var userSessionPositionGroupReservations in userSessionMappedReservations.GroupBy(it => it.PositionGroup))
                            {
                                var sessionMinutes = 0d;
                                DateTime? firstDate = null;
                                DateTime? secondDate = null;
                                var mixUserPositionSessionStatsReservations = new List<MixUserPositionSessionStat>();
                                foreach (var userReservation in userSessionPositionGroupReservations.OrderBy(x => x.CalculatedStartTime))
                                {
                                    var allReservationsActiveInPeriod = otherReservationsOfSession.Where(x => x.CalculatedStartTime < userReservation.CaluclatedEndTime && x.CaluclatedEndTime > userReservation.CalculatedStartTime).ToList();
                                    var dates = allReservationsActiveInPeriod.Select(x => x.CalculatedStartTime).Union(allReservationsActiveInPeriod.Select(x => x.CaluclatedEndTime)).Where(x => x > userReservation.CalculatedStartTime && x < userReservation.CaluclatedEndTime).ToList();
                                    dates.AddRange(new List<DateTime>() { userReservation.CalculatedStartTime, userReservation.CaluclatedEndTime });
                                    dates = dates.Distinct().OrderBy(x => x).ToList();
                                    var distinctDatesCount = dates.Count();
                                    var previousReservationCountAtDate = 0;
                                    var counter = 0;
                                    foreach (var date in dates)
                                    {
                                        var reservationCountForDate = allReservationsActiveInPeriod.Count(x => x.CalculatedStartTime <= date && x.CaluclatedEndTime >= date);
                                        var minimumRequired = defaultMinumumPlayersRequired ?? listFactors.FirstOrDefault(x => x.DateStart <= date).MinimumPlayersRequired;
                                        if ((previousReservationCountAtDate < minimumRequired && reservationCountForDate >= minimumRequired)
                                            || (previousReservationCountAtDate >= minimumRequired && reservationCountForDate < minimumRequired))
                                        {
                                            if (firstDate == null)
                                                firstDate = date;
                                            else
                                                secondDate = date;
                                        }

                                        if (++counter == distinctDatesCount && firstDate.HasValue) //this will add the last date as second date if there is an open interval.
                                            secondDate = date;

                                        if (secondDate != null)
                                        {
                                            var difference = secondDate.Value.Subtract(firstDate.Value);
                                            mixUserPositionSessionStatsReservations.Add(new MixUserPositionSessionStat()
                                            {
                                                UserId = userSessionMappedReservations.Key,
                                                MixSessionId = sessionMappedReservations.Key,
                                                PositionId = userReservation.PositionId,
                                                PlayTimeSeconds = (int)(difference.TotalSeconds - (difference.TotalSeconds % 1))
                                            });
                                            sessionMinutes += difference.TotalMinutes;
                                            firstDate = null;
                                            secondDate = null;
                                        }
                                        previousReservationCountAtDate = reservationCountForDate;
                                    }
                                }

                                if (sessionMinutes >= 15d)
                                    mixUsersPositionsSessionsStatsToSave.AddRange(mixUserPositionSessionStatsReservations);

                                sessionMinutes = 0d;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex, $"Something went wrong while calculating stats for mixSession: {sessionMappedReservations.Key} of userId: {userSessionMappedReservations.Key}");
                            throw;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Something went wrong while calculating stats for mixSession: {sessionMappedReservations.Key}");
                }
                mixSessionIdsResolved.Add(sessionMappedReservations.Key);
            }

            mixSessionIdsResolved.AddRange(sessionsIdsWithoutReservations);
            await _mixUserPositionSessionManager.AddMultipleAsync(mixUsersPositionsSessionsStatsToSave);
            await _mixSessionManager.SetStatsCalculatedDateForMixSessionsAsync(mixSessionIdsResolved.Distinct());
        }


        private async Task<List<UserStatInfoLineDto>> GetMixUserPositionStatsFromCacheOrDb(int? serverId = null)
        {
            if (!MemoryCache.TryGetValue(Cache.AllUserStatInfoLines, out List<UserStatInfoLineDto> cacheEntry))
            {
                var MixUserPositionSessionStats = await _mixUserPositionSessionManager.GetAllAsync(); //this will get all userStatInfoLines
                var mixSessionIdsWithServerId = await _mixSessionManager.GetAllSessionIdsWithServerId();
                var positions = await _positionManager.GetAllPositionsAsync(null);

                cacheEntry = MixUserPositionSessionStats.Select(s =>
                    new UserStatInfoLineDto(s.UserId, positions.First(p => p.PositionId == s.PositionId).PositionGroupId, mixSessionIdsWithServerId[s.MixSessionId], s.PlayTimeSeconds, s.MixSessionId))
                    .ToList();

                MemoryCache.Set(Cache.AllUserStatInfoLines, cacheEntry, TimeSpan.FromHours(4));
            }

            if (serverId.HasValue)
                return cacheEntry.Where(x => x.ServerId == serverId).ToList();
            return cacheEntry;
        }

        private List<MixPositionReservationTimeCalcDto> MapAllReservations(IReadOnlyCollection<MixPositionReservation> reservations, IReadOnlyCollection<Data.Entities.Position.Position> positions, IReadOnlyCollection<MixSession> sessions)
        {
            var reservationsCalcDtos = new List<MixPositionReservationTimeCalcDto>();

            foreach (var resSessionGrouping in reservations.GroupBy(x => x.MixPosition.MixTeam.MixSessionId))
            {
                var mixSession = sessions.SingleOrDefault(ms => ms.MixSessionId == resSessionGrouping.Key);
                if (mixSession == null)
                {
                    Logger.LogError($"MixStatsCalc: No session found for sessionId: {resSessionGrouping.Key}");
                    continue;
                }
                foreach (var resSessionPositionGrouping in resSessionGrouping.Where(r => r.DateEnd > mixSession.DateStart).GroupBy(rsg => rsg.MixPosition.PositionId))
                {
                    var position = positions.SingleOrDefault(p => p.PositionId == resSessionPositionGrouping.Key);
                    if (position == null)
                    {
                        Logger.LogError($"MixStatsCalc: No position found for positionId: {resSessionPositionGrouping.Key}");
                        continue;
                    }
                    foreach (var res in resSessionPositionGrouping)
                    {
                        reservationsCalcDtos.Add(new MixPositionReservationTimeCalcDto()
                        {
                            ReservationId = res.MixPositionReservationId,
                            MixSessionId = resSessionGrouping.Key,
                            UserId = res.UserId,
                            PositionId = position.PositionId,
                            PositionGroup = position.PositionGroup.Name,
                            PositionGroupOrder = position.PositionGroup.Order,
                            CalculatedStartTime = new[] { mixSession.DateStart, res.DateStart }.Max(),
                            CaluclatedEndTime = new[] { res.DateEnd ?? DateTime.MaxValue, res.MixPosition.DateEnd ?? DateTime.MaxValue, mixSession.DateToClose, mixSession.DateClosed ?? DateTime.MaxValue }.Min()

                        });
                    }
                }
            }


            return reservationsCalcDtos;
        }

        private List<MinimumPlayersRequiredTimeFrameDto> GetListFactorsForSession(MixSession mixSession)
        {
            var listFactors = new List<MinimumPlayersRequiredTimeFrameDto>();
            if (mixSession.MixTeams.Any(mt => mt.PositionsLocked))
                listFactors.Add(new MinimumPlayersRequiredTimeFrameDto(1, mixSession.DateStart, mixSession.DateToClose));
            else
            {
                var startTeamOne = mixSession.MixTeams.First().Formation.OrderBy(p => p.DateStart).FirstOrDefault()?.DateStart ?? DateTime.MinValue;
                var startTeamTwo = mixSession.MixTeams.First().Formation.OrderBy(p => p.DateStart).Skip(1).FirstOrDefault()?.DateStart ?? DateTime.MinValue;
                if (startTeamOne != startTeamTwo)
                {
                    var maxDate = startTeamOne > startTeamTwo ? startTeamOne : startTeamTwo;
                    var minDate = startTeamOne < startTeamTwo ? startTeamOne : startTeamTwo;
                    listFactors.Add(new MinimumPlayersRequiredTimeFrameDto(1, mixSession.DateStart, maxDate));
                    listFactors.Add(new MinimumPlayersRequiredTimeFrameDto(2, maxDate, mixSession.DateToClose));
                }
                else
                    listFactors.Add(new MinimumPlayersRequiredTimeFrameDto(2, mixSession.DateStart, mixSession.DateToClose));
            }

            return listFactors;
        }

        private string GetPositionGroupName(string positionGroupName) => positionGroupName switch
        {
            PositionConstants.GroupNames.Goalkeeper => GeneralMessages.GoalKeeperGroup.GetValueForLanguage(),
            PositionConstants.GroupNames.Defenders => GeneralMessages.DefendersGroup.GetValueForLanguage(),
            PositionConstants.GroupNames.Midfielders => GeneralMessages.MidfieldersGroup.GetValueForLanguage(),
            PositionConstants.GroupNames.Attackers => GeneralMessages.AttackersGroup.GetValueForLanguage(),
            _ => throw new ArgumentOutOfRangeException(positionGroupName, $"no translation found for positionGroup: '{positionGroupName}'")
        };
    }
}
