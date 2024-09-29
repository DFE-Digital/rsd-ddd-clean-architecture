using DfE.DomainDrivenDesignTemplate.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DfE.DomainDrivenDesignTemplate.Application.Common.EventHandlers
{
#pragma warning disable S2629, S2139
    public abstract class BaseEventHandler<TEvent>(ILogger<BaseEventHandler<TEvent>> logger)
        : INotificationHandler<TEvent>
        where TEvent : IDomainEvent
    {
        public virtual async Task Handle(TEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation($"Handling event: {typeof(TEvent).Name}");


                await HandleEvent(notification, cancellationToken);

                logger.LogInformation($"Event handled successfully: {typeof(TEvent).Name}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error handling event: {typeof(TEvent).Name}");
                throw;
            }
        }

        protected virtual Task HandleEvent(TEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
#pragma warning restore S2629, S2139
}
