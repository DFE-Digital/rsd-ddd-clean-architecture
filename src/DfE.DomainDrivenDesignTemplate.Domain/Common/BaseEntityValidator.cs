using FluentValidation;

namespace DfE.DomainDrivenDesignTemplate.Domain.Common
{
    public abstract class BaseEntityValidator<T> : AbstractValidator<T> where T : class
    {
        protected BaseEntityValidator()
        {
            foreach (var validator in GetValidationRules())
            {
                Include(validator);
            }
        }

        protected abstract IEnumerable<IValidator<T>> GetValidationRules();

        public void ValidateAndThrow(T instance)
        {
            var validationResult = Validate(instance);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
        }
    }
}
