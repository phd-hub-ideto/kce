using FluentValidation;
using FluentValidation.Results;
using KhumaloCraft.Domain;
using KhumaloCraft.Domain.Validation;

namespace KhumaloCraft.Data.Sql;

public abstract class ValidatingRepositoryBase<T> : IValidatingRepository<T>
{
    private readonly IEnumerable<IValidator<T>> _validators;

    protected ValidatingRepositoryBase(IEnumerable<IValidator<T>> validators)
    {
        _validators = validators;
    }

    public bool TryUpsert(T domainEntity, out ValidationResult validationResult)
    {
        var failures = new List<ValidationFailure>();

        if (domainEntity is ValidatedDomainObject validatedDomainObject && !validatedDomainObject.TryValidate(out var errors))
        {
            failures.AddRange(errors.Select(error => new ValidationFailure(null, error)));
        }

        failures.AddRange(_validators.SelectMany(v => v.Validate(domainEntity).Errors));

        validationResult = new ValidationResult(failures);

        if (validationResult.IsValid)
        {
            DoUpsert(domainEntity);
        }

        return validationResult.IsValid;
    }

    protected abstract void DoUpsert(T domainEntity);
}