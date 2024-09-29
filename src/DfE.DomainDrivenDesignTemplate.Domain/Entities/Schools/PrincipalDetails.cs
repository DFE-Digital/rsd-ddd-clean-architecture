using DfE.DomainDrivenDesignTemplate.Domain.Common;
using DfE.DomainDrivenDesignTemplate.Domain.ValueObjects;

namespace DfE.DomainDrivenDesignTemplate.Domain.Entities.Schools
{
#pragma warning disable CS8618

    public sealed class PrincipalDetails : IEntity<PrincipalId>
    {
        public PrincipalId Id { get; private set; }
        public string? Email { get; private set; }
        public string? Phone { get; private set; }
        public int TypeId { get; private set; }

        private PrincipalDetails() { }

        public PrincipalDetails(
            PrincipalId principalId,
            int typeId,
            string? email = null,
            string? phone = null)
        {
            if (typeId <= 0) throw new ArgumentException("TypeId must be positive", nameof(typeId));

            Id = principalId ?? throw new ArgumentNullException(nameof(principalId)); 
            TypeId = typeId;
            Email = email;
            Phone = phone;
        }

        internal PrincipalDetails(
            int typeId,
            string? email = null,
            string? phone = null)
        {
            if (typeId <= 0) throw new ArgumentException("TypeId must be positive", nameof(typeId));

            TypeId = typeId;
            Email = email;
            Phone = phone;
        }
    }
#pragma warning restore CS8618

}
