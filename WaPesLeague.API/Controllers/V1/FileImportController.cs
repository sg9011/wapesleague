using AutoMapper;
using Base.Bot.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WaPesLeague.API.Controllers.V1.Requests;
using WaPesLeague.GoogleSheets;
using WaPesLeague.GoogleSheets.Interfaces;

namespace WaPesLeague.API.Controllers.V1
{
    [ApiVersion("1.0")]
    public class FileImportController : BaseApiController
    {
        private readonly IImportFileTabDataHandler _importFileTabDataHandler;
        public FileImportController(IImportFileTabDataHandler importFileTabDataHandler, IMapper mapper) : base(mapper)
        {
            _importFileTabDataHandler = importFileTabDataHandler;
        }

        //fileName: Copy WAPES League player Database
        //tab: "Full player database "
        //fileId: 1oi3fjTz_tbfzTdL-Ev10J8UOpZNf-slyAx5RAwzjyOY
        [HttpPost]
        public async Task<IActionResult> ImportFileAsync([FromBody] FileImportRequest request)
        {
            //var result = await _importFileTabDataHandler.HandleAsync("Copy WAPES League player Database", "Full player database", "1oi3fjTz_tbfzTdL-Ev10J8UOpZNf-slyAx5RAwzjyOY");
            var result = await _importFileTabDataHandler.HandleAsync("V3 live update (devs copy)", "V3 global min/max", "1c89tdesdBy3P6qpY-5jmuiAIUX_sB44B4GyhpXFKYD8");
            return Ok("All Good!");
        }
    }
}
