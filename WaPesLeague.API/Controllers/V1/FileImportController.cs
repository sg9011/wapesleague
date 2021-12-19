using AutoMapper;
using Base.Bot.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WaPesLeague.API.Controllers.V1.Requests;
using WaPesLeague.Business.Workflows.Interfaces;
using WaPesLeague.GoogleSheets.Interfaces;

namespace WaPesLeague.API.Controllers.V1
{
    [ApiVersion("1.0")]
    public class FileImportController : BaseApiController
    {
        private readonly IImportFileTabDataHandler _importFileTabDataHandler;
        private readonly IProcessFileImportWorkflow _processFileImportWorkflow;
        public FileImportController(IImportFileTabDataHandler importFileTabDataHandler, IProcessFileImportWorkflow processFileImportWorkflow, IMapper mapper) : base(mapper)
        {
            _importFileTabDataHandler = importFileTabDataHandler;
            _processFileImportWorkflow = processFileImportWorkflow;
        }

        //fileName: Copy WAPES League player Database
        //tab: "Full player database "
        //fileId: 1oi3fjTz_tbfzTdL-Ev10J8UOpZNf-slyAx5RAwzjyOY
        //[HttpPost]
        //public async Task<IActionResult> ImportFileAsync([FromBody] FileImportRequest request)
        //{
        //    var result = await _importFileTabDataHandler.HandleAsync("Copy WAPES Legends S04 Div 1", "Results", "1Abum30ooJyESOy6YGlcZLBKmRahujH1lnifeehPl_JY");
        //    //var result = await _importFileTabDataHandler.HandleAsync("V3 live update (devs copy)", "V3 global min/max", "1c89tdesdBy3P6qpY-5jmuiAIUX_sB44B4GyhpXFKYD8");
        //    return Ok("All Good!");
        //}


        [HttpPost]
        public async Task<IActionResult> ImportGoogleSheetFilesAsync()
        {
            var result = await _importFileTabDataHandler.ImportGoogleSheetsAsync();
            return Ok("All Good!");
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessFileImportAsync([FromBody] int fileImportId)
        {
            await _processFileImportWorkflow.ProcessFileImportAsync(fileImportId);
            return Ok("This is the Shit");
        }
    }
}
