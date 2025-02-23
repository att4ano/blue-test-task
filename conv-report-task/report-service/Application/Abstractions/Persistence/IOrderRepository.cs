using Application.Abstractions.Entities;
using Application.Abstractions.Models;

namespace Application.Abstractions.Persistence;

public interface IOrderRepository
{
    IAsyncEnumerable<OrderEntity> QueryAsync(OrderQuery query, CancellationToken cancellationToken);
}