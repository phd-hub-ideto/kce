using KhumaloCraft.Dependencies;

namespace KhumaloCraft.Http;

[Singleton]
public class FormContentFactory
{
    public HttpContent Create(IFormContentModel model)
    {
        var content = new FormUrlEncodedContent(model.ToNameValueCollection());
        content.Headers.ContentType = null;
        content.Headers.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
        return content;
    }
}