using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace KhumaloCraft.Application.Mvc;

public class MaintainCaseJsonResult : JsonResult
{
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = null //Set to null to maintain the existing case.
    };

    public MaintainCaseJsonResult(object value)
        : base(value, _options)
    {

    }
}