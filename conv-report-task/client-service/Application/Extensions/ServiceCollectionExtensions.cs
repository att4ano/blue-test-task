using Application.Contracts;
using Application.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ReportService = Application.Application.ReportService;

namespace Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGrpcOptions(this IServiceCollection collection)
    {
        collection.AddOptions<GrpcClientOptions>().BindConfiguration("Grpc");
        return collection;
    }
    
    public static IServiceCollection AddGrpcClient(this IServiceCollection collection)
    {
        collection.AddGrpcClient<Reports.Service.ReportService.ReportServiceClient>((provider, o) =>
        {
            o.Address = new Uri(provider.GetRequiredService<IOptions<GrpcClientOptions>>().Value.Url);
        });

        return collection;
    }
    public static IServiceCollection AddServices(this IServiceCollection collection)
    {
        collection.AddScoped<IReportService, ReportService>();
        return collection;
    }
}