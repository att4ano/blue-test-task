using FluentMigrator.Runner;
using FluentMigrator.Runner.Processors.Postgres;
using Infrastructure.Data.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using Npgsql.NameTranslation;

namespace Infrastructure.Data.Extensions;

public static class DataExtensions
{
    private static readonly INpgsqlNameTranslator Translator = new NpgsqlSnakeCaseNameTranslator();

    public static IServiceCollection AddPostgres(this IServiceCollection collection)
    {
        var builder = new NpgsqlDataSourceBuilder();
        collection.AddSingleton<NpgsqlDataSource>(provider =>
        {
            IOptions<Options.PostgresOptions> options = provider.GetRequiredService<IOptions<Options.PostgresOptions>>();
            builder.ConnectionStringBuilder.Host = options.Value.Host;
            builder.ConnectionStringBuilder.Port = options.Value.Port;
            builder.ConnectionStringBuilder.Username = options.Value.Username;
            builder.ConnectionStringBuilder.Password = options.Value.Password;

            return builder.Build();
        });

        return collection;
    }   

    public static IServiceCollection AddRedis(this IServiceCollection collection, ConfigurationManager manager)
    {
        collection.AddStackExchangeRedisCache(options =>
        {
            var redisSection = manager.GetSection("DataAccess:Redis");
            options.Configuration = $"{redisSection["Host"]}:{redisSection["Port"]}";
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
                    var options = provider.GetRequiredService<IOptions<Options.PostgresOptions>>();
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