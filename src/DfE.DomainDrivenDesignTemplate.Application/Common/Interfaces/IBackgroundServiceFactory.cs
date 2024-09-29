using MediatR;

namespace DfE.DomainDrivenDesignTemplate.Application.Common.Interfaces
{
    public interface IBackgroundServiceFactory
    {
        void EnqueueTask<TResult, TNotification>(Func<Task<TResult>> taskFunc, Func<TResult, TNotification>? eventFactory = null)
            where TNotification : INotification;
    }
}
