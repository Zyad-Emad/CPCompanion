using CPCompanion.Application.Interfaces;
using CPCompanion.Domain.Entities;
using CPCompanion.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPCompanion.Infrastructure.BackgroundServices
{
    public class CodeforcesSyncBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CodeforcesSyncBackgroundService> _logger;
        private readonly TimeSpan _interval;

        public CodeforcesSyncBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<CodeforcesSyncBackgroundService> logger,
            IOptions<BackgroundServiceSettings> settings)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _interval = TimeSpan.FromHours(settings.Value.SyncIntervalHours);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Starting Codeforces sync cycle at {Time}", DateTime.UtcNow);

                   await SyncAllUsersAsync(stoppingToken);

                    _logger.LogInformation("Sync cycle completed successfully.");
                }
                catch (OperationCanceledException)
                     when (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation(
                        "Codeforces sync service is stopping.");

                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during the sync cycle.");
                }
                try
                {
                    await Task.Delay(_interval, stoppingToken);
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }
        private async Task SyncAllUsersAsync(
    CancellationToken cancellationToken)
        {
            List<Guid> userIds;

            await using (var scope = _scopeFactory.CreateAsyncScope())
            {
                var context = scope.ServiceProvider
                    .GetRequiredService<AppDbContext>();

                userIds = await context.UserProfiles
                    .AsNoTracking()
                    .Select(u => u.Id)
                    .ToListAsync(cancellationToken);
            }

            foreach (var userId in userIds)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    await using var scope =
                        _scopeFactory.CreateAsyncScope();

                    var syncService = scope.ServiceProvider
                        .GetRequiredService<ISyncService>();

                    await syncService.SyncUserAsync(
                        userId,
                        cancellationToken);
                }
                catch (OperationCanceledException)
                    when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "An error occurred while syncing user {UserId}",
                        userId);
                }
            }
        }
        
    }
    public class BackgroundServiceSettings
    {
        public int SyncIntervalHours { get; set; }
    }
}
