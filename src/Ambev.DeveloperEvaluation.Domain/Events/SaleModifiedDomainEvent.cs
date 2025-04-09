using Ambev.DeveloperEvaluation.Domain.Abstractions;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public record SaleModifiedDomainEvent(Guid SaleId, DateTime ModifiedAt) : IDomainEvent;

