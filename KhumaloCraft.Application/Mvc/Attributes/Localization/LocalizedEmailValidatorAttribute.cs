namespace KhumaloCraft.Application.Mvc.Attributes
{
    public class LocalizedEmailValidatorAttribute : EmailValidatorAttribute
    {
        /// <summary>
        /// Constructs an instance of LocalizedEmailValidatorAttribute with a localized message format.
        /// </summary>
        /// <param name="allowFullAddressSpecification">
        /// Whether to allow full email address specification, which includes a display name and email address in
        /// angle brackets, e.g. Display-Name &lt;name@domain.com&gt;. Default is False.
        /// </param>
        public LocalizedEmailValidatorAttribute(bool allowFullAddressSpecification = false)
            : base(EmailValidatorMessageFormat.Localized, allowFullAddressSpecification)
        {

        }
    }
}
