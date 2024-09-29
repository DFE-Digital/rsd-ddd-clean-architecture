using System.Diagnostics.CodeAnalysis;
using FluentValidation.Results;

namespace DfE.DomainDrivenDesignTemplate.Application.Common.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class ValidationException() : Exception("One or more validation failures have occurred.")
    {
        public ValidationException(IEnumerable<ValidationFailure> failures)
            : this()
        {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }

        public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>();
    }

}
