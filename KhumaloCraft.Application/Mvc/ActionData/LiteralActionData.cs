namespace KhumaloCraft.Application.Mvc;

public class LiteralActionData : ActionData
{
    public string Value { get; set; }

    public LiteralActionData(string value)
    {
        Value = value;
    }

    public override ActionData Clone()
    {
        return new LiteralActionData(Value);
    }
}
