namespace Application.Abstractions.Entities;

public record OrderEntity(long Id, long ProductId, long Quantity, DateTime PayedAt, decimal TotalPrice);