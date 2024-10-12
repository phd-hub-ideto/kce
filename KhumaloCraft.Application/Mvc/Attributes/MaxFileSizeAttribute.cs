using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace KhumaloCraft.Application.Mvc.Attributes;

public class MaxFileSizeAttribute : ValidationAttribute
{
    private readonly int _maxFileSize;
    private bool _fileRequired = true;

    public MaxFileSizeAttribute(int maxFileSize, bool isRequired = true)
    {
        _maxFileSize = maxFileSize;
        _fileRequired = isRequired;
    }

    public override bool IsValid(object value)
    {
        var file = value as FormFile;
        if (file == null)
        {
            return !_fileRequired;
        }
        return file.Length <= _maxFileSize;
    }

    public override string FormatErrorMessage(string name)
    {
        return base.FormatErrorMessage(_maxFileSize.ToString());
    }
}
