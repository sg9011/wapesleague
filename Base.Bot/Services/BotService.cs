using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Base.Bot.Services
{
    public class BotService : BaseBackgroundService<BotService>
    {
        private readonly IServiceProvider _provider;

        public BotService(ILogger<BotService> logger, IServiceProvider serviceProvider) : base(logger)
        {
            _provider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => Logger.LogDebug("BotService background service Register Call."));
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _provider.CreateScope())
                    {
                        var bot = scope.ServiceProvider.GetRequiredService<Bot>();
                        await bot.RunAsync();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "ExecuteAsync for running the BOT catched the following error.");
                }


                await Task.Delay(TimeSpan.FromMilliseconds(100), stoppingToken);
            }

            Logger.LogCritical("BotService will no longer repeat itself");
        }
    }
}
