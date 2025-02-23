using System.Data;
using System.Runtime.CompilerServices;
using Application.Abstractions.Entities;
using Application.Abstractions.Models;
using Application.Abstractions.Persistence;
using Npgsql;

namespace Infrastructure.Data.Repository;

public class ReportRepository : IReportRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public ReportRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async IAsyncEnumerable<ReportEntity> QueryAsync(
        ReportQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = @"select report_id, product_id, start_period, end_period, view_to_payment_ratio, payment_count, view_count
        from reports
        where (cardinality(:ids) = 0 or report_id = any (:ids))
        order by start_period;";

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        
        command.Parameters.Add(new NpgsqlParameter("ids", query.Ids));

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new ReportEntity(
                Id: reader.GetGuid("report_id"),
                ProductId: reader.GetInt64("product_id"),
                StartPeriod: reader.GetDateTime("start_period"),
                EndPeriod: reader.GetDateTime("end_period"),
                Ratio: reader.GetDouble("view_to_payment_ratio"),
                PaymentCount: reader.GetInt32("payment_count"),
                ViewCount: reader.GetInt32("view_count")
            );
        }
    }

    public async Task AddAsync(IReadOnlyCollection<InsertReportModel> reports, CancellationToken cancellationToken)
    {
        const string sql = @"
            insert into reports (report_id, product_id, start_period, end_period, view_to_payment_ratio, payment_count, view_count, created_at)
            select * from unnest(
                @ids::uuid[],
                @product_ids::bigint[],
                @start_periods::timestamp[],
                @end_periods::timestamp[],
                @ratios::double precision[],
                @payment_counts::integer[],
                @view_counts::integer[],
                array_fill(now(), array[array_length(@ids, 1)])
            ) as source(id, product_id, start_period, end_period, ratio, payment_count, view_count);";

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("ids", reports.Select(r => r.Id).ToArray());
        command.Parameters.AddWithValue("product_ids", reports.Select(r => r.ProductId).ToArray());
        command.Parameters.AddWithValue("start_periods", reports.Select(r => r.StartPeriod).ToArray());
        command.Parameters.AddWithValue("end_periods", reports.Select(r => r.EndPeriod).ToArray());
        command.Parameters.AddWithValue("ratios", reports.Select(r => r.Ratio).ToArray());
        command.Parameters.AddWithValue("payment_counts", reports.Select(r => r.PaymentCount).ToArray());
        command.Parameters.AddWithValue("view_counts", reports.Select(r => r.ViewCount).ToArray());

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

}


