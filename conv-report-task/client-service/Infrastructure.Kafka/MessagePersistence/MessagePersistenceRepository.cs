using System.Data;
using System.Runtime.CompilerServices;
using Infrastructure.Kafka.MessagePersistence.Queries;
using Infrastructure.Kafka.Models;
using Npgsql;

namespace Infrastructure.Kafka.MessagePersistence;

public class MessagePersistenceRepository : IMessagePersistenceRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public MessagePersistenceRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async IAsyncEnumerable<PersistenceKafkaMessage> QueryAsync(
        PersistenceMessageQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        const string sql = @"
            select persisted_message_id,
                   persisted_message_name,
                   persisted_message_created_at,
                   persisted_message_state,
                   persisted_message_key,
                   persisted_message_value
            from persisted_messages
            where (persisted_message_name = @message_name)
              and (cardinality(@states) = 0 or persisted_message_state = any (@states))
              and (@ignore_cursor or persisted_message_id >= @cursor)
            order by persisted_message_created_at
            limit @page_size";

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("message_name", query.Name));
        command.Parameters.Add(new NpgsqlParameter("states", query.States));
        command.Parameters.Add(new NpgsqlParameter("ignore_cursor", query.Cursor is null));
        command.Parameters.Add(new NpgsqlParameter("cursor", query.Cursor ?? 0));
        command.Parameters.Add(new NpgsqlParameter("page_size", query.PageSize));

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new PersistenceKafkaMessage
            {
                Id = reader.GetInt64("persisted_message_id"),
                TopicName = reader.GetString("persisted_message_name"),
                CreatedAt = reader.GetFieldValue<DateTimeOffset>("persisted_message_created_at"),
                Key = reader.GetString("persisted_message_key"),
                Value = reader.GetString("persisted_message_value"),
                State = reader.GetFieldValue<MessageState>("persisted_message_state"),
            };
        }
    }

    public IAsyncEnumerable<PersistenceKafkaMessage> QueryAllPending(string topicName,
        CancellationToken cancellationToken)
    {
        var query = new PersistenceMessageQuery(
            Name: topicName,
            States: [MessageState.Pending],
            Cursor: null,
            PageSize: int.MaxValue
        );

        return QueryAsync(query, cancellationToken);
    }

    public async Task AddAsync(IReadOnlyCollection<PersistenceKafkaMessage> messages,
        CancellationToken cancellationToken)
    {
        const string sql = @"
        insert into persisted_messages
                    (persisted_message_name,
                     persisted_message_created_at,
                     persisted_message_key,
                     persisted_message_value)
        select * from unnest(@names::text[], @created_at::timestamp[], @keys::jsonb[], @values::jsonb[])";

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("names", messages.Select(message => message.TopicName).ToArray()));
        command.Parameters.Add(new NpgsqlParameter("created_at", messages.Select(message => message.CreatedAt.ToUniversalTime()).ToArray()));
        command.Parameters.Add(new NpgsqlParameter("keys", messages.Select(message => message.Key).ToArray()));
        command.Parameters.Add(new NpgsqlParameter("values", messages.Select(message => message.Value).ToArray()));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }


    public async Task UpdateAsync(IReadOnlyCollection<PersistenceKafkaMessage> messages,
        CancellationToken cancellationToken)
    {
        const string sql = @"
        update persisted_messages
        set persisted_message_state = source.state
        from (select * from unnest(@ids, @states)) as source(id, state)
        where persisted_message_id = source.id";

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("ids", messages.Select(x => x.Id).ToArray()));
        command.Parameters.Add(new NpgsqlParameter("states", messages.Select(x => x.State).ToArray()));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}