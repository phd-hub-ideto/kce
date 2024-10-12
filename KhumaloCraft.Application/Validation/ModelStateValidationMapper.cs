using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace KhumaloCraft.Application.Validation;

public class ModelStateValidationMapper
{
    private readonly Dictionary<string, string> _mappings;

    public ModelStateValidationMapper(Dictionary<string, string> mappings)
    {
        _mappings = mappings;
    }

    public void MapValidationResult(ModelStateDictionary modelState, ValidationResult validationResult)
    {
        foreach (var validationError in validationResult.Errors)
        {
            if (!_mappings.TryGetValue(validationError.PropertyName, out var modelPropertyName))
            {
                modelPropertyName = "";
            }

            modelState.AddModelError(modelPropertyName, validationError.ErrorMessage);
        }
    }
}