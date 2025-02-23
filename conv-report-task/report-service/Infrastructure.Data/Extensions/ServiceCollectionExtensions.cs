using Application.Abstractions.Cache;
using Application.Abstractions.Persistence;
using Infrastructure.Data.Options;
using Infrastructure.Data.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Data.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPostgresOptions(this IServiceCollection collection)
    {
        collection.AddOptions<PostgresOptions>().BindConfiguration("DataAccess:Postgres");
        return collection;
    }
    
    public static IServiceCollection AddPostgresRepository(this IServiceCollection collection)
    {
        collection.AddScoped<IOrderRepository, OrderRepository>();
        collection.AddScoped<IProductViewRepository, ProductViewRepository>();
        collection.AddScoped<IReportRepository, ReportRepository>();

        return collection;
    }

    public static IServiceCollection AddRedisRepository(this IServiceCollection collection)
    {
        collection.AddScoped<ICreatedReportRepository, CreatedReportRepository>();
        return collection;
    }
}