using KhumaloCraft.Application.Mvc.WebViewPages;
using Microsoft.AspNetCore.Http;

namespace KhumaloCraft.Application.Mvc;

public abstract class CustomWebViewPage<TModel> : RazorViewPage<TModel>
{
    public bool IsPost => Context.Request.Method == HttpMethods.Post;

    [Obsolete("Razor cannot correctly render a Task. Use the await keyword to render the result.", error: true)]
    public void Write<T>(Task<T> task)
    {
        throw new NotImplementedException();
    }
}
