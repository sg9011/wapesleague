using Base.Bot.Services;
using Base.Bot.Services.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WaPesLeague.Business.Workflows.Interfaces;
using WaPesLeague.GoogleSheets.Interfaces;

namespace WaPesLeague.Bot.Services
{
    public class ProcessDailyFileImportsService : BaseBackgroundService<ProcessDailyFileImportsService>
    {
        private readonly IServiceProvider _provider;
        private const int delayMilliSeconds = 86400000; //24hours

        public ProcessDailyFileImportsService(ILogger<ProcessDailyFileImportsService> logger, IServiceProvider serviceProvider) : base(logger)
        {
            _provider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => Logger.LogDebug("ProcessDailyFileImportsService background service Register Call."));

            var startDelay = BackgroundTimerHelper.CalculateStartingDelay(new TimeSpan(7, 0, 0));
            await Task.Delay((int)startDelay.TotalMilliseconds);

            while (!stoppingToken.IsCancellationRequested)
            {
                var nowAtStartOfExecution = DateTime.UtcNow;
                try
                {
                    using (var scope = _provider.CreateScope())
                    {
                        var processFileImportWorkflow = scope.ServiceProvider.GetRequiredService<IProcessFileImportWorkflow>();
                        await processFileImportWorkflow.ProcessDailyFileImports();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "ExecuteAsync for ProcessDailFileImports catched the following error.");
                }


                await Task.Delay(BackgroundTimerHelper.TimeSpanInMilliSecondsTillNextRun(delayMilliSeconds, nowAtStartOfExecution), stoppingToken);
            }

            Logger.LogCritical("ProcessDailyFileImportsService will no longer repeat itself");

        }

    }
}
