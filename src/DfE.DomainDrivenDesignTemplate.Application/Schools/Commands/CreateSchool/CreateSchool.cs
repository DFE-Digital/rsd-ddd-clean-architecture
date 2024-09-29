using DfE.DomainDrivenDesignTemplate.Application.Common.Interfaces;
using DfE.DomainDrivenDesignTemplate.Application.Schools.Models;
using DfE.DomainDrivenDesignTemplate.Application.Services.BackgroundServices.Events;
using DfE.DomainDrivenDesignTemplate.Application.Services.BackgroundServices.Tasks;
using DfE.DomainDrivenDesignTemplate.Domain.Entities.Schools;
using DfE.DomainDrivenDesignTemplate.Domain.Interfaces.Repositories;
using DfE.DomainDrivenDesignTemplate.Domain.ValueObjects;
using MediatR;

namespace DfE.DomainDrivenDesignTemplate.Application.Schools.Commands.CreateSchool
{
    public record CreateSchoolCommand(
        string SchoolName,
        DateTime LastRefresh,
        DateOnly? EndDate,
        NameDetailsModel NameDetails,
        PrincipalDetailsModel PrincipalDetails
    ) : IRequest<SchoolId>;

    public class CreateSchoolCommandHandler(ISclRepository<School> schoolRepository, IBackgroundServiceFactory backgroundServiceFactory)
        : IRequestHandler<CreateSchoolCommand, SchoolId>
    {
        public async Task<SchoolId> Handle(CreateSchoolCommand request, CancellationToken cancellationToken)
        {
            var school = School.Create(
                request.SchoolName,
                new NameDetails(request.NameDetails.FirstName, request.NameDetails.FirstName, request.NameDetails.FirstName),
                request.LastRefresh,
                request.EndDate,
                request.PrincipalDetails.Email,
                request.PrincipalDetails.Phone,
                request.PrincipalDetails.TypeId
            );

            await schoolRepository.AddAsync(school, cancellationToken);

            // Example of enqueuing a background task
            var taskName = "Task1";

            backgroundServiceFactory.EnqueueTask(
                async () => await (new CreateReportExampleTask()).RunAsync(taskName),
                result => new CreateReportExampleTaskCompletedEvent(taskName, result)
                );

            return school.Id!;
        }
    }
}
