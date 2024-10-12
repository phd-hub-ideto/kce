using FluentValidation.Results;

namespace KhumaloCraft.Domain.Validation;

public interface IValidatingRepository<T>
{
    bool TryUpsert(T domainEntity, out ValidationResult validationResult);
}