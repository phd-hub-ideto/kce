namespace KhumaloCraft.Application.Mvc.Attributes;

/// <summary>
/// This interface can be implemented by a view model property class, so that an alternative value can be provided from which RequiredIf validation attributes can evaluate validity.
/// </summary>
public interface IRequiredIfValueProvider
{
    object GetValue();
}