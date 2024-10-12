using System.ComponentModel.DataAnnotations;

namespace KhumaloCraft.Application.Validation;

public interface IValidator<T>
{
    IEnumerable<ValidationResult> Validate(T instance);
}