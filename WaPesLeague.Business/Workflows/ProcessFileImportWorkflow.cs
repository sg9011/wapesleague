using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WaPesLeague.Business.Workflows.Interfaces;
using WaPesLeague.Constants.Resources;
using WaPesLeague.Data.Entities.FileImport;
using WaPesLeague.Data.Managers.FileImport.Intefaces;
using WaPesLeague.Data.Managers.Association.Interfaces;
using System.Linq;
using Newtonsoft.Json;
using WaPesLeague.Business.Dto.Association;
using System.Collections.Generic;
using System;
using System.Globalization;

namespace WaPesLeague.Business.Workflows
{
    public class ProcessFileImportWorkflow : BaseWorkflow<ProcessFileImportWorkflow>, IProcessFileImportWorkflow
    {
        private readonly IFileImportManager _fileImportManager;
        private readonly IDivisionGroupManager _divisionGroupManager;
        private readonly IDivisionGroupRoundManager _divisionGroupRoundManager;

        public ProcessFileImportWorkflow(IFileImportManager fileImportManager, IDivisionGroupManager divisionGroupManager, IDivisionGroupRoundManager divisionGroupRoundManager,
            IMemoryCache memoryCache, IMapper mapper, ILogger<ProcessFileImportWorkflow> logger, ErrorMessages errorMessages, GeneralMessages generalMessages)
            : base(memoryCache, mapper, logger, errorMessages, generalMessages)
        {
            _fileImportManager = fileImportManager;
            _divisionGroupManager = divisionGroupManager;
            _divisionGroupRoundManager = divisionGroupRoundManager;
        }

        public async Task ProcessFileImportAsync(int fileImportId)
        {
            var fileImport = await _fileImportManager.GetByIdAsync(fileImportId);
            switch (fileImport.RecordType)
            {
                case Data.Entities.FileImport.Enums.RecordType.LeagueResultLineV1:
                    await HandleLeagueResultLineV1FileAsync(fileImport);
                    break;
                default:
                    Logger.LogError($"Invalid FileImport.RecordType: {fileImport.RecordType} is not supported in ProcessFileImportAsync");
                    break;
            }
        }

        private async Task HandleLeagueResultLineV1FileAsync(FileImport fileImport)
        {
            var linkedDivisionGroup = await _divisionGroupManager.GetDivisionGroupByGoogleSheetImportTypeIdAsync(fileImport.FileImportTypeId);
            if (linkedDivisionGroup == null)
            {
                Logger.LogWarning($"No DivisionGroup is linked to the GoogleSheetImportTypeId: {fileImport.FileImportTypeId}");
                return;
            }

            if (linkedDivisionGroup.DivisionGroupRounds.Any())
                await _divisionGroupRoundManager.DeleteByDivisionGroupIdAsync(linkedDivisionGroup.DivisionGroupId);

            //var mappedDictionaryRecords = fileImport.FileImportRecords.Select(x => new Tuple<int, Dictionary<string, string>>(x.Row, JsonConvert.DeserializeObject<Dictionary<string, string>>(x.Record))).ToList();
            var mappedLeagueResultLines = fileImport.FileImportRecords.Select(x => MapRecordDictionaryToLeagueResultLineV1Dto(x)).ToList();
            var aap = "a";
        }

        private LeagueResultLineV1Dto MapRecordDictionaryToLeagueResultLineV1Dto(FileImportRecord fileImportRecord)
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
                    if (!string.IsNullOrWhiteSpace(konamiNote))
                    {
                        konamiNote = konamiNote.Trim().Replace(',', '.');
                        leagueResultLineV1Dto.Players.Add(new LeagueResultLinePlayer()
                        {
                            PlayerName = dict[$"Nom Joueur {i}"]?.Trim(),
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
            catch(Exception ex)
            {
                Logger.LogError(ex, "MapRecordDictionaryToLeagueResultLineV1Dto failed");
                throw;
            }
        }
    }
}
