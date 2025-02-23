using Infrastructure.Kafka.MessagePersistence.Queries;
using Infrastructure.Kafka.Models;

namespace Infrastructure.Kafka.MessagePersistence;

public interface IMessagePersistenceRepository
{
    IAsyncEnumerable<PersistenceKafkaMessage> QueryAsync(PersistenceMessageQuery query, CancellationToken cancellationToken);

    IAsyncEnumerable<PersistenceKafkaMessage> QueryAllPending(string topicName, CancellationToken cancellationToken);

    Task AddAsync(IReadOnlyCollection<PersistenceKafkaMessage> messages, CancellationToken cancellationToken);

    Task UpdateAsync(IReadOnlyCollection<PersistenceKafkaMessage> messages, CancellationToken cancellationToken);
}