using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Base.Bot.Services
{
    public abstract class BaseBackgroundService<T> : IHostedService, IDisposable where T : BaseBackgroundService<T>
    {
        private Task _executingTask;

        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
        protected readonly ILogger<T> Logger;

        protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

        public BaseBackgroundService(ILogger<T> logger)
        {
            Logger = logger;
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogDebug("Start Async");
            _executingTask = ExecuteAsync(_stoppingCts.Token);
            if (_executingTask.IsCompleted)
            {
                Logger.LogDebug("executing task completed");
                return _executingTask;
            }

            return Task.CompletedTask;
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogDebug("StopAsync");
            if (_executingTask != null)
            {
                try
                {
                    _stoppingCts.Cancel();
                    Logger.LogWarning("Cancellation Token Cancelled");
                }
                finally
                {
                    await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));
                }
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _stoppingCts.Cancel();
                _stoppingCts.Dispose();
            }
        }
    }
}
