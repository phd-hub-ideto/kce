using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace KhumaloCraft.Application.Validation;

public static class FluentValidationHelper
{
    public static void CopyToModelState(
        this IEnumerable<ValidationFailure> failures,
        ModelStateDictionary modelState,
        IDictionary<string, string> domainToModelPropertyMappings)
    {
        foreach (var failure in failures)
        {
            failure.CopyToModelState(modelState, domainToModelPropertyMappings);
        }
    }

    public static void CopyToModelState(
        this ValidationFailure failure,
        ModelStateDictionary modelState,
        IDictionary<string, string> domainToModelPropertyMappings)
    {
        if (failure.PropertyName != null)
        {
            modelState.AddModelError(domainToModelPropertyMappings[failure.PropertyName], failure.ErrorMessage);
        }
        else
        {
            modelState.AddModelError("", failure.ErrorMessage);
        }
    }
}