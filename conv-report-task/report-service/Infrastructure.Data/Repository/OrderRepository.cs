using System.Data;
using System.Runtime.CompilerServices;
using Application.Abstractions.Entities;
using Application.Abstractions.Models;
using Application.Abstractions.Persistence;
using Npgsql;

namespace Infrastructure.Data.Repository;

public class OrderRepository : IOrderRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public OrderRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async IAsyncEnumerable<OrderEntity> QueryAsync(
        OrderQuery query, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = @"
            select order_id, product_id, quantity, paid_at, amount
            from orders
            where (cardinality(@ids) = 0 or order_id = any (@ids)) and
                  (cardinality(@product_ids) = 0 or product_id = any (@product_ids)) and
                  (paid_at >= @start_period or @start_period is null) and
                  (paid_at <= @end_period or @end_period is null)
            order by paid_at;";

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        
        command.Parameters.Add(new NpgsqlParameter("ids", query.Ids));
        command.Parameters.Add(new NpgsqlParameter("product_ids", query.ProductIds));
        command.Parameters.Add(new NpgsqlParameter("start_period", query.StartPeriod));
        command.Parameters.Add(new NpgsqlParameter("end_period", query.EndPeriod));

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new OrderEntity(
                    Id: reader.GetInt64("order_id"),
                    ProductId: reader.GetInt64("product_id"),
                    Quantity: reader.GetInt64("quantity"),
                    PayedAt: reader.GetDateTime("paid_at"),
                    TotalPrice: reader.GetDecimal("amount"));
        }
    }
}