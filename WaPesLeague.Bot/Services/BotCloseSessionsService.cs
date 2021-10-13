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
    public class BotCloseSessionsService : BaseBackgroundService<BotCloseSessionsService>
    {
        private readonly IServiceProvider _provider;
        private const int delayMilliSeconds = 1800000; //30Minutes

        public BotCloseSessionsService(ILogger<BotCloseSessionsService> logger, IServiceProvider serviceProvider) : base(logger)
        {
            _provider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => Logger.LogDebug("BotCloseSessionsService background service Register Call."));
            while (!stoppingToken.IsCancellationRequested)
            {
                var nowAtStartOfExecution = DateTime.Now;
                try
                {
                    using (var scope = _provider.CreateScope())
                    {
                        var mixGroupWorkflow = scope.ServiceProvider.GetRequiredService<IMixGroupWorkflow>();
                        await mixGroupWorkflow.HandleAutoCloseAndRecreateOfMixSesisons();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "ExecuteAsync for HandleAutoCloseAndRecreateOfMixSesisons catched the following error.");
                }


                await Task.Delay(BackgroundTimerHelper.TimeSpanInMilliSecondsTillNextRun(delayMilliSeconds, nowAtStartOfExecution), stoppingToken);
            }

            Logger.LogCritical("BotCloseSessionsService will no longer repeat itself");
        }
    }
}
