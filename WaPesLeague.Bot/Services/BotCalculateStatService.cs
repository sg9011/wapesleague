using Base.Bot.Services;
using Base.Bot.Services.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WaPesLeague.Business.Workflows.Interfaces;

namespace WaPesLeague.Bot.Services
{
    public class BotCalculateStatService : BaseBackgroundService<BotCalculateStatService>
    {
        private readonly IServiceProvider _provider;
        private const int delayMilliSeconds = 43200000; //12hours

        public BotCalculateStatService(ILogger<BotCalculateStatService> logger, IServiceProvider serviceProvider) : base(logger)
        {
            _provider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => Logger.LogDebug("BotCalculateStatService background service Register Call."));
            await Task.Delay(120000); //2min
            while (!stoppingToken.IsCancellationRequested)
            {
                var nowAtStartOfExecution = DateTime.UtcNow;
                try
                {
                    using (var scope = _provider.CreateScope())
                    {
                        var mixStatsflow = scope.ServiceProvider.GetRequiredService<IMixStatsWorkflow>();
                        await mixStatsflow.HandleCalculatedNotCalculatedSessions();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "ExecuteAsync for HandleNotCalculatedSessions catched the following error.");
                }


                await Task.Delay(BackgroundTimerHelper.TimeSpanInMilliSecondsTillNextRun(delayMilliSeconds, nowAtStartOfExecution), stoppingToken);
            }

            Logger.LogCritical("BotCalculateStatService will no longer repeat itself");
        }
    }
}
