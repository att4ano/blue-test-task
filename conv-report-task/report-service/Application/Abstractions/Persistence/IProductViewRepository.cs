using Application.Abstractions.Entities;
using Application.Abstractions.Models;

namespace Application.Abstractions.Persistence;

public interface IProductViewRepository
{
    IAsyncEnumerable<ViewEntity> QueryAsync(ViewQuery query, CancellationToken cancellationToken);
}