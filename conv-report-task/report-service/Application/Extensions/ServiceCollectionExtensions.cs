using Application.Application;
using Application.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection collection)
    {
        collection.AddScoped<IReportService, ReportService>();
        return collection;
    }
}