using DfE.DomainDrivenDesignTemplate.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DfE.DomainDrivenDesignTemplate.Infrastructure.Database.Interceptors
{
    public class DomainEventDispatcherInterceptor(IMediator mediator) : SaveChangesInterceptor
    {
        public override async ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData eventData,
            int result,
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;

            if (context == null) return result;

            var entitiesWithEvents = context.ChangeTracker
                .Entries<IHasDomainEvents>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents != null && e.DomainEvents.Any())
                .ToList();

            var allEvents = entitiesWithEvents
                .SelectMany(e => e.DomainEvents)
                .ToList();

            foreach (var entity in entitiesWithEvents)
            {
                entity.ClearDomainEvents();
            }

            foreach (var domainEvent in allEvents)
            {
                await mediator.Publish(domainEvent, cancellationToken);
            }

            return result;
        }
    }
}
