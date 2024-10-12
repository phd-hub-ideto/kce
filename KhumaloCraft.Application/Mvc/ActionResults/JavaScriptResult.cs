using Microsoft.AspNetCore.Mvc;

namespace KhumaloCraft.Application.Mvc;

public class JavaScriptResult : ContentResult
{
    public JavaScriptResult(string script)
    {
        Content = script;
        ContentType = "application/javascript";
    }
}