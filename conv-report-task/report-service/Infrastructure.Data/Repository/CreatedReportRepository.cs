using Application.Abstractions.Cache;
using Application.Abstractions.Entities;

namespace Infrastructure.Data.Repository;

using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

public class CreatedReportRepository : ICreatedReportRepository
{
    private readonly IDistributedCache _distributedCache;

    public CreatedReportRepository(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task Add(ReportEntity report, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(report);
        var cacheKey = $"report:{report.Id}";
        await _distributedCache.SetStringAsync(cacheKey, json, cancellationToken);
    }

    public async Task<ReportEntity?> Get(Guid reportId, CancellationToken cancellationToken)
    {
        var cacheKey = $"report:{reportId}";
        var json = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);
        
        return json == null ? null : JsonSerializer.Deserialize<ReportEntity>(json);
    }
}