using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace KhumaloCraft.Application.Mvc.Attributes;

/// <summary>
/// Specifies that a parameter or property should be bound using route-data from the current request or from the request query string.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class FromRouteOrQueryAttribute : Attribute, IBindingSourceMetadata, IModelNameProvider, IFromRouteMetadata, IFromQueryMetadata
{
    private static readonly BindingSource _bindingSource = CompositeBindingSource.Create(
        new[] { BindingSource.Path, BindingSource.Query },
        "PathOrQuery"
    );

    public BindingSource BindingSource => _bindingSource;

    public string Name { get; set; }
}