using AutoFixture;
using DfE.DomainDrivenDesignTemplate.Client.Contracts;

namespace DfE.DomainDrivenDesignTemplate.Tests.Common.Customizations.Commands
{
    public class CreateSchoolCommandApiClientCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<CreateSchoolCommand>(composer => composer.FromFactory(() =>
            {
                var nameDetails = new NameDetailsModel()
                {
                    FirstName = "John",
                    LastName = "Doe",
                    MiddleName = ""
                };

                var lastRefresh = fixture.Create<DateTime>().Date;
                if (lastRefresh > DateTime.Now.Date)
                {
                    lastRefresh = DateTime.Now.Date.AddDays(-1);
                }

                return new CreateSchoolCommand()
                {
                    SchoolName = fixture.Create<string>(), 
                    LastRefresh = lastRefresh,
                    EndDate = fixture.Create<DateTime>(),
                    NameDetails = nameDetails,
                    PrincipalDetails = fixture.Create<PrincipalDetailsModel>()
                };
            }));
        }
    }
}
