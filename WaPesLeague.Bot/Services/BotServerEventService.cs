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
    public class BotServerEventService : BaseBackgroundService<BotServerEventService>
    {
        private readonly IServiceProvider _provider;
        private const int delayMilliSeconds = 43200000; //12hours

        public BotServerEventService(ILogger<BotServerEventService> logger, IServiceProvider serviceProvider) : base(logger)
        {
            _provider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => Logger.LogDebug("BotServerEventService background service Register Call."));
            await Task.Delay(720000); //12min
            while (!stoppingToken.IsCancellationRequested)
            {
                var nowAtStartOfExecution = DateTime.UtcNow;
                try
                {
                    using (var scope = _provider.CreateScope())
                    {
                        var serverWorkflow = scope.ServiceProvider.GetRequiredService<IServerWorkflow>();
                        await serverWorkflow.HandleServerEventsAndActionsAsync();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "ExecuteAsync for HandleServerEventsAndActionsAsync catched the following error.");
                }


                await Task.Delay(BackgroundTimerHelper.TimeSpanInMilliSecondsTillNextRun(delayMilliSeconds, nowAtStartOfExecution), stoppingToken);
            }

            Logger.LogCritical("BotServerEventService will no longer repeat itself");
        }
    }
}
