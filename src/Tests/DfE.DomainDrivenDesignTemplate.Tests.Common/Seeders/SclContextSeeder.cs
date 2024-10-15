using DfE.DomainDrivenDesignTemplate.Domain.Entities.Schools;
using DfE.DomainDrivenDesignTemplate.Domain.ValueObjects;
using DfE.DomainDrivenDesignTemplate.Infrastructure.Database;

namespace DfE.DomainDrivenDesignTemplate.Tests.Common.Seeders
{
    public static class SclContextSeeder
    {
        public static void Seed(SclContext context)
        {
                var memberContact1 = new PrincipalDetails(
                    new PrincipalId(1),
                    1,
                    "test1@example.com",
                    null
                );

                var memberContact2 = new PrincipalDetails(
                    new PrincipalId(2),
                    1,
                    "test2@example.com",
                    null
                );

                var school1 = new School(
                    new SchoolId(1),
                    new PrincipalId(1),
                    "Test School 1",
                    new NameDetails(
                        "Wood, John",
                        "John Wood",
                        "Mr. John Wood MP"
                    ),
                    DateTime.UtcNow,
                    null,
                    memberContact1
                );

                var school2 = new School(
                    new SchoolId(2),
                    new PrincipalId(2),
                    "Test School 2",
                    new NameDetails(
                        "Wood, Joe",
                        "Joe Wood",
                        "Mr. Joe Wood MP"
                    ),
                    DateTime.UtcNow,
                    null,
                    memberContact2
                );

                context.Schools.AddRange(school1, school2);
                context.SaveChanges();
        }
    }
}
