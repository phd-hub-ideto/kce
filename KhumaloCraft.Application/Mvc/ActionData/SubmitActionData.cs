namespace KhumaloCraft.Application.Mvc;

public class SubmitActionData : UrlStringActionData
{
    public SubmitActionData(string url)
        : base(url)
    {
    }

    public override ActionData Clone()
    {
        return new SubmitActionData(Url);
    }
}