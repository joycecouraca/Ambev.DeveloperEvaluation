using Ambev.DeveloperEvaluation.Domain.Abstractions;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public record SaleCreatedDomainEvent(Guid SaleId, DateTime CreatedAt) : IDomainEvent;
