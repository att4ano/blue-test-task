using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Presentation.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRateLimit(this IServiceCollection collection, ConfigurationManager manager)
    {
        var rateLimiterSection = manager.GetSection("RateLimiter");
        collection.AddRateLimiter(o =>
        {
            o.AddFixedWindowLimiter(policyName: "fixed", options =>
            {
                options.PermitLimit = int.Parse(rateLimiterSection["PermitLimit"]);
                options.Window = TimeSpan.FromSeconds(int.Parse(rateLimiterSection["WindowSeconds"]));
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.QueueLimit = int.Parse(rateLimiterSection["QueueLimit"]);
            });
        });

        return collection;
    }
}