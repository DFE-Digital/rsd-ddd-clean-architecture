using DfE.DomainDrivenDesignTemplate.Application.Services.BackgroundServices.Events;
using MediatR;

namespace DfE.DomainDrivenDesignTemplate.Application.Services.BackgroundServices.EventHandlers
{
    public class SimpleTaskCompletedEventHandler : INotificationHandler<CreateReportExampleTaskCompletedEvent>
    {
        public Task Handle(CreateReportExampleTaskCompletedEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Event received for Task: {notification.TaskName}, Message: {notification.Message}");
            return Task.CompletedTask;
        }
    }
}
