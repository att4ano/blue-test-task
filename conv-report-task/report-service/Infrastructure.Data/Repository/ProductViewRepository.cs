using System.Data;
using System.Runtime.CompilerServices;
using Application.Abstractions.Entities;
using Application.Abstractions.Models;
using Application.Abstractions.Persistence;
using Npgsql;

namespace Infrastructure.Data.Repository;

public class ProductViewRepository : IProductViewRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public ProductViewRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async IAsyncEnumerable<ViewEntity> QueryAsync(
        ViewQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = @"select view_id, product_id, viewed_at
                             from product_views
                            where (cardinality(:ids) = 0 or view_id = any (:ids)) and
                                (cardinality(:product_ids) = 0 or product_id = any (:product_ids)) and
                                (viewed_at >= :start_period or :start_period is null) and
                                (viewed_at <= :end_period or :end_period is null)
                            order by viewed_at;";

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.Add(new NpgsqlParameter("ids", query.Ids));
        command.Parameters.Add(new NpgsqlParameter("product_ids", query.ProductIds));
        command.Parameters.Add(new NpgsqlParameter("start_period", query.StartPeriod));
        command.Parameters.Add(new NpgsqlParameter("end_period", query.EndPeriod));

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new ViewEntity(
                Id: reader.GetInt64("view_id"),
                ProductId: reader.GetInt64("product_id"),
                ViewedAt: reader.GetDateTime("viewed_at")
            );
        }
    }
}