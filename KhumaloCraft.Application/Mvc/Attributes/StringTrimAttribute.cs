namespace KhumaloCraft.Application.Attributes;

// http://stackoverflow.com/a/20355012/70345
/// <summary>
/// Denotes a data field that should be trimmed during MVC model binding via the <see cref="Application.ModelBinding.StringTrimModelBinder"/>.
/// </summary>
/// <remarks>
/// <para>
/// Support for trimming is implmented in the model binder, as currently Data Annotations provides no mechanism to coerce the value.
/// </para>
/// <para>
/// This attribute does not imply that empty strings should be converted to null.
/// When that is required you must additionally use the <see cref="System.ComponentModel.DataAnnotations.DisplayFormatAttribute.ConvertEmptyStringToNull"/>
/// option to control what happens to empty strings.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public class StringTrimAttribute : Attribute
{
}