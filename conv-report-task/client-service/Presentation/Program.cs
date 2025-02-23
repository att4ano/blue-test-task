using Application.Extensions;
using Infrastructure.Kafka.Extensions;
using Presentation.Extensions;
using Presentation.Middlewares;
using Reports.Kafka.Contracts;

WebApplicationBuilder builder = WebApplication.CreateBuilder();

builder.Services
    .AddDataOptions()
    .AddDataAccess()
    .AddMigrations()
    .AddKafkaOptions()
    .AddKafkaProducer<ReportCreationKey, ReportCreationValue>()
    .AddOutbox<ReportCreationKey, ReportCreationValue>()
    .AddEventPublisher()
    .AddGrpcOptions()
    .AddGrpcClient()
    .AddServices()
    .AddScoped<ExceptionFormatMiddleware>()
    .AddRateLimit(builder.Configuration)
    .AddControllers();

builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

app.MapControllers();
app.UseMiddleware<ExceptionFormatMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
