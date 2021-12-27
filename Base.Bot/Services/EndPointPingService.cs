using Base.Bot.Infrastructure;
using Base.Bot.Services.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Base.Bot.Services
{
    public class EndPointPingService : BaseBackgroundService<EndPointPingService>
    {
        private readonly IServiceProvider _provider;
        private const int delayMilliSeconds = 600000; //10Minutes

        public EndPointPingService(ILogger<EndPointPingService> logger, IServiceProvider serviceProvider) : base(logger) {
            _provider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => Logger.LogDebug("EndPointPingService background service Register Call."));
            await Task.Delay(BackgroundTimerHelper.TimeSpanInMilliSecondsTillNextRun(delayMilliSeconds / 10, DateTime.UtcNow), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                var nowAtStartOfExecution = DateTime.UtcNow;
                try
                {
                    using (var scope = _provider.CreateScope())
                    {
                        var pingSettings = scope.ServiceProvider.GetRequiredService<PingSettings>();
                        string baseUrl = pingSettings.Url;
                        using (HttpClient client = new HttpClient())
                        {
                            using (HttpResponseMessage res = await client.GetAsync(baseUrl))
                            {
                                using (HttpContent content = res.Content)
                                {
                                    string data = await content.ReadAsStringAsync();
                                    if (data != null)
                                    {
                                        Logger.LogInformation($"Result of the ping: {data}");
                                    }
                                }
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Pinging ourself catched the following error.");
                }

                await Task.Delay(BackgroundTimerHelper.TimeSpanInMilliSecondsTillNextRun(delayMilliSeconds, nowAtStartOfExecution), stoppingToken);
            }

            Logger.LogWarning("BotPingService will no longer repeat itself");
        }
    }
}
