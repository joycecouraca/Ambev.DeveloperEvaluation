using Ambev.DeveloperEvaluation.Domain.Abstractions;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public record ItemCancelledDomainEvent(Guid SaleId, List<Guid> ItemId, DateTime CancelledAt) : IDomainEvent;
