using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace KhumaloCraft.Application.Mvc.Attributes;

public class FileUploadAllowedMimeTypesAttribute : ValidationAttribute
{
    private readonly IEnumerable<string> _allowedMimeTypes;
    private readonly string _message;

    /// <summary>
    /// Files with extensions that are excluded from the .exe MimeType validation.
    /// </summary>
    private static readonly string[] _exeMimeTypeExtensionExceptions = new[] { ".msg" };

    public FileUploadAllowedMimeTypesAttribute(params string[] allowedMimeTypes)
    {
        _allowedMimeTypes = allowedMimeTypes;
        _message = "Invalid file type";
    }

    public FileUploadAllowedMimeTypesAttribute(string errorMessage, params string[] allowedMimeTypes)
    {
        _allowedMimeTypes = allowedMimeTypes;
        _message = errorMessage;
    }

    public override bool IsValid(object value)
    {
        var file = value as FormFile;
        if (value != null)  //empty values should be caught using [required] 
        {
            if (Constants.MimeTypes.Exe.Types.Any(t => t == file.ContentType) &&
                !_exeMimeTypeExtensionExceptions.Any(ee => ee == Path.GetExtension(file.FileName)))
                return false;

            if (!_allowedMimeTypes.IsNullOrEmpty() && !_allowedMimeTypes.Any(amt => amt == file.ContentType))
                return false;
        }

        return true;
    }

    public override string FormatErrorMessage(string name)
    {
        return _message;
    }
}