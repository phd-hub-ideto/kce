using Microsoft.AspNetCore.Mvc.Abstractions;

namespace KhumaloCraft.Application.Mvc;

public class ActionInfo
{
    public ActionInfo(ActionData actionData, ActionDescriptor actionDescriptor, string url)
    {
        ActionData = actionData;
        ActionDescriptor = actionDescriptor;
        Url = url;
    }

    public string Url { get; }

    public ActionDescriptor ActionDescriptor { get; }

    public ActionData ActionData { get; }
}