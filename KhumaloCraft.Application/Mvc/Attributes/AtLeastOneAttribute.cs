using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace KhumaloCraft.Application.Mvc.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class AtLeastOneAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        return value is IList list && list.Count > 0;
    }
}
