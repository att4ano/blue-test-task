using FluentMigrator.Runner;
using Infrastructure.Kafka.Models;
using Infrastructure.Kafka.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using Npgsql.NameTranslation;

namespace Infrastructure.Kafka.Extensions;

public static class DataExtensions
{
    private static readonly INpgsqlNameTranslator Translator = new NpgsqlSnakeCaseNameTranslator();

    public static IServiceCollection AddDataAccess(this IServiceCollection collection)
    {
        var builder = new NpgsqlDataSourceBuilder();
        collection.AddSingleton<NpgsqlDataSource>(provider =>
        {
            IOptions<PostgresOptions> options = provider.GetRequiredService<IOptions<PostgresOptions>>();
            builder.ConnectionStringBuilder.Host = options.Value.Host;
            builder.ConnectionStringBuilder.Port = options.Value.Port;
            builder.ConnectionStringBuilder.Username = options.Value.Username;
            builder.ConnectionStringBuilder.Password = options.Value.Password;

            builder.MapEnum<MessageState>("persisted_message_state");   
            
            return builder.Build();
        });

        return collection;
    }

    public static IServiceCollection AddMigrations(this IServiceCollection collection)
    {
        collection.AddFluentMigratorCore()
            .ConfigureRunner(runner => runner
                .AddPostgres()
                .WithGlobalConnectionString(provider =>
                {
                    var options = provider.GetRequiredService<IOptions<PostgresOptions>>();
                    var connectionString = $"Host={options.Value.Host};Port={options.Value.Port};Username={options.Value.Username};Password={options.Value.Password}";
                    return connectionString;
                })
                .WithMigrationsIn(typeof(IMigrationAssemblyMarker).Assembly));

        return collection;
    }

    public static void DoMigrations(this IServiceProvider provider)
    {
        IMigrationRunner runner = provider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
}