using Infrastructure.Kafka.Models;

namespace Infrastructure.Kafka.MessagePersistence.Queries;

public record PersistenceMessageQuery(
    string Name,
    MessageState[] States,
    long? Cursor,
    long PageSize);