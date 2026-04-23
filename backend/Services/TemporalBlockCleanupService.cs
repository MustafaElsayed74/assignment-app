using BlockedCountriesApi.Repositories;

namespace BlockedCountriesApi.Services;

/// <summary>
/// Background service that runs periodically to remove expired temporal blocks.
/// </summary>
public class TemporalBlockCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TemporalBlockCleanupService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5);

    public TemporalBlockCleanupService(IServiceProvider serviceProvider, ILogger<TemporalBlockCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Temporal Block Cleanup Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // We need to resolve scoped/singleton services from the provider if needed
                // Here, repository is singleton, but using scope is good practice
                using (var scope = _serviceProvider.CreateScope())
                {
                    var repository = scope.ServiceProvider.GetRequiredService<ITemporalBlockRepository>();
                    int removedCount = repository.RemoveExpired();
                    
                    if (removedCount > 0)
                    {
                        _logger.LogInformation("Removed {Count} expired temporal blocks.", removedCount);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing temporal block cleanup.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("Temporal Block Cleanup Service is stopping.");
    }
}
