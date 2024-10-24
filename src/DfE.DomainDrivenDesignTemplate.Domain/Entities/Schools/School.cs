using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DfE.DomainDrivenDesignTemplate.Domain.Common;
using DfE.DomainDrivenDesignTemplate.Domain.Events;
using DfE.DomainDrivenDesignTemplate.Domain.Validators;
using DfE.DomainDrivenDesignTemplate.Domain.ValueObjects;

namespace DfE.DomainDrivenDesignTemplate.Domain.Entities.Schools
{
#pragma warning disable CS8618
    public sealed class School : BaseAggregateRoot, IEntity<SchoolId>
    {
        public SchoolId Id { get;}
        public int PrimitiveId
        {
            get => Id.Value;
            internal set { }
        }
        public PrincipalId PrincipalId { get; internal set; }
        public string SchoolName { get; internal set; }
        public NameDetails NameDetails { get; internal set; }
        public DateTime LastRefresh { get; internal set; }
        public DateOnly? EndDate { get; internal set; }

        public PrincipalDetails PrincipalDetails { get; internal set; }

        private School() { }

        public School(
            SchoolId schoolId,
            PrincipalId principalId,
            string schoolName,
            NameDetails nameDetails,
            DateTime lastRefresh,
            DateOnly? endDate,
            PrincipalDetails principalDetails)
        {
            Id = schoolId ?? throw new ArgumentNullException(nameof(schoolId));
            PrincipalId = principalId ?? throw new ArgumentNullException(nameof(principalId));
            SchoolName = schoolName;
            NameDetails = nameDetails ?? throw new ArgumentNullException(nameof(nameDetails));
            LastRefresh = lastRefresh;
            EndDate = endDate;
            PrincipalDetails = principalDetails;
        }

        private School(
            string schoolName,
            NameDetails nameDetails,
            DateTime lastRefresh,
            DateOnly? endDate,
            PrincipalDetails principalDetails)
        {
            SchoolName = schoolName ?? throw new ArgumentNullException(nameof(schoolName));
            NameDetails = nameDetails ?? throw new ArgumentNullException(nameof(nameDetails));
            LastRefresh = lastRefresh;
            EndDate = endDate;
            PrincipalDetails = principalDetails;
        }

        public static School Create(
            string schoolName,
            NameDetails nameDetails,
            DateTime lastRefresh,
            DateOnly? endDate,
            string principalEmail,
            string principalPhone,
            int principalTypeId)
        {
            var principalDetails = new PrincipalDetails(principalTypeId, principalEmail, principalPhone);

            var school = new School(schoolName, nameDetails, lastRefresh, endDate, principalDetails);

            var createValidator = new SchoolCreateValidator();

            createValidator.ValidateAndThrow(school);

            school.AddDomainEvent(new SchoolCreatedEvent(school));

            return school;
        }
    }
#pragma warning restore CS8618
}
