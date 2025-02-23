using Infrastructure.Data.Extensions;

namespace Presentation.Services;

public class MigrationService : BackgroundService
{
    private readonly IServiceScopeFactory _factory;

    public MigrationService(IServiceScopeFactory factory)
    {
        _factory = factory;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ExecuteMigrationsAsync(stoppingToken);
    }

    private async Task ExecuteMigrationsAsync(CancellationToken stoppingToken)
    {
        using var scope = _factory.CreateScope();
        scope.ServiceProvider.DoMigrations();
        await Task.CompletedTask;
    }
}