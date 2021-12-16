using AutoMapper;
using Base.Bot.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Business.Dto.Server;
using WaPesLeague.Business.Helpers;
using WaPesLeague.Business.Workflows.Interfaces;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;
using WaPesLeague.Data.Entities.Discord;
using WaPesLeague.Data.Entities.Formation;
using WaPesLeague.Data.Helpers;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Business.Workflows
{
    public class ServerWorkflow : BaseWorkflow<ServerWorkflow>, IServerWorkflow
    {
        private readonly IServerManager _serverManager;
        private readonly IServerTeamManager _serverTeamManager;
        private readonly IFormationManager _formationManager;
        private readonly IMixUserPositionSessionStatManager _mixUserPositionSessionStatManager;
        private readonly IUserMemberManager _userMemberManager;
        private readonly IServerSnipingManager _serverSnipingManager;
        private readonly Base.Bot.Bot _bot;
        private readonly DefaultServerSettings _defaultServerSettings;

        public ServerWorkflow(IServerManager serverManager, IServerTeamManager serverTeamManager, IFormationManager formationManager, IMixUserPositionSessionStatManager mixUserPositionSessionStatManager, IUserMemberManager userMemberManager, IServerSnipingManager serverSnipingManager,
            Base.Bot.Bot bot, DefaultServerSettings defaultServerSettings, IMemoryCache cache, IMapper mapper, ILogger<ServerWorkflow> logger
            , ErrorMessages errorMessages, GeneralMessages generalMessages)
            : base (cache, mapper, logger, errorMessages, generalMessages)
        {
            _serverManager = serverManager;
            _serverTeamManager = serverTeamManager;
            _formationManager = formationManager;
            _mixUserPositionSessionStatManager = mixUserPositionSessionStatManager;
            _userMemberManager = userMemberManager;
            _serverSnipingManager = serverSnipingManager;
            _bot = bot;
            _defaultServerSettings = defaultServerSettings;
        }

        public async Task<Server> GetOrCreateServerAsync(ulong discordServerId, string discordServerName)
        {
            var server = await GetServerFromCacheOrDbAsync(discordServerId, discordServerName);
            if (server == null)
            {
                server = await CreateServerAsync(discordServerId, discordServerName);
                MemoryCache.Set(Cache.GetServerId(discordServerId).ToUpperInvariant(), server, TimeSpan.FromMinutes(1430));
            }    

            return server;
        }

        public async Task<DiscordWorkflowResult> UpdateAsync(UpdateServerSettingsDto updateServerSettingsDto)
        {
            var server = await GetOrCreateServerAsync(updateServerSettingsDto.DiscordCommandPropsDto.ServerId, updateServerSettingsDto.DiscordCommandPropsDto.ServerName);
            if (ServerValuesAreEqual(server, updateServerSettingsDto))
                return new DiscordWorkflowResult(GeneralMessages.NothingChanged.GetValueForLanguage(), false);

            if (!string.Equals(server.TimeZoneName, updateServerSettingsDto.TimeZoneName, StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    var timeZone = TimeZoneInfo.FindSystemTimeZoneById(updateServerSettingsDto.TimeZoneName);
                    if (timeZone == null)
                        throw new ArgumentException(ErrorMessages.NoTimeZoneFound.GetValueForLanguage());
                }
                catch (Exception)
                {
                    return new DiscordWorkflowResult($"{ErrorMessages.NoTimeZoneFoundForCode.GetValueForLanguage()}: {updateServerSettingsDto.TimeZoneName}", false);
                }
            }

            if (!string.Equals(server.Language, updateServerSettingsDto.Language, StringComparison.InvariantCultureIgnoreCase) &&
                !Bot.SupportedLanguages.GetAll().Any(x => string.Equals(x, updateServerSettingsDto.Language, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ArgumentException($"Language {updateServerSettingsDto.Language} is not supported\nSupportedLanguages: {Bot.SupportedLanguages.ReadableListOfLanguages()}");
            }

            var teamA = server.DefaultTeams.First(mt => mt.Tags.Any(t => string.Equals(t.Tag, "A", StringComparison.InvariantCultureIgnoreCase)));
            var teamB = server.DefaultTeams.First(mt => mt.Tags.Any(t => string.Equals(t.Tag, "B", StringComparison.InvariantCultureIgnoreCase)));

            await UpdateServerTeamIfNeededAsync(teamA, updateServerSettingsDto.DefaultTeamAName, updateServerSettingsDto.DefaultTeamAOpen);
            await UpdateServerTeamIfNeededAsync(teamB, updateServerSettingsDto.DefaultTeamBName, updateServerSettingsDto.DefaultTeamBOpen);

            if (!ServerPrimaryAttribuesAreEqual(server, updateServerSettingsDto))
            {
                server.DefaultSessionRecurringWithAClosedTeam = updateServerSettingsDto.DefaultSessionRecurringWithAClosedTeam;
                server.DefaultSessionRecurringWithAllOpen = updateServerSettingsDto.DefaultSessionRecurringWithAllOpen;
                server.DefaultAutoCreateExtraSessionsWhenAllTeamsOpen = updateServerSettingsDto.DefaultAutoCreateExtraSessionsWhenAllTeamsOpen;
                server.DefaultAutoCreateExtraSessionsWithAClosedTeam = updateServerSettingsDto.DefaultAutoCreateExtraSessionsWithAClosedTeam;
                server.DefaultStartTime = updateServerSettingsDto.DefaultStartTime;
                server.DefaultHoursToOpenRegistrationBeforeStart = updateServerSettingsDto.DefaultHoursToOpenRegistrationBeforeStart;
                server.DefaultSessionDuration = updateServerSettingsDto.DefaultSessionDuration;
                server.UsePasswordForSessions = updateServerSettingsDto.UsePasswordForSessions;
                server.UseServerForSessions = updateServerSettingsDto.UseServerForSessions;
                server.ShowPESSideSelectionInfo = updateServerSettingsDto.ShowPESSideSelectionInfo;
                server.DefaultSessionExtraInfo = updateServerSettingsDto.DefaultSessionExtraInfo;
                server.DefaultSessionPassword = updateServerSettingsDto.DefaultSessionPassword;
                server.TimeZoneName = updateServerSettingsDto.TimeZoneName;
                server.Language = updateServerSettingsDto.Language?.ToUpper() ?? Bot.SupportedLanguages.English;
                server.AllowActiveSwapCommand = updateServerSettingsDto.AllowActiveSwapCommand;
                server.AllowInactiveSwapCommand = updateServerSettingsDto.AllowInactiveSwapCommand;

                var mappedServer = Mapper.Map<Server>(server);
                await _serverManager.UpdateAsync(mappedServer);
            }

            var updatedServer = await _serverManager.GetServerByDiscordIdAsync(updateServerSettingsDto.DiscordCommandPropsDto.ServerId.ToString());
            MemoryCache.Set(Cache.GetServerId(updateServerSettingsDto.DiscordCommandPropsDto.ServerId).ToUpperInvariant(), updatedServer, TimeSpan.FromMinutes(1430));

            return new DiscordWorkflowResult(GeneralMessages.AppliedChanges.GetValueForLanguage(), true, new FollowUpParameters() { Server = updatedServer });
        }

        public async Task<DiscordWorkflowResult> GetTimeAsync(ulong discordServerId, string discordServerName)
        {
            var server = await GetServerFromCacheOrDbAsync(discordServerId, discordServerName);
            var time = new Time(DateTimeHelper.GetNowForApplicationTimeZone(server.TimeZoneName));

            return new DiscordWorkflowResult(time.ToDiscordString(), true);
        }

        public async Task HandleServerEventsAndActionsAsync()
        {
            var servers = await _serverManager.GetServersAsync();
            await HandlePlayedAmountOfSessionEventsAsync(servers); //When more events are added return some values so they can be reused in other methods aswell (DiscordClient for example etc)
            
        }

        public async Task UpdateServerCacheValueAsync(ulong discordServerId)
        {
            var cacheEntry = await _serverManager.GetServerByDiscordIdAsync(discordServerId.ToString());
            MemoryCache.Set(Cache.GetServerId(discordServerId).ToUpperInvariant(), cacheEntry, TimeSpan.FromMinutes(1430));
        }

        private async Task UpdateServerTeamIfNeededAsync(ServerTeam team, string name, bool isOpen)
        {
            if (!string.Equals(team.Name, name, StringComparison.InvariantCultureIgnoreCase) || team.IsOpen != isOpen)
            {
                var teamToUpdate = Mapper.Map<ServerTeam>(team);
                teamToUpdate.IsOpen = isOpen;
                teamToUpdate.Name = name;
                await _serverTeamManager.UpdateAsync(teamToUpdate);
            }
        }
        private async Task<Server> CreateServerAsync(ulong discordServerId, string discordServerName)
        {
            var formations = await _formationManager.GetAllFormationsAsync();
            var teams = _defaultServerSettings.Teams.Select(t => new ServerTeam()
            {
                Name = t.Name,
                IsOpen = t.IsOpen,
                Tags = t.Tags.Select(tt => new ServerTeamTag()
                {
                    Tag = tt
                }).ToList()
            }).ToList();

            var serverFormations = formations.Select(f => new ServerFormation()
            {
                Name = f.Name,
                IsDefault = f.IsDefault,
                Tags = f.Tags.Select(ft => new ServerFormationTag()
                {
                    Tag = ft.Tag
                }).ToList(),
                Positions = f.FormationPositions.Select(fp => new ServerFormationPosition()
                {
                    PositionId = fp.PositionId
                }).ToList()
            }).ToList();
            var server = new Server()
            {
                DiscordServerId = discordServerId.ToString(),
                ServerName = discordServerName.ToString(),
                IsActive = true,
                DefaultSessionRecurringWithAClosedTeam = _defaultServerSettings.DefaultSessionRecurringWithAClosedTeam,
                DefaultSessionRecurringWithAllOpen = _defaultServerSettings.DefaultSessionRecurringWithAllOpen,
                DefaultAutoCreateExtraSessionsWhenAllTeamsOpen = _defaultServerSettings.DefaultAutoCreateExtraSessionsWhenAllTeamsOpen,
                DefaultAutoCreateExtraSessionsWithAClosedTeam = _defaultServerSettings.DefaultAutoCreateExtraSessionsWithAClosedTeam,
                DefaultStartTime = _defaultServerSettings.DefaultStartTime,
                DefaultHoursToOpenRegistrationBeforeStart = _defaultServerSettings.DefaultHoursToOpenRegistrationBeforeStart,
                DefaultSessionDuration = _defaultServerSettings.DefaultSessionDuration,
                DefaultSessionExtraInfo = _defaultServerSettings.DefaultSessionExtraInfo,
                DefaultSessionPassword = "",
                UsePasswordForSessions = _defaultServerSettings.DefaultUsePasswordForSessions,
                UseServerForSessions = _defaultServerSettings.DefaultUseServerForSessions,
                ShowPESSideSelectionInfo = _defaultServerSettings.DefaultShowPESSideSelectionInfo,
                Language = Bot.SupportedLanguages.English,
                TimeZoneName = _defaultServerSettings.TimeZoneName,
                AllowActiveSwapCommand = _defaultServerSettings.AllowActiveSwapCommand,
                AllowInactiveSwapCommand = _defaultServerSettings.AllowInactiveSwapCommand,
                DefaultTeams = teams,
                ServerFormations = serverFormations
            };
            server = await _serverManager.AddAsync(server);

            return server;
        }

        private async Task<Server> GetServerFromCacheOrDbAsync(ulong discordServerId, string serverName)
        {
            if (!MemoryCache.TryGetValue(Cache.GetServerId(discordServerId).ToUpperInvariant(), out Server cacheEntry))
            {
                cacheEntry = await _serverManager.GetServerByDiscordIdAsync(discordServerId.ToString());
                if (cacheEntry != null)
                {
                    if (!string.Equals(cacheEntry.ServerName, serverName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        cacheEntry.ServerName = serverName;
                        await _serverManager.UpdateAsync(Mapper.Map<Server>(cacheEntry));
                    }
                    MemoryCache.Set(Cache.GetServerId(discordServerId).ToUpperInvariant(), cacheEntry, TimeSpan.FromMinutes(1430));
                } 
            }

            return cacheEntry;
        }

        private bool ServerValuesAreEqual(Server server, UpdateServerSettingsDto dto)
        {
            var teamA = server.DefaultTeams.First(mt => mt.Tags.Any(t => string.Equals(t.Tag, "A", StringComparison.InvariantCultureIgnoreCase)));
            var teamB = server.DefaultTeams.First(mt => mt.Tags.Any(t => string.Equals(t.Tag, "B", StringComparison.InvariantCultureIgnoreCase)));

            return
                ServerPrimaryAttribuesAreEqual(server, dto)
                && string.Equals(teamA.Name, dto.DefaultTeamAName, StringComparison.InvariantCultureIgnoreCase)
                && string.Equals(teamB.Name, dto.DefaultTeamBName, StringComparison.InvariantCultureIgnoreCase)
                && teamA.IsOpen == dto.DefaultTeamAOpen
                && teamB.IsOpen == dto.DefaultTeamBOpen;
        }

        private bool ServerPrimaryAttribuesAreEqual(Server server, UpdateServerSettingsDto dto)
        {
            return server.DefaultSessionRecurringWithAClosedTeam == dto.DefaultSessionRecurringWithAClosedTeam
                && server.DefaultSessionRecurringWithAllOpen == dto.DefaultSessionRecurringWithAllOpen
                && server.DefaultAutoCreateExtraSessionsWhenAllTeamsOpen == dto.DefaultAutoCreateExtraSessionsWhenAllTeamsOpen
                && server.DefaultAutoCreateExtraSessionsWithAClosedTeam == dto.DefaultAutoCreateExtraSessionsWithAClosedTeam
                && server.DefaultStartTime == dto.DefaultStartTime
                && server.DefaultHoursToOpenRegistrationBeforeStart == dto.DefaultHoursToOpenRegistrationBeforeStart
                && server.DefaultSessionDuration == dto.DefaultSessionDuration
                && server.UsePasswordForSessions == dto.UsePasswordForSessions
                && server.UseServerForSessions == dto.UseServerForSessions
                && server.ShowPESSideSelectionInfo == dto.ShowPESSideSelectionInfo
                && server.AllowActiveSwapCommand == dto.AllowActiveSwapCommand
                && server.AllowInactiveSwapCommand == dto.AllowInactiveSwapCommand
                && string.Equals(server.DefaultSessionExtraInfo, dto.DefaultSessionExtraInfo, StringComparison.InvariantCultureIgnoreCase)
                && string.Equals(server.DefaultSessionPassword, dto.DefaultSessionPassword, StringComparison.InvariantCultureIgnoreCase)
                && string.Equals(server.TimeZoneName, dto.TimeZoneName, StringComparison.InvariantCultureIgnoreCase)
                && string.Equals(server.Language, dto.Language, StringComparison.InvariantCultureIgnoreCase);
        }

        private async Task HandlePlayedAmountOfSessionEventsAsync(IReadOnlyCollection<Server> servers)
        {
            try
            {
                foreach (var server in servers.Where(s => s.ServerEvents.Any(se => se.EventType == Data.Entities.Discord.Enums.EventType.PlayedAmountOfSessions)))
                {
                    var dictionary = await GetPlayedAmountOfSessionEventWithUserIdsDictionaryAsync(server);

                    if (dictionary.Any())
                    {
                        var discordClient = await _bot.GetConnectedDiscordClientAsync();
                        var guild = await discordClient.GetGuildAsync(ulong.Parse(server.DiscordServerId));
                        foreach (var dictItem in dictionary)
                        {
                            switch (dictItem.Key.ActionEntity)
                            {
                                case Data.Entities.Discord.Enums.ActionEntity.Role:
                                    if (ulong.TryParse(dictItem.Key.ActionValue, out var actionValue))
                                    {
                                        var role = guild.Roles?.FirstOrDefault(x => x.Value.Id == actionValue).Value ?? null;
                                        if (role != null)
                                        {
                                            var userMembers = await _userMemberManager.GetUserMembersByUserIdsAndServerId(dictItem.Value, dictItem.Key.ServerId);
                                            var userMembersDiscordIds = userMembers.Select(x => ulong.Parse(x.DiscordUserId)).ToList();
                                            var members = (await guild.GetAllMembersAsync()).Where(m => userMembersDiscordIds.Contains(m.Id) && m.Roles.Any(r => r.Id == role.Id)).ToList();
                                            if (members.Any())
                                            {
                                                switch (dictItem.Key.ActionType)
                                                {
                                                    case Data.Entities.Discord.Enums.ActionType.Remove:
                                                        Logger.LogWarning($"Removing role for {members.Count} members, ServerEventId: {dictItem.Key.ServerEventId}");
                                                        foreach (var member in members)
                                                        {
                                                            await member.RevokeRoleAsync(role);
                                                            Logger.LogWarning($"Removed role for {member.Id}, ServerEventId: {dictItem.Key.ServerEventId}");
                                                        }
                                                        break;
                                                    default:
                                                        Logger.LogError($"ActionType for serverEvent: {dictItem.Key.ServerEventId} is not supported");
                                                        break;
                                                }
                                            }
                                        }
                                        else
                                            Logger.LogWarning($"No role found on server for serverEventId: {dictItem.Key.ServerEventId}");
                                    }
                                    else
                                        Logger.LogError($"Failed to Parse ActionValue of serverEvent: {dictItem.Key.ServerEventId}");

                                    break;
                                default:
                                    Logger.LogError($"Action Entity for serverEvent: {dictItem.Key.ServerEventId} is not supported");
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Something went wrong while handling HandlePlayedAmountOfSessionEventsAsync");
            }
        }

        private async Task<Dictionary<ServerEvent, List<int>>> GetPlayedAmountOfSessionEventWithUserIdsDictionaryAsync(Server server)
        {
            var dict = new Dictionary<ServerEvent, List<int>>();
            var userIdsAndSessionAmount = await _mixUserPositionSessionStatManager.GetUserIdsWithSessionAmountAsync(server.ServerId);
            foreach (var serverEvent in server.ServerEvents.Where(se => se.EventType == Data.Entities.Discord.Enums.EventType.PlayedAmountOfSessions))
            {
                if (int.TryParse(serverEvent.EventValue, out var value))
                {
                    var userIds = userIdsAndSessionAmount
                        .Where(x => x.SessionAmount >= value && x.SessionAmount <= value + 4)
                        .Select(x => x.UserId)
                        .ToList();

                    if (userIds.Any())
                        dict.Add(serverEvent, userIds);
                }
                else
                    Logger.LogError($"Failed to Parse Eventvalue of serverEvent: {serverEvent.ServerEventId}");
            }

            return dict;
        }

        public async Task<DiscordWorkflowResult> SetSnipingAsync(Server server, int intervalAfterRegistrationOpeningInMinutes, int signUpDelayInMinutes, int signUpDelayDurationInHours)
        {
            var currentServerSniping = server.ServerSnipings.FirstOrDefault();

            var hasCorrectValues = intervalAfterRegistrationOpeningInMinutes != 0 && signUpDelayInMinutes != 0 && signUpDelayDurationInHours != 0;
            var hasDeleteValues = intervalAfterRegistrationOpeningInMinutes == 0 && signUpDelayInMinutes == 0 && signUpDelayDurationInHours == 0;

            if (!hasCorrectValues && !hasDeleteValues)
            {
                return new DiscordWorkflowResult(ErrorMessages.InvalidSnipingValues.GetValueForLanguage(), false);
            }

            var changesMade = false;
            if (currentServerSniping == null && hasCorrectValues)
            {
                await _serverSnipingManager.AddAsync(new ServerSniping()
                {
                    ServerId = server.ServerId,
                    Enabled = true,
                    IntervalAfterRegistrationOpeningInMinutes = intervalAfterRegistrationOpeningInMinutes,
                    SignUpDelayInMinutes = signUpDelayInMinutes,
                    SignUpDelayDurationInHours = signUpDelayDurationInHours
                });
                changesMade = true;
            }

            if (currentServerSniping != null && hasDeleteValues)
            {
                //Delete ServerSnipings Related to this one
                await _serverSnipingManager.DeActivateAsync(currentServerSniping.ServerSnipingId);
                changesMade = true;
            }

            if (currentServerSniping != null && hasCorrectValues)
            {
                await _serverSnipingManager.DeActivateAsync(currentServerSniping.ServerSnipingId);
                await _serverSnipingManager.AddAsync(new ServerSniping()
                {
                    ServerId = server.ServerId,
                    Enabled = true,
                    IntervalAfterRegistrationOpeningInMinutes = intervalAfterRegistrationOpeningInMinutes,
                    SignUpDelayInMinutes = signUpDelayInMinutes,
                    SignUpDelayDurationInHours = signUpDelayDurationInHours
                });
                changesMade = true;
            }

            if (changesMade)
            {
                await UpdateServerCacheValueAsync(ulong.Parse(server.DiscordServerId));
            }

            return new DiscordWorkflowResult(string.Format(GeneralMessages.SnipingValuesSet.GetValueForLanguage(), intervalAfterRegistrationOpeningInMinutes, signUpDelayInMinutes, signUpDelayDurationInHours), true);
        }
    }
}
