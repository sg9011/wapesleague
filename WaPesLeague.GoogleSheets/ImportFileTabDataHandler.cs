using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.GoogleSheets.Interfaces;

namespace WaPesLeague.GoogleSheets
{
    public class ImportFileTabDataHandler : IImportFileTabDataHandler
    {
        static readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static SheetsService service;

        private readonly ILogger<ImportFileTabDataHandler> _logger;
        public ImportFileTabDataHandler(ILogger<ImportFileTabDataHandler> logger)
        {
            _logger = logger;
        }
        //fileName: Copy WAPES League player Database
        //tab: "Full player database "
        //fileId: 1oi3fjTz_tbfzTdL-Ev10J8UOpZNf-slyAx5RAwzjyOY
        public async Task<List<ImportFileTabRecord>> HandleAsync(string fileName, string tab, string fileId)
        {
            try
            {
                GoogleCredential credential;
                using (var stream = new FileStream("googlesettings.json", FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
                }

                service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = fileName
                });
                var range = $"{tab}!A1:AZ999";
                var request = service.Spreadsheets.Values.Get(fileId, range);
                request.PrettyPrint = true;
                var response = await request.ExecuteAsync();

                var importFileTabRecords = new List<ImportFileTabRecord>();
                if (response.Values != null && response.Values.Count > 1)
                {
                    var titleRow = response.Values.First();
                    if (titleRow.Any())
                    {
                        var titleRowDict = titleRow.ToDictionary(t => titleRow.IndexOf(t), t => t);
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
                        }
                    }
                }
                var json = JsonConvert.SerializeObject(importFileTabRecords);
                return importFileTabRecords;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong while importing tab: {tab} of file: {fileName}", ex);
            }


            return new List<ImportFileTabRecord>();
        }
    }
}
