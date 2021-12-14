using Base.Bot.Services;
using Base.Bot.Services.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WaPesLeague.GoogleSheets.Interfaces;

namespace WaPesLeague.Bot.Services
{
    public class ImportDailyGoogleSheetsService : BaseBackgroundService<ImportDailyGoogleSheetsService>
    {
        private readonly IServiceProvider _provider;
        private const int delayMilliSeconds = 86400000; //24hours

        public ImportDailyGoogleSheetsService(ILogger<ImportDailyGoogleSheetsService> logger, IServiceProvider serviceProvider) : base(logger)
        {
            _provider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => Logger.LogDebug("ImportDailyGoogleSheetsService background service Register Call."));

            var startDelay = BackgroundTimerHelper.CalculateStartingDelay(new TimeSpan(6, 0, 0));
            await Task.Delay((int)startDelay.TotalMilliseconds);

            while (!stoppingToken.IsCancellationRequested)
            {
                var nowAtStartOfExecution = DateTime.UtcNow;
                try
                {
                    using (var scope = _provider.CreateScope())
                    {
                        var importFileTabDataHandler = scope.ServiceProvider.GetRequiredService<IImportFileTabDataHandler>();
                        await importFileTabDataHandler.ImportGoogleSheetsAsync(); ;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "ExecuteAsync for ImportDailyGoogleSheets catched the following error.");
                }


                await Task.Delay(BackgroundTimerHelper.TimeSpanInMilliSecondsTillNextRun(delayMilliSeconds, nowAtStartOfExecution), stoppingToken);
            }

            Logger.LogCritical("ImportDailyGoogleSheetsService will no longer repeat itself");

        }
    }
}
