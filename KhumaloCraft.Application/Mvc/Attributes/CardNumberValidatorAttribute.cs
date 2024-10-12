using System.ComponentModel.DataAnnotations;
using KhumaloCraft.Helpers;

namespace KhumaloCraft.Application.Mvc.Attributes;

/// <summary>
/// A validation attribute for payment card numbers.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
public class CardNumberValidatorAttribute : DataTypeAttribute
{
    public CardNumberValidatorAttribute()
        : base(DataType.CreditCard)
    {
    }

    public override bool IsValid(object value)
    {
        string cardNo = value as string;

        return string.IsNullOrEmpty(cardNo) || CardValidator.IsValidNumber(cardNo);
    }
}