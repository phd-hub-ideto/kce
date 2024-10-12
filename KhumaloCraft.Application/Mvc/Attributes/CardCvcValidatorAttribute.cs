using System.ComponentModel.DataAnnotations;
using KhumaloCraft.Helpers;

namespace KhumaloCraft.Application.Mvc.Attributes
{
    /// <summary>
    /// A validation attribute for card verification/security codes for all cards except American Express.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
    public class CardCvcValidatorAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string cvc = value as string;
            return string.IsNullOrEmpty(cvc) ? true : CardValidator.IsValidCVC(cvc, false);
        }

    }
}