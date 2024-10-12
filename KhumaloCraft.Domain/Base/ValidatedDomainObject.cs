using KhumaloCraft.Exceptions;

namespace KhumaloCraft.Domain;

[Serializable]
public abstract class ValidatedDomainObject
{
    /// <summary>
    /// Implemented in derived classes to perform validation.
    /// </summary>
    /// <param name="fail">
    /// Validation failures should be recorded by calling this delegate.
    /// This delegate does not throw an exception, allowing validation to continue,
    /// collecting multiple validation errors.
    /// </param>
    protected abstract void Validate(Action<string> fail);

    /// <summary>
    /// Performs validation and returns a boolean flag indicating whether validation succeeded or not.
    /// </summary>
    /// <param name="errors">
    ///	When True is returned, null.
    ///	When False is returned, an array of one or more validation errors.
    /// </param>
    /// <returns>
    /// True if no validation failures were recorded.
    /// False if one or more validation failures were recorded.
    /// </returns>
    public bool TryValidate(out string[] errors)
    {
        List<string> errorList = null;

        Action<string> fail = s =>
        {
            if (null == errorList)
                errorList = new List<string>();

            var message = s ?? "Unspecified validation error.";

            errorList.Add(message);
        };

        Validate(fail);

        if (errorList.IsNullOrEmpty())
        {
            errors = null;

            return true;
        }

        errors = errorList.ToArray();

        return false;
    }

    /// <summary>
    /// Performs validation and returns a boolean flag indicating whether validation succeeded or not.
    /// </summary>
    /// <returns>
    /// True if no validation failures were recorded.
    /// False if one or more validation failures were recorded.
    /// </returns>
    public bool TryValidate()
    {
        var success = true;
        Validate(s => success = false);

        return success;
    }

    /// <summary>
    /// Performs validation and throws a ValidationException if one or more validation failures were recorded.
    /// </summary>
    public void Validate()
    {
        if (!TryValidate(out string[] errors))
            throw new ValidationException(GetType(), errors);
    }
}
