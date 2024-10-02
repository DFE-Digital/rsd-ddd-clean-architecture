using DfE.CoreLibs.BackgroundService.Interfaces;
using DfE.DomainDrivenDesignTemplate.Application.Services.BackgroundServices.Events;

namespace DfE.DomainDrivenDesignTemplate.Application.Services.BackgroundServices.EventHandlers
{
    public class SimpleTaskCompletedEventHandler : IBackgroundServiceEventHandler<CreateReportExampleTaskCompletedEvent>
    {
        public Task Handle(CreateReportExampleTaskCompletedEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Event received for Task: {notification.TaskName}, Message: {notification.Message}");
            return Task.CompletedTask;
        }
    }
}
