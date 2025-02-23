using Infrastructure.Kafka.Extensions;
using Infrastructure.Kafka.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Kafka.Handlers;
using Presentation.Kafka.Services;
using Reports.Kafka.Contracts;

namespace Presentation.Kafka.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddReportHandler(this IServiceCollection collection)
    {
        collection.AddScoped<IConsumerHandler<ReportCreationKey, ReportCreationValue>, ReportCreationHandler>();
        collection.AddScoped<ConsumerMessageHandler<ReportCreationKey, ReportCreationValue>>();
        collection.AddHostedService<KafkaConsumerBackgroundService<ReportCreationKey, ReportCreationValue>>();
        
        return collection;
    }
}