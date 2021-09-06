using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TrashPandaBot
{
    public abstract class CriticalBackgroundService : BackgroundService
    {
        private readonly IHostApplicationLifetime _lifetime;
        private readonly ILogger<CriticalBackgroundService> _logger;

        public CriticalBackgroundService(IHostApplicationLifetime lifetime, ILogger<CriticalBackgroundService> logger)
        {
            _lifetime = lifetime;
            _logger = logger;
        }

        public abstract Task DoWork(CancellationToken cancellationToken);

        protected override Task ExecuteAsync(CancellationToken cancellationToken) => Task.Run(async () =>
        {
            try
            {
                await DoWork(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "A critical service has failed, exiting.");
            }
            finally
            {
                _lifetime.StopApplication();
            }
        });
    }
}
