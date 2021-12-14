using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.FileImport;
using WaPesLeague.Data.Entities.FileImport.Enums;
using WaPesLeague.Data.Helpers;
using WaPesLeague.GoogleSheets.Interfaces;
using WaPesLeague.GoogleSheets.Helpers;
using WaPesLeague.Data.Managers.FileImport.Intefaces;

namespace WaPesLeague.GoogleSheets
{
    public class ImportFileTabDataHandler : IImportFileTabDataHandler
    {
        static readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static SheetsService service;

        private readonly IGoogleSheetImportManager _googleSheetImportManager;
        private readonly IFileImportManager _fileImportManager;
        private readonly IFileImportRecordManager _fileImportRecordManager;
        private readonly ILogger<ImportFileTabDataHandler> _logger;
        public ImportFileTabDataHandler(IGoogleSheetImportManager googleSheetImportManager, IFileImportManager fileImportManager, IFileImportRecordManager fileImportRecordManager, ILogger<ImportFileTabDataHandler> logger)
        {
            _googleSheetImportManager = googleSheetImportManager;
            _fileImportManager = fileImportManager;
            _fileImportRecordManager = fileImportRecordManager;
            _logger = logger;
        }

        public async Task<List<ImportFileTabRecord>> ImportGoogleSheetsAsync()
        {
            try
            {
                var googleImports = await _googleSheetImportManager.GetAll();

                if (!googleImports.Any())
                    return null;

                var googleCredential = GetGoogleCredential();

                var importFileTabRecords = new List<ImportFileTabRecord>();

                foreach (var googleImport in googleImports.Where(gi => !(gi.ImportUntil.HasValue && gi.GoogleSheetImports.Any(x => x.DateCreated > gi.ImportUntil.Value && x.FileStatus == FileStatus.Success))))
                {
                    var fileImport = new FileImport()
                    {
                        FileImportTypeId = googleImport.GoogleSheetImportTypeId,
                        FileStatus = FileStatus.Processing,
                        RecordType = googleImport.RecordType,
                        DateCreated = DateTimeHelper.GetDatabaseNow()
                    };
                    await _fileImportManager.AddAsync(fileImport);

                    try
                    {
                        var response = await GetGooleSheetValueRangeAsync(googleImport, googleCredential);

                        if (response.Values != null && response.Values.Count > 1)
                        {
                            if (googleImport.HasTitleRow)
                            {
                                var titleRow = response.Values.First();

                                if (titleRow.Any())
                                {
                                    var uniqueNameTitleRow = titleRow.Select(x => x.ToString()).ToList().MakeAllValuesUnique('_');
                                    var titleRowDict = uniqueNameTitleRow.ToDictionary(t => uniqueNameTitleRow.IndexOf(t), t => t);
                                    var fileImportRecords = new List<FileImportRecord>();
                                    foreach (var row in response.Values.Skip(1))
                                    {
                                        var rowDict = new Dictionary<string, object>();
                                        foreach (var titleColumn in titleRowDict)
                                        {
                                            rowDict.Add(titleColumn.Value.ToString(), row.Count > titleColumn.Key ? row[titleColumn.Key] : string.Empty);
                                        }
                                        importFileTabRecords.Add(new ImportFileTabRecord()
                                        {
                                            Row = response.Values.IndexOf(row) + 1,
                                            Data = rowDict
                                        });

                                        fileImportRecords.Add(new FileImportRecord()
                                        {
                                            Record = JsonConvert.SerializeObject(rowDict),
                                            Row = response.Values.IndexOf(row) + 1,
                                            FileImportId = fileImport.FileImportId
                                        });
                                    }

                                    await _fileImportRecordManager.AddMultipleAsync(fileImportRecords);
                                }
                            }

                            fileImport.FileStatus = FileStatus.Success;
                            await _fileImportManager.UpdateAsync(fileImport);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing the googleImport!");
                        fileImport.ErrorMessage = ex.Message;
                        fileImport.FileStatus = FileStatus.Failed;
                        await _fileImportManager.UpdateAsync(fileImport);
                    }
                }

                return importFileTabRecords;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "What is this shit!");
                throw;
            }

        }

        private GoogleCredential GetGoogleCredential()
        {
            GoogleCredential credential;
            try
            {
                using (var stream = new FileStream("googlesettings.json", FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Something went wrong while reading the googlesettings.json");
                throw;
            }

            return credential;
        }
        private async Task<ValueRange> GetGooleSheetValueRangeAsync(GoogleSheetImportType googleImport, GoogleCredential credential)
        {
            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = googleImport.GoogleSheetName
            });

            var range = $"{googleImport.TabName}!{googleImport.Range}";
            var request = service.Spreadsheets.Values.Get(googleImport.GoogleSheetId, range);
            request.PrettyPrint = true;
            return await request.ExecuteAsync();
        }

        
    }
}
