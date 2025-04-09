using Ambev.DeveloperEvaluation.Domain.Abstractions;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public record SaleCancelledDomainEvent(Guid SaleId, DateTime CancelledAt) : IDomainEvent;
