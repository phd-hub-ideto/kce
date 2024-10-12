using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace KhumaloCraft.Application.Mvc.Extensions;

public class ModelStateError
{
    public ModelStateError(string property, string message)
    {
        this.Property = property;
        this.Message = message;
    }

    public string Property { get; private set; }
    public string Message { get; private set; }
}

public static class ModelStateDictionaryExtensions
{
    public static IEnumerable<ModelStateError> GetErrors(this ModelStateDictionary state)
    {
        var errors = new List<ModelStateError>();
        foreach (var key in state.Keys)
        {
            var modelState = state[key];
            if (modelState.Errors.Count > 0)
            {
                errors.Add(new ModelStateError(key, modelState.Errors.First().ErrorMessage));
            }
        }
        return errors;
    }

    public static void ClearAllErrors(this ModelStateDictionary state)
    {
        foreach (var key in state.Keys)
        {
            var modelState = state[key];
            if (modelState.Errors.Count > 0)
            {
                state[key].Errors.Clear();
            }
        }
    }

    public static bool IsValidField(this ModelStateDictionary state, string prefix)
    {
        foreach (var key in state.Keys)
        {
            if (ModelStateDictionary.StartsWithPrefix(prefix, key))
            {
                if (state[key].Errors.Any()) return false;
            }
        }

        return true;
    }
}