namespace KhumaloCraft.Application.Mvc;

public class UrlStringActionData : ActionData
{
    public string Url { get; }

    public UrlStringActionData(string url)
    {
        Url = url;
    }

    public override ActionData Clone()
    {
        return new UrlStringActionData(Url);
    }

    public override string ToString()
    {
        return Url;
    }
}