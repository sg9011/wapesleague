using AutoMapper;
using Base.Bot.Infrastructure;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
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
        private readonly DefaultServerSettings _defaultServerSettings;

        public ServerWorkflow(IServerManager serverManager, IServerTeamManager serverTeamManager, IFormationManager formationManager, DefaultServerSettings defaultServerSettings,IMemoryCache cache, IMapper mapper, ILogger<ServerWorkflow> logger
            , ErrorMessages errorMessages, GeneralMessages generalMessages)
            : base (cache, mapper, logger, errorMessages, generalMessages)
        {
            _serverManager = serverManager;
            _serverTeamManager = serverTeamManager;
            _formationManager = formationManager;
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
    }
}
