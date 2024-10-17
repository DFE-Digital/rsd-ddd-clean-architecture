using DfE.DomainDrivenDesignTemplate.Application.Schools.Queries.GetSchools;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace DfE.DomainDrivenDesignTemplate.Api.Controllers
{
    [Route("odata/[controller]")]
    public class SchoolsODataController(ISender sender) : ODataController
    {
        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] int top,
            [FromQuery] int skip,
            [FromQuery] string filter,
            [FromQuery] string select,
            [FromQuery] string expand,
            [FromQuery] string orderby)
        {
            var result = await sender.Send(new GetSchoolsQuery());
            return Ok(result);
        }
    }
}
