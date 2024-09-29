using MediatR;

namespace DfE.DomainDrivenDesignTemplate.Application.Services.BackgroundServices.Events
{
    public class CreateReportExampleTaskCompletedEvent(string taskName, string message) : INotification
    {
        public string TaskName { get; } = taskName;
        public string Message { get; } = message;
    }
}
