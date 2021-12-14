using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WaPesLeague.Business.Workflows.Interfaces;
using WaPesLeague.Constants.Resources;
using WaPesLeague.Data.Managers.FileImport.Intefaces;
using WaPesLeague.Data.Managers.Association.Interfaces;
using System.Linq;
using Newtonsoft.Json;
using WaPesLeague.Business.Dto.Association;
using System.Collections.Generic;
using System;
using System.Globalization;
using WaPesLeague.Data.Helpers;
using System.Text;
using WaPesLeague.Data.Entities.Association;
using WaPesLeague.Data.Managers.Interfaces;
using WaPesLeague.Data.Entities.User;
using WaPesLeague.Data.Entities.Association.Enums;
using FI = WaPesLeague.Data.Entities.FileImport;
using WaPesLeague.Business.Helpers;
using WaPesLeague.Business.Dto.User;
using WaPesLeague.Constants;
using WaPesLeague.Data.Entities.Match;

namespace WaPesLeague.Business.Workflows
{
    public class ProcessFileImportWorkflow : BaseWorkflow<ProcessFileImportWorkflow>, IProcessFileImportWorkflow
    {
        private const string UserCreatedByGoogleSheetData = "User Created By Google Registration Sheet";
        private const string CreatedByFileImport = "Created By FileImport";

        private readonly IFileImportManager _fileImportManager;
        private readonly IDivisionGroupManager _divisionGroupManager;
        private readonly IDivisionGroupRoundManager _divisionGroupRoundManager;
        private readonly IAssociationTeamManager _associationTeamManager;
        private readonly IAssociationTenantPlayerManager _associationTenantPlayerManager;
        private readonly IAssociationManager _associationManager;
        private readonly IUserManager _userManager;
        private readonly IMetadataManager _metadataManager;
        private readonly IUserMetadataManager _userMetadataManager;

        public ProcessFileImportWorkflow(IFileImportManager fileImportManager, IDivisionGroupManager divisionGroupManager, IDivisionGroupRoundManager divisionGroupRoundManager,
            IAssociationTeamManager associationTeamManager, IAssociationTenantPlayerManager associationTenantPlayerManager, IAssociationManager associationManager,
            IUserManager userManager, IMetadataManager metadataManager, IUserMetadataManager userMetadataManager,
            IMemoryCache memoryCache, IMapper mapper, ILogger<ProcessFileImportWorkflow> logger, ErrorMessages errorMessages, GeneralMessages generalMessages)
            : base(memoryCache, mapper, logger, errorMessages, generalMessages)
        {
            _fileImportManager = fileImportManager;
            _divisionGroupManager = divisionGroupManager;
            _divisionGroupRoundManager = divisionGroupRoundManager;
            _associationTeamManager = associationTeamManager;
            _associationTenantPlayerManager = associationTenantPlayerManager;
            _associationManager = associationManager;
            _userManager = userManager;
            _metadataManager = metadataManager;
            _userMetadataManager = userMetadataManager;
        }

        public async Task ProcessDailyFileImports()
        {
            var fileImports = await _fileImportManager.GetAllSuccessfulFileImports();
            var groupedFileImports = fileImports.OrderByDescending(fi => fi.DateCreated).GroupBy(x => x.FileImportTypeId);
            foreach (var fileImportGroup in groupedFileImports)
            {
                try
                {
                    var firstItem = fileImportGroup.First();
                    if (firstItem.ProcessStatus == FI.Enums.ProcessStatus.Processed)
                        continue;

                    await ProcessFileImportAsync(firstItem.FileImportId);
                }
                catch(Exception ex)
                {
                    Logger.LogError(ex, "An error occured in the ProcessDailyFileImports");
                }
            }
        }

        public async Task ProcessFileImportAsync(int fileImportId)
        {
            var fileImport = await _fileImportManager.GetByIdAsync(fileImportId);
            ProcessFileImportResult result = null;
            switch (fileImport.RecordType)
            {
                case FI.Enums.RecordType.LeagueResultLineV1:
                    result = await HandleLeagueResultLineV1FileAsync(fileImport);
                    break;
                case FI.Enums.RecordType.PlayerRegistrationRecordV1:
                    result = await HandlePlayerRegistrationRecordsAsync(fileImport);
                    break;
                default:
                    Logger.LogError($"Invalid FileImport.RecordType: {fileImport.RecordType} is not supported in ProcessFileImportAsync");
                    break;
            }

            fileImport.ProcessStatus = result.Success == true ? FI.Enums.ProcessStatus.Processed : FI.Enums.ProcessStatus.Failed;
            fileImport.ErrorMessage = result.ErrorMessage;
            fileImport.FileImportRecords = null;
            fileImport.FileImportType = null;
            
            await _fileImportManager.UpdateAsync(fileImport);
        }

        #region LeagueResultLineV1
        private async Task<ProcessFileImportResult> HandleLeagueResultLineV1FileAsync(FI.FileImport fileImport)
        {
            var result = new ProcessFileImportResult();
            var dbNow = DateTimeHelper.GetDatabaseNow();

            var linkedDivisionGroup = await _divisionGroupManager.GetDivisionGroupByGoogleSheetImportTypeIdAsync(fileImport.FileImportTypeId);
            if (linkedDivisionGroup == null)
            {
                Logger.LogWarning($"No DivisionGroup is linked to the GoogleSheetImportTypeId: {fileImport.FileImportTypeId}");
                return result.Failed($"No DivisionGroup is linked to the GoogleSheetImportTypeId: {fileImport.FileImportTypeId}");
            }

            var mappedLeagueResultLines = fileImport.FileImportRecords.Select(x => MapRecordDictionaryToLeagueResultLineV1Dto(x)).ToList();
            var mappedWaPesResultLines = MapLeagueResultLinesToAppResultLines(mappedLeagueResultLines, linkedDivisionGroup.DivisionGroupId);

            var hasErrors = CheckAndLogErrors(mappedWaPesResultLines);
            if (hasErrors)
                return result.Failed($"Error in CheckAndLogErrors: Check The Logs");

            var orderedMappedWaPesResultLines = mappedWaPesResultLines.OrderBy(x => x.DivisionGroupRoundNumber).ThenBy(x => x.HomeTeam).ThenBy(x => x.MatchNumber).ToList();

            var linkedAssociation = await _associationManager.GetAssociationByDivisionGroupIdAsync(linkedDivisionGroup.DivisionGroupId);

            try
            {
                var teams = await FindTeamsAndCreateEnitiesIfNeededAsync(orderedMappedWaPesResultLines, linkedAssociation.AssociationId, linkedAssociation.DefaultTeamType);
                var tenantPlayers = await FindTenantPlayersAndCreateEntitiesIfNeededAsync(orderedMappedWaPesResultLines, linkedAssociation.AssociationTenantId);

                await CreateMatchesAndRounds(orderedMappedWaPesResultLines, linkedDivisionGroup.DivisionGroupId, dbNow);
                var associationTeamPlayers = await FindTeamPlayersAndCreateEntitiesIfNeededAsync(teams, tenantPlayers, orderedMappedWaPesResultLines, linkedAssociation.AssociationTenantId);
                

                if (linkedDivisionGroup.DivisionGroupRounds.Any())
                    await _divisionGroupRoundManager.DeleteByDivisionGroupIdAsync(linkedDivisionGroup.DivisionGroupId);

                return result.IsSuccessful();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in the HandleLeagueResultLineV1FileAsync");
                return result.Failed($"Error in the HandleLeagueResultLineV1FileAsync: {ex.Message}");
            }
        }

        private LeagueResultLineV1Dto MapRecordDictionaryToLeagueResultLineV1Dto(FI.FileImportRecord fileImportRecord)
        {
            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileImportRecord.Record);
                var leagueResultLineV1Dto = new LeagueResultLineV1Dto()
                {
                    FileImportRecord = fileImportRecord,
                    SheetRowId = dict["N°"]?.Trim(),
                    GroupRound = dict["Journée"]?.Trim(),
                    MatchNumber = int.Parse(dict["Match"]?.Replace("Match ", "")),
                    HomeTeam = dict["Equipe"]?.Trim(),
                    AwayTeam = dict["Adversaire"]?.Trim(),
                    HomeScore = int.Parse(dict["Score Dom"]),
                    HomeScoreConfirmed = int.Parse(dict["Score Dom_"]),
                    AwayScore = int.Parse(dict["Score Ext"]),
                    AwayScoreConfirmed = int.Parse(dict["Score Ext_"]),
                    Score = dict["Score"]?.Trim(),
                    ReportingError = dict["Erreur reportée"]?.Trim(),
                    MatchIdentifierOnGoogleSheet = dict["Match_"]?.Trim(),
                    EmailResultPoster = dict["Email"]?.Trim()
                };

                for (int i = 1; i <= 22; i++)
                {
                    var konamiNote = dict[$"Note Joueur {i}"];
                    var playerName = dict[$"Nom Joueur {i}"];
                    if (!string.IsNullOrWhiteSpace(konamiNote) && !string.IsNullOrWhiteSpace(playerName))
                    {
                        konamiNote = konamiNote.Trim().Replace(',', '.');
                        leagueResultLineV1Dto.Players.Add(new LeagueResultLinePlayer()
                        {
                            PlayerName = playerName?.Trim(),
                            Position = dict[$"Poste Joueur {i}"]?.Trim(),
                            KonamiRating = decimal.Parse(konamiNote, NumberStyles.Any, CultureInfo.InvariantCulture),
                            Goals = int.Parse(string.IsNullOrWhiteSpace(dict[$"Buts Joueur {i}"]?.Trim()) ? "0" : dict[$"Buts Joueur {i}"].Trim()),
                            Assists = int.Parse(string.IsNullOrWhiteSpace(dict[$"Passes Joueur {i}"]?.Trim()) ? "0" : dict[$"Passes Joueur {i}"].Trim()),
                            PlayerNumberOnSheet = i
                        });
                    }
                }
                return leagueResultLineV1Dto;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "MapRecordDictionaryToLeagueResultLineV1Dto failed");
                throw;
            }
        }

        private List<AppResultLine> MapLeagueResultLinesToAppResultLines(List<LeagueResultLineV1Dto> leagueResultLinesV1, int divisionGroupId)
        {
            var resultLines = new List<AppResultLine>();
            foreach (var groupRoundResults in leagueResultLinesV1.GroupBy(it => it.GroupRound))
            {
                var groupRoundNumber = int.Parse(groupRoundResults.Key.OnlyKeepNumbers09());
                foreach (var matchDayResults in groupRoundResults.OrderBy(x => x.MatchNumber).GroupBy(x => x.MatchNumber))
                {
                    var matchDayNumberListGames = new List<AppResultLine>();
                    foreach (var matchDayResult in matchDayResults)
                    {
                        var postResultOfOtherTeam = matchDayNumberListGames.SingleOrDefault(x => string.Equals(x.AwayTeam, matchDayResult.HomeTeam, StringComparison.InvariantCultureIgnoreCase));
                        if (postResultOfOtherTeam == null)
                        {
                            var appResultLine = new AppResultLine()
                            {
                                SheetRowId = matchDayResult.SheetRowId,
                                DivisionGroupDBId = divisionGroupId,
                                DivisionGroupRoundNumber = groupRoundNumber,
                                MatchNumber = matchDayResults.Key,
                                HomeTeam = matchDayResult.HomeTeam,
                                HomeScore = matchDayResult.HomeScoreConfirmed,
                                AwayTeam = matchDayResult.AwayTeam,
                                AwayScore = matchDayResult.AwayScoreConfirmed,
                                HomePlayers = matchDayResult.Players.Select(x => ToAppPlayerResultLine(x)).ToList()
                            };
                            matchDayNumberListGames.Add(appResultLine);
                        }
                        else
                        {
                            postResultOfOtherTeam.AwayPlayers = matchDayResult.Players.Select(x => ToAppPlayerResultLine(x)).ToList();
                        }
                    }
                    resultLines.AddRange(matchDayNumberListGames);
                }
            }

            return resultLines;
        }

        private AppPlayerResultLine ToAppPlayerResultLine(LeagueResultLinePlayer player)
        {
            return new AppPlayerResultLine()
            {
                PlayerName = player.PlayerName,
                Position = player.Position,
                KonamiRating = player.KonamiRating,
                Goals = player.Goals,
                Assists = player.Assists,
                PlayerNumberOnSheet = player.PlayerNumberOnSheet
            };
        }

        private bool CheckAndLogErrors(List<AppResultLine> appResultLines)
        {
            var errorLines = appResultLines.Where(x => (x.AwayPlayers == null || !x.AwayPlayers.Any()) || (x.HomePlayers == null || !x.HomePlayers.Any())).ToList();
            if (errorLines.Any())
            {
                var errorBuilder = new StringBuilder();
                foreach(var errorLine in errorLines)
                {
                    var error = (errorLine.HomePlayers == null || !errorLine.HomePlayers.Any())
                        ? $"No home players found so record posted by team: {errorLine.HomeTeam ?? string.Empty} vs {errorLine.AwayTeam ?? string.Empty} is not correctly posted for Round: {errorLine.DivisionGroupRoundNumber} ,Match: {errorLine.MatchNumber}"
                        : (errorLine.AwayPlayers == null || !errorLine.AwayPlayers.Any())
                            ? $"No away players found so record posted by team: {errorLine.AwayTeam ?? string.Empty} is not correctly posted for Round: {errorLine.DivisionGroupRoundNumber} ,Match: {errorLine.MatchNumber}"
                            : $"Something is not right for record home team and or away team: {errorLine.AwayTeam ?? string.Empty}, {errorLine.HomeTeam ?? string.Empty} for Round: {errorLine.DivisionGroupRoundNumber} ,Match: {errorLine.MatchNumber}";
                    errorBuilder.AppendLine($"Error in line: {errorLine.SheetRowId}, {error}");
                }

                Logger.LogError(errorBuilder.ToString());
                return true;
            }

            return false;
        }

        private async Task<List<AssociationTeam>> FindTeamsAndCreateEnitiesIfNeededAsync(List<AppResultLine> appResultLines, int associationId, TeamType teamType)
        {
            var appResultLine = appResultLines.First();
            var teams = appResultLines.SelectMany(x => new List<string>() { x.HomeTeam, x.AwayTeam }).Distinct().ToList();
            var associationTeams = await _associationTeamManager.GetAssociationTeamsByAssociationIdAsync(associationId);
            var databaseNow = DateTimeHelper.GetDatabaseNow();

            var returnTeams = new List<AssociationTeam>();

            foreach (var team in teams)
            {
                var matchingAssociationTeam = associationTeams.FirstOrDefault(at => string.Equals(team, at.Name, StringComparison.InvariantCultureIgnoreCase));
                if (matchingAssociationTeam != null)
                {
                    appResultLines.Where(arl => string.Equals(arl.HomeTeam, team, StringComparison.InvariantCultureIgnoreCase))
                        .ToList()
                        .ForEach(arl => arl.HomeAssociationTeamId = matchingAssociationTeam.AssociationTeamId);

                    appResultLines.Where(arl => string.Equals(arl.AwayTeam, team, StringComparison.InvariantCultureIgnoreCase))
                        .ToList()
                        .ForEach(arl => arl.AwayAssociationTeamId = matchingAssociationTeam.AssociationTeamId);

                    returnTeams.Add(matchingAssociationTeam);
                }
                else
                {
                    var newAssociationTeam = new AssociationTeam()
                    {
                        AssociationId = associationId,
                        Guid = Guid.NewGuid(),
                        Name = team,
                        Description = $"{team} - createdBy FileImport",
                        TeamType = teamType,
                        DateCreated = databaseNow
                    };
                    returnTeams.Add(newAssociationTeam);
                }
            }

            await _associationTeamManager.AddMultipleAsync(returnTeams.Where(t => t.AssociationTeamId == 0).ToList());

            return returnTeams;
        }
        
        private async Task<List<UserAndTenantPlayerDto>> FindTenantPlayersAndCreateEntitiesIfNeededAsync(List<AppResultLine> appResultLines, int associationTenantId)
        {
            var returnObject = new List<UserAndTenantPlayerDto>();
            var existingUserNoTenantPlayerList = new List<UserAndTenantPlayerDto>();
            var noUserList = new List<UserAndTenantPlayerDto>();
            var players = appResultLines.SelectMany(a => a.HomePlayers.Concat(a.AwayPlayers)).ToList();
            var distinctPlayerNames = players.Select(p => p.PlayerName).Distinct().ToList();
            var associationTenantPlayers = await _associationTenantPlayerManager.GetAssociationTenantPlayersByAssociationTenantIdAsync(associationTenantId);
            var userIdsThatAreAlreadyTenantPlayers = associationTenantPlayers.Select(atp => atp.UserId).Distinct().ToList();
            var usersThatAreNotTenantPlayers = (await _userManager.GetAllAsync()).Where(u => !userIdsThatAreAlreadyTenantPlayers.Contains(u.UserId)).ToList();
            var databaseNow = DateTimeHelper.GetDatabaseNow();
            var metadatas = await _metadataManager.GetAllAsync();
            var discordNameMetadataId = metadatas.First(x => string.Equals(x.Code, MetadataConstants.WaPesDiscordName, StringComparison.InvariantCultureIgnoreCase)).MetadataId;

            foreach (var playerName in distinctPlayerNames)
            {
                var playersToUpdate = players.Where(x => string.Equals(x.PlayerName, playerName, StringComparison.InvariantCultureIgnoreCase)).ToList();
                var matchingAssociationTenantPlayer = associationTenantPlayers.FirstOrDefault(at => string.Equals(playerName, at.Name, StringComparison.InvariantCultureIgnoreCase));
                if (matchingAssociationTenantPlayer != null)
                {
                    playersToUpdate.ForEach(p => p.AssociationPlayerTenantId = matchingAssociationTenantPlayer.AssociationTenantPlayerId);
                    returnObject.Add(new UserAndTenantPlayerDto(playerName, matchingAssociationTenantPlayer.User, playersToUpdate, matchingAssociationTenantPlayer));
                }
                else
                {
                    var existingUser = FindUser(usersThatAreNotTenantPlayers, playerName, discordNameMetadataId);
                    if (existingUser != null)
                    {
                        var associationTenantPlayer = new AssociationTenantPlayer()
                        {
                            UserId = existingUser.UserId,
                            AssociationTenantId = associationTenantId,
                            DateCreated = databaseNow,
                            Name = playerName
                        };

                        existingUserNoTenantPlayerList.Add(new UserAndTenantPlayerDto(playerName, existingUser, playersToUpdate, associationTenantPlayer));
                    }
                    else
                    {
                        var newUser = new User()
                        {
                            UserGuid = Guid.NewGuid(),
                            ExtraInfo = CreatedByFileImport,
                            ExternalId = playerName,
                        };
                        var associationTenantPlayer = new AssociationTenantPlayer()
                        {
                            AssociationTenantId = associationTenantId,
                            DateCreated = databaseNow,
                            Name = playerName
                        };

                        noUserList.Add(new UserAndTenantPlayerDto(playerName, newUser, playersToUpdate, associationTenantPlayer));
                    }
                }
            }

            //existingUserNoTenantPlayerList
            await _associationTenantPlayerManager.AddMultipleAsync(existingUserNoTenantPlayerList.Select(x => x.AssociationTenantPlayer).ToList());
            existingUserNoTenantPlayerList.ForEach(x => x.PlayerResultLines.ForEach(prl => prl.AssociationPlayerTenantId = x.AssociationTenantPlayer.AssociationTenantPlayerId));

            //noUserList
            await _userManager.AddMultipleAsync(noUserList.Select(x => x.User).ToList());
            noUserList.ForEach(x => x.AssociationTenantPlayer.UserId = x.User.UserId);
            await _associationTenantPlayerManager.AddMultipleAsync(noUserList.Select(x => x.AssociationTenantPlayer).ToList());
            noUserList.ForEach(x => x.PlayerResultLines.ForEach(prl => prl.AssociationPlayerTenantId = x.AssociationTenantPlayer.AssociationTenantPlayerId));

            //We could also return the 3 objects so actual creation is handled seperatly together with the teams
            return returnObject.Concat(existingUserNoTenantPlayerList).Concat(noUserList).ToList();
        }

        private async Task CreateMatchesAndRounds(List<AppResultLine> resultLines, int divisionGroupId, DateTime dbNow)
        {
            var groupedResultLines = resultLines.GroupBy(x => x.DivisionGroupRoundNumber);
            var orderCounter = 0;
            foreach (var groupResultLines in groupedResultLines)
            {
                var order = ++orderCounter;
                var divisionGroupRound = new DivisionGroupRound()
                {
                    DivisionGroupId = divisionGroupId,
                    Name = $"Round {order}",
                    Order = order,
                    DateCreated = dbNow,
                    Start = DateTime.MinValue,
                    End = null,
                    Matches = new List<Match>(),
                };
                foreach(var resultLine in groupResultLines)
                {
                    var match = new Match()
                    {
                        Guid = Guid.NewGuid(),
                        Order = resultLine.MatchNumber,
                        DateCreated = dbNow,
                        MatchStatus = Data.Entities.Match.Enums.MatchStatus.ResultConfirmed,
                        DivisionGroupRound = divisionGroupRound,
                        MatchTeams = new List<MatchTeam>()
                    };

                    var homeTeam = new MatchTeam()
                    {
                        Match = match,
                        TeamId = resultLine.HomeAssociationTeamId.Value,
                        Goals = resultLine.HomeScore,
                        DateConfirmed = dbNow,
                    };
                    foreach (var playerResultLine in resultLine.HomePlayers)
                    {

                    }
                    match.MatchTeams.Add(homeTeam);
                    

                    var awayTeam = new MatchTeam()
                    {
                        Match = match,
                        TeamId = resultLine.AwayAssociationTeamId.Value,
                        Goals = resultLine.AwayScore,
                        DateConfirmed = dbNow
                    };
                    match.MatchTeams.Add(awayTeam);


                    divisionGroupRound.Matches.Add(match);
                }
            }
        }

        private async Task<List<AssociationTeamPlayer>> FindTeamPlayersAndCreateEntitiesIfNeededAsync(List<AssociationTeam> associationTeams, List<UserAndTenantPlayerDto> userAndTenantPlayerDtos, List<AppResultLine> appResultLines, int associationTenantId)
        {
            var allAssociationTenantPlayers = await _associationTenantPlayerManager.GetAllAsync(associationTenantId);
            foreach(var appResultLine in appResultLines)
            {
                var homeTeam = associationTeams.FirstOrDefault(at => at.AssociationTeamId == appResultLine.HomeAssociationTeamId.Value);
                var awayTeam = associationTeams.FirstOrDefault(at => at.AssociationTeamId == appResultLine.AwayAssociationTeamId.Value);
                
                //ToDo Handle Dates Etc
                foreach(var playerLine in appResultLine.HomePlayers)
                {
                    var associationTenantPlayer = allAssociationTenantPlayers.First(x => x.AssociationTenantPlayerId == playerLine.AssociationPlayerTenantId.Value);
                    var playerTeamRecord = associationTenantPlayer.AssociationTeamPlayers.FirstOrDefault(atp => atp.AssociationTeamId == homeTeam.AssociationTeamId);
                    if (playerTeamRecord == null)
                    {
                        playerTeamRecord = new AssociationTeamPlayer()
                        {
                            AssociationTeamId = homeTeam.AssociationTeamId,
                            AssociationTenantPlayerId = associationTenantPlayer.AssociationTenantPlayerId,
                            Start = DateTime.MinValue,
                            TeamMemberType = TeamMemberType.Player
                            //AssociationTenantPlayer = associationTenantPlayer

                        };
                        associationTenantPlayer.AssociationTeamPlayers.Add(playerTeamRecord);
                    }
                }

                foreach (var playerLine in appResultLine.AwayPlayers)
                {
                    var associationTenantPlayer = allAssociationTenantPlayers.First(x => x.AssociationTenantPlayerId == playerLine.AssociationPlayerTenantId.Value);
                    var playerTeamRecord = associationTenantPlayer.AssociationTeamPlayers.FirstOrDefault(atp => atp.AssociationTeamId == awayTeam.AssociationTeamId);
                    if (playerTeamRecord == null)
                    {

                    }
                }
            }
            return new List<AssociationTeamPlayer>();
        }

        private User FindUser(List<User> users, string userName, int discordNameMetadataId)
        {
            return users.FirstOrDefault(u => string.Equals(userName, u.ExternalId, StringComparison.InvariantCultureIgnoreCase))
                ?? users.FirstOrDefault(u => u.UserMembers.Any(um => string.Equals(userName, um.DiscordUserName, StringComparison.InvariantCultureIgnoreCase)))
                ?? users.FirstOrDefault(u => u.UserMembers.Any(um => string.Equals(userName, um.DiscordNickName, StringComparison.InvariantCultureIgnoreCase)))
                ?? users.FirstOrDefault(u => u.PlatformUsers.Any(up => string.Equals(userName, up.UserName, StringComparison.InvariantCultureIgnoreCase)))
                ?? users.FirstOrDefault(u => u.UserMetadatas?.Any(um => um.MetadataId == discordNameMetadataId && string.Equals(um.Value, userName, StringComparison.InvariantCultureIgnoreCase)) ?? false);
        }

        #endregion

        #region PlayerRegistrationRecordV1
        private async Task<ProcessFileImportResult> HandlePlayerRegistrationRecordsAsync(FI.FileImport fileImport)
        {
            var result = new ProcessFileImportResult();
            var allUsers = await _userManager.GetAllAsync();

            var mappedRegistrationLines = fileImport.FileImportRecords.Select(x => MapFileImportRecordToPlayerV1RegistrationRecordDto(x)).ToList();
            var registrationMetadataIds = await GetRegistrationMetadataIds();
            var usersToCreate = new List<User>();
            var usersToUpdate = new List<User>();
            var userMetadataToAdd = new List<UserMetadata>();
            var userMetadataToUpdate = new List<UserMetadata>();
            foreach (var mappedRegistrationLine in mappedRegistrationLines.Where(x => !string.IsNullOrWhiteSpace(x.InternalId)))
            {
                var user = FindUserByV1RegistrationLine(allUsers, mappedRegistrationLine, registrationMetadataIds);
                if (user != null)
                {
                    if (UpdateUser(user, mappedRegistrationLine, userMetadataToAdd, userMetadataToUpdate, registrationMetadataIds))
                    {
                        usersToUpdate.Add(user);
                    }
                }
                if (user == null)
                {
                    usersToCreate.Add(CreateUserObject(mappedRegistrationLine, registrationMetadataIds));
                }
            }

            if (usersToCreate.Any())
            {
                await _userManager.AddMultipleAsync(usersToCreate);
            }
            if (usersToUpdate.Any())
            {
                await _userManager.UpdateMulitpleAsync(usersToUpdate);
            }
            if (userMetadataToAdd.Any())
            {
                await _userMetadataManager.AddMultipleAsync(userMetadataToAdd);
            }
            if (userMetadataToUpdate.Any())
            {
                await _userMetadataManager.UpdateMultipleAsync(userMetadataToUpdate);
            }

            return result.IsSuccessful();
        }


        private User FindUserByV1RegistrationLine(IReadOnlyCollection<User> users, PlayerRegistrationRecordV1Dto registrationLine, RegistrationMetadataId registrationMetadataId)
        {
            var strippedName = registrationLine.GetDiscordNameWithoutDiscriminator();
            var discriminator = registrationLine.GetDiscordNameWithoutDiscriminator();

            var returnUser = users.FirstOrDefault(u => u.UserMembers.Any(um => string.Equals(um.DiscordUserId, registrationLine.DiscordId, StringComparison.InvariantCultureIgnoreCase)))
                ?? users.FirstOrDefault(u => u.UserMetadatas.Any(um => um.MetadataId == registrationMetadataId.DiscordId && string.Equals(um.Value, registrationLine.DiscordId, StringComparison.InvariantCultureIgnoreCase)))
                ?? users.FirstOrDefault(u => string.Equals(u.DiscordName, strippedName, StringComparison.InvariantCultureIgnoreCase) && (discriminator == null || string.Equals(u.DiscordDiscriminator, discriminator, StringComparison.InvariantCultureIgnoreCase)));

            if (returnUser == null && !string.IsNullOrWhiteSpace(registrationLine.InternalId))
            {
                returnUser = users.FirstOrDefault(u => string.Equals(u.ExternalId, registrationLine.InternalId, StringComparison.InvariantCultureIgnoreCase));
            }

            return returnUser;

        }
        private PlayerRegistrationRecordV1Dto MapFileImportRecordToPlayerV1RegistrationRecordDto(FI.FileImportRecord fileImportRecord)
        {
            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileImportRecord.Record);
                var stringJoinDate = dict["Horodateur"]?.Trim();
                var parsedJoinDate = DateTime.TryParse(stringJoinDate, out var parsedDate);
                var ageString = dict["Age"]?.Trim();
                var playerRegistrationRecordV1Dto = new PlayerRegistrationRecordV1Dto()
                {
                    FileImportRecord = fileImportRecord,
                    JoiningDate = parsedJoinDate ? (DateTime?)parsedDate : null,
                    GoogleImageLink = dict["Avatar (add image google drive link)"]?.Trim(),
                    DiscordName = dict["Pseudo (discord)"]?.Trim(),
                    PSNName = dict["Pseudo (PSN)"]?.Trim(),
                    Status = dict["Status"]?.Trim(),
                    NationalTeam = dict["National team"]?.Trim(),
                    OriginalNation = dict["(Original nation)"]?.Trim(),
                    Age = string.IsNullOrEmpty(ageString) ? null : (int?)int.Parse(ageString),
                    English = dict["Speak english"]?.Trim().OptionStringToBool(ErrorMessages, true) ?? true,
                    Position1 = dict["Position 1 (prefered)"]?.Trim(),
                    Position2 = dict["Position 2"]?.Trim(),
                    Motto = dict["Motto"]?.Trim(),
                    FootballStyle = dict["Football style"]?.Trim(),
                    Quality1 = dict["Best quality 1"]?.Trim(),
                    Quality2 = dict["Best quality 2"]?.Trim(),
                    Quality3 = dict["Best quality 3"]?.Trim(),
                    Email = dict["Adresse e-mail"]?.Trim(),
                    HowDidYouFindWapes = dict["How did you hear about the WAPES League?"]?.Trim(),
                    DiscordId = dict["DiscordId"]?.Trim(),
                    InternalId = dict["InternalId"]?.Trim()
                };

                return playerRegistrationRecordV1Dto;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "MapFileImportRecordToPlayerV1RegistrationRecordDto failed");
                throw;
            }
        }

        private User CreateUserObject(PlayerRegistrationRecordV1Dto registrationLine, RegistrationMetadataId registrationMetadataId)
        {
            var user = new User()
            {
                UserGuid = Guid.NewGuid(),
                DiscordName = registrationLine.GetDiscordNameWithoutDiscriminator(),
                DiscordDiscriminator = registrationLine.GetDiscriminator(),
                Email = registrationLine.Email,
                ExternalId = registrationLine.InternalId,
                ExtraInfo = UserCreatedByGoogleSheetData,
            };

            var userMetadatas = new List<UserMetadata>();
            if (!string.IsNullOrWhiteSpace(registrationLine.DiscordId))
            {
                userMetadatas.Add(new UserMetadata()
                {
                    MetadataId = registrationMetadataId.DiscordId,
                    Value = registrationLine.DiscordId,
                    User = user,
                });
            }
            if (!string.IsNullOrWhiteSpace(registrationLine.GetDiscordNameWithoutDiscriminator()))
            {
                userMetadatas.Add(new UserMetadata()
                {
                    MetadataId = registrationMetadataId.WaPesDiscordName,
                    Value = registrationLine.GetDiscordNameWithoutDiscriminator(),
                    User = user,
                });
            }
            if (!string.IsNullOrWhiteSpace(registrationLine.PSNName))
            {
                userMetadatas.Add(new UserMetadata()
                {
                    MetadataId = registrationMetadataId.WaPesPSNName,
                    Value = registrationLine.PSNName,
                    User = user,
                });
            }
            userMetadatas.Add(new UserMetadata()
            {
                    MetadataId = registrationMetadataId.SpeakEnglish,
                    Value = registrationLine.English.ToString(),
                    User = user,
            });
            if (!string.IsNullOrWhiteSpace(registrationLine.Position1))
            {
                userMetadatas.Add(new UserMetadata()
                {
                    MetadataId = registrationMetadataId.Pos1,
                    Value = registrationLine.Position1,
                    User = user,
                });
            }
            if (!string.IsNullOrWhiteSpace(registrationLine.Position2))
            {
                userMetadatas.Add(new UserMetadata()
                {
                    MetadataId = registrationMetadataId.Pos2,
                    Value = registrationLine.Position2,
                    User = user,
                });
            }

            if (!string.IsNullOrWhiteSpace(registrationLine.Motto))
            {
                userMetadatas.Add(new UserMetadata()
                {
                    MetadataId = registrationMetadataId.Motto,
                    Value = registrationLine.Motto,
                    User = user,
                });
            }
            if (!string.IsNullOrWhiteSpace(registrationLine.FootballStyle))
            {
                userMetadatas.Add(new UserMetadata()
                {
                    MetadataId = registrationMetadataId.FootballStyle,
                    Value = registrationLine.FootballStyle,
                    User = user,
                });
            }

            if (!string.IsNullOrWhiteSpace(registrationLine.Quality1))
            {
                userMetadatas.Add(new UserMetadata()
                {
                    MetadataId = registrationMetadataId.Quality1,
                    Value = registrationLine.Quality1,
                    User = user,
                });
            }
            if (!string.IsNullOrWhiteSpace(registrationLine.Quality2))
            {
                userMetadatas.Add(new UserMetadata()
                {
                    MetadataId = registrationMetadataId.Quality2,
                    Value = registrationLine.Quality2,
                    User = user,
                });
            }
            if (!string.IsNullOrWhiteSpace(registrationLine.Quality3))
            {
                userMetadatas.Add(new UserMetadata()
                {
                    MetadataId = registrationMetadataId.Quality3,
                    Value = registrationLine.Quality3,
                    User = user,
                });
            }
            if (registrationLine.JoiningDate.HasValue)
            {
                userMetadatas.Add(new UserMetadata()
                {
                    MetadataId = registrationMetadataId.WaPesJoinDate,
                    Value = registrationLine.JoiningDate.Value.ToUniversalTime().ToString(),
                    User = user,
                });
            }

            user.UserMetadatas = userMetadatas;
            return user;
        }

        private bool UpdateUser(User user, PlayerRegistrationRecordV1Dto registrationLine, List<UserMetadata> userMetadataToAdd, List<UserMetadata> userMetadataToUpdate, RegistrationMetadataId registrationMetadataId)
        {
            var updateUser = false;
            
            if (!string.IsNullOrWhiteSpace(registrationLine.InternalId) && !string.Equals(user.ExternalId, registrationLine.InternalId, StringComparison.InvariantCultureIgnoreCase))
            {
                updateUser = true;
                user.ExternalId = registrationLine.InternalId;
            }
            if (!string.IsNullOrWhiteSpace(registrationLine.GetDiscriminator()) && string.IsNullOrWhiteSpace(user.DiscordDiscriminator))
            {
                updateUser = true;
                user.DiscordDiscriminator = registrationLine.GetDiscriminator();
            }

            if (!string.IsNullOrWhiteSpace(registrationLine.GetDiscordNameWithoutDiscriminator()) && string.IsNullOrWhiteSpace(user.DiscordName))
            {
                updateUser = true;
                user.DiscordName = registrationLine.GetDiscordNameWithoutDiscriminator();
            }

            if (!string.IsNullOrWhiteSpace(registrationLine.Email) && string.IsNullOrWhiteSpace(user.Email))
            {
                updateUser = true;
                user.Email = registrationLine.Email;
            }

            AddOrUpdateStringMetadata(user, registrationLine.DiscordId, registrationMetadataId.DiscordId, userMetadataToAdd, userMetadataToUpdate);
            AddOrUpdateStringMetadata(user, registrationLine.DiscordName, registrationMetadataId.WaPesDiscordName, userMetadataToAdd, userMetadataToUpdate);
            AddOrUpdateStringMetadata(user, registrationLine.PSNName, registrationMetadataId.WaPesPSNName, userMetadataToAdd, userMetadataToUpdate);
            AddOrUpdateStringMetadata(user, registrationLine.Position1, registrationMetadataId.Pos1, userMetadataToAdd, userMetadataToUpdate);
            AddOrUpdateStringMetadata(user, registrationLine.Position2, registrationMetadataId.Pos2, userMetadataToAdd, userMetadataToUpdate);
            AddOrUpdateStringMetadata(user, registrationLine.Motto, registrationMetadataId.Motto, userMetadataToAdd, userMetadataToUpdate);
            AddOrUpdateStringMetadata(user, registrationLine.FootballStyle, registrationMetadataId.FootballStyle, userMetadataToAdd, userMetadataToUpdate);
            AddOrUpdateStringMetadata(user, registrationLine.Quality1, registrationMetadataId.Quality1, userMetadataToAdd, userMetadataToUpdate);
            AddOrUpdateStringMetadata(user, registrationLine.Quality2, registrationMetadataId.Quality2, userMetadataToAdd, userMetadataToUpdate);
            AddOrUpdateStringMetadata(user, registrationLine.Quality3, registrationMetadataId.Quality3, userMetadataToAdd, userMetadataToUpdate);
            AddOrUpdateBoolMetadata(user, registrationLine.English, registrationMetadataId.SpeakEnglish, userMetadataToAdd, userMetadataToUpdate);
            AddOrUpdateDateTimeMetadata(user, registrationLine.JoiningDate, registrationMetadataId.WaPesJoinDate, userMetadataToAdd, userMetadataToUpdate);
            




            return updateUser;
        }

        private void AddOrUpdateStringMetadata(User user, string newValue, int metadataId, List<UserMetadata> userMetadatasToAdd, List<UserMetadata> userMetadatasToUpdate)
        {
            if (!string.IsNullOrWhiteSpace(newValue))
            {
                var metadataValueRecord = user.UserMetadatas.FirstOrDefault(um => um.MetadataId == metadataId);
                if (metadataValueRecord == null)
                {
                    userMetadatasToAdd.Add(new UserMetadata()
                    {
                        UserId = user.UserId,
                        MetadataId = metadataId,
                        Value = newValue
                    });
                }
                else if (!string.Equals(metadataValueRecord.Value, newValue))
                {
                    metadataValueRecord.Value = newValue;
                    userMetadatasToUpdate.Add(metadataValueRecord);
                }
            }

            return;
        }
        private void AddOrUpdateBoolMetadata(User user, bool? newValue, int metadataId, List<UserMetadata> userMetadatasToAdd, List<UserMetadata> userMetadatasToUpdate)
        {
            if (newValue.HasValue)
            {
                var transformedNewValue = newValue.Value.ToString();
                var metadataValueRecord = user.UserMetadatas.FirstOrDefault(um => um.MetadataId == metadataId);
                if (metadataValueRecord == null)
                {
                    userMetadatasToAdd.Add(new UserMetadata()
                    {
                        UserId = user.UserId,
                        MetadataId = metadataId,
                        Value = transformedNewValue
                    });
                }
                else if (!string.Equals(metadataValueRecord.Value, transformedNewValue))
                {
                    metadataValueRecord.Value = transformedNewValue;
                    userMetadatasToUpdate.Add(metadataValueRecord);
                }
            }

            return;
        }

        private void AddOrUpdateDateTimeMetadata(User user, DateTime? newValue, int metadataId, List<UserMetadata> userMetadatasToAdd, List<UserMetadata> userMetadatasToUpdate)
        {
            if (newValue.HasValue)
            {
                var transformedNewValue = newValue.Value.ToUniversalTime().ToString();
                var metadataValueRecord = user.UserMetadatas.FirstOrDefault(um => um.MetadataId == metadataId);
                if (metadataValueRecord == null)
                {
                    userMetadatasToAdd.Add(new UserMetadata()
                    {
                        UserId = user.UserId,
                        MetadataId = metadataId,
                        Value = transformedNewValue
                    });
                }
                else if (!string.Equals(metadataValueRecord.Value, transformedNewValue))
                {
                    metadataValueRecord.Value = transformedNewValue;
                    userMetadatasToUpdate.Add(metadataValueRecord);
                }
            }

            return;
        }
        private async Task<RegistrationMetadataId> GetRegistrationMetadataIds()
        {
            var metadatas = await _metadataManager.GetAllAsync();
            return new RegistrationMetadataId()
            {
                DiscordId = metadatas.First(x => string.Equals(x.Code, MetadataConstants.DiscordId, StringComparison.InvariantCultureIgnoreCase)).MetadataId,
                WaPesDiscordName = metadatas.First(x => string.Equals(x.Code, MetadataConstants.WaPesDiscordName, StringComparison.InvariantCultureIgnoreCase)).MetadataId,
                WaPesPSNName = metadatas.First(x => string.Equals(x.Code, MetadataConstants.WaPesPSNName, StringComparison.InvariantCultureIgnoreCase)).MetadataId,
                SpeakEnglish = metadatas.First(x => string.Equals(x.Code, MetadataConstants.SpeakEnglish, StringComparison.InvariantCultureIgnoreCase)).MetadataId,
                Pos1 = metadatas.First(x => string.Equals(x.Code, MetadataConstants.FavouritePosition1, StringComparison.InvariantCultureIgnoreCase)).MetadataId,
                Pos2 = metadatas.First(x => string.Equals(x.Code, MetadataConstants.FavouritePosition2, StringComparison.InvariantCultureIgnoreCase)).MetadataId,
                Motto = metadatas.First(x => string.Equals(x.Code, MetadataConstants.Motto, StringComparison.InvariantCultureIgnoreCase)).MetadataId,
                FootballStyle = metadatas.First(x => string.Equals(x.Code, MetadataConstants.FootballStyle, StringComparison.InvariantCultureIgnoreCase)).MetadataId,
                Quality1 = metadatas.First(x => string.Equals(x.Code, MetadataConstants.Quality1, StringComparison.InvariantCultureIgnoreCase)).MetadataId,
                Quality2 = metadatas.First(x => string.Equals(x.Code, MetadataConstants.Quality2, StringComparison.InvariantCultureIgnoreCase)).MetadataId,
                Quality3 = metadatas.First(x => string.Equals(x.Code, MetadataConstants.Quality3, StringComparison.InvariantCultureIgnoreCase)).MetadataId,
                WaPesJoinDate = metadatas.First(x => string.Equals(x.Code, MetadataConstants.WaPesJoinDate, StringComparison.InvariantCultureIgnoreCase)).MetadataId
            };
        }

        #endregion
    }
}