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
    public class BotScanForServerMembersService : BaseBackgroundService<BotScanForServerMembersService>
    {
        private readonly IServiceProvider _provider;
        private const int delayMilliSeconds = 86400000; //24hours

        public BotScanForServerMembersService(ILogger<BotScanForServerMembersService> logger, IServiceProvider serviceProvider) : base(logger)
        {
            _provider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => Logger.LogDebug("BotScanForServerMembersService background service Register Call."));

            var startDelay = BackgroundTimerHelper.CalculateStartingDelay(new TimeSpan(5, 0, 0));
            await Task.Delay((int)startDelay.TotalMilliseconds);

            while (!stoppingToken.IsCancellationRequested)
            {
                var nowAtStartOfExecution = DateTime.UtcNow;
                try
                {
                    using (var scope = _provider.CreateScope())
                    {
                        var discordWorkflow = scope.ServiceProvider.GetRequiredService<IDiscordWorkflow>();
                        await discordWorkflow.HandleScanForMembersAsync();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "ExecuteAsync for ScanForMembersAsync catched the following error.");
                }


                await Task.Delay(BackgroundTimerHelper.TimeSpanInMilliSecondsTillNextRun(delayMilliSeconds, nowAtStartOfExecution), stoppingToken);
            }

            Logger.LogCritical("BotScanForServerMembersService will no longer repeat itself");
        }
    }
}
