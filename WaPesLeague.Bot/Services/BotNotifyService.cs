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
    public class BotNotifyService : BaseBackgroundService<BotNotifyService>
    {
        private readonly IServiceProvider _provider;
        private const int delayMilliSeconds = 600000; //10Minutes

        public BotNotifyService(ILogger<BotNotifyService> logger, IServiceProvider serviceProvider) : base(logger)
        {
            _provider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => Logger.LogDebug("BotNotifyService background service Register Call."));
            while (!stoppingToken.IsCancellationRequested)
            {
                var nowAtStartOfExecution = DateTime.UtcNow;
                try
                {
                    using (var scope = _provider.CreateScope())
                    {
                        var mixSessionWorkflow = scope.ServiceProvider.GetRequiredService<IMixSessionWorkflow>();
                        await mixSessionWorkflow.HandleNotificationsOfMixSessionsAsync();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "ExecuteAsync for HandleNotificationsOfMixSessionsAsync catched the following error.");
                }

                await Task.Delay(BackgroundTimerHelper.TimeSpanInMilliSecondsTillNextRun(delayMilliSeconds, nowAtStartOfExecution), stoppingToken);
            }

            Logger.LogCritical("BotNotifyService will no longer repeat itself");
        }
    }
}
