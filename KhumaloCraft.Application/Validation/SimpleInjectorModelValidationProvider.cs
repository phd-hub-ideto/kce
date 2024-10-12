using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SimpleInjector;
using System.Collections.Concurrent;

namespace KhumaloCraft.Application.Validation;

internal class SimpleInjectorModelValidatorProvider : IModelValidatorProvider
{
    private readonly Container _container;
    private readonly ConcurrentDictionary<Type, IModelValidator> _typeValidatorCache = new();

    public SimpleInjectorModelValidatorProvider(Container container)
    {
        _container = container;
    }

    public void CreateValidators(ModelValidatorProviderContext ctx)
    {
        var modelType = ctx.ModelMetadata.ModelType;

        if (!_typeValidatorCache.TryGetValue(modelType, out IModelValidator validator))
        {
            if (!TryCreateValidatorForType(modelType, out validator))
            {
                foreach (var implementedInterface in modelType.GetInterfaces())
                {
                    if (TryCreateValidatorForType(implementedInterface, out validator))
                    {
                        break;
                    }
                }
            }

            _typeValidatorCache.TryAdd(modelType, validator);
        }

        if (validator is not null)
        {
            ctx.Results.Add(new ValidatorItem { Validator = validator });
        }
    }

    private bool TryCreateValidatorForType(Type modelType, out IModelValidator validator)
    {
        var validatorType = typeof(ModelValidator<>).MakeGenericType(modelType);

        var atValidator = (IATModelValidator)_container.GetInstance(validatorType);

        if (atValidator.IsApplicableValidator)
        {
            validator = atValidator;
        }
        else
        {
            validator = null;
        }
        return validator != null;
    }
}

/// <inheritdoc/>
internal interface IATModelValidator : IModelValidator
{
    bool IsApplicableValidator { get; }
}

internal class ModelValidator<TModel> : IATModelValidator
{
    private readonly List<IValidator<TModel>> _validators;

    public ModelValidator(IEnumerable<IValidator<TModel>> validators)
    {
        _validators = validators.ToList();
    }

    public bool IsApplicableValidator => _validators.Count > 0;

    public IEnumerable<ModelValidationResult> Validate(ModelValidationContext ctx) => Validate((TModel)ctx.Model);

    private IEnumerable<ModelValidationResult> Validate(TModel model)
    {
        var errors = from validator in _validators
                     from error in validator.Validate(model)
                     select error;

        foreach (var error in errors)
        {
            if (error.MemberNames.Any())
            {
                foreach (var membername in error.MemberNames)
                {
                    yield return new ModelValidationResult(membername, error.ErrorMessage);
                }
            }
            else
            {
                yield return new ModelValidationResult(null, error.ErrorMessage);
            }
        }
    }
}