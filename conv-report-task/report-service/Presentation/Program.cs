using Application.Extensions;
using Infrastructure.Data.Extensions;
using Infrastructure.Data.Options;
using Infrastructure.Kafka.Extensions;
using Presentation.Controllers;
using Presentation.Kafka.Extensions;
using Presentation.Services;
using Reports.Kafka.Contracts;

WebApplicationBuilder builder = WebApplication.CreateBuilder();

builder.Services
    .AddPostgres()
    .AddRedis(builder.Configuration)
    .AddMigrations()
    .AddPostgresOptions()
    .AddKafkaOptions()
    .AddKafkaInfrastructureConsumer<ReportCreationKey, ReportCreationValue>()
    .AddReportHandler()
    .AddPostgresRepository()
    .AddRedisRepository()
    .AddServices()
    .AddHostedService<MigrationService>();

builder.Services.AddGrpc();

WebApplication app = builder.Build();

app.MapGrpcService<ReportController>();

app.Run();
