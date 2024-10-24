using DfE.DomainDrivenDesignTemplate.Domain.Entities.Schools;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace DfE.DomainDrivenDesignTemplate.Infrastructure.OData
{
    public static class EdmModelBuilder
    {
        public static IEdmModel GetEdmModel()
        {
            var oDataBuilder = new ODataConventionModelBuilder();

            oDataBuilder.EntitySet<School>("Schools").EntityType.HasKey(x => x.PrimitiveId).Ignore(x => x.Id);

            return oDataBuilder.GetEdmModel();
        }
    }
}
