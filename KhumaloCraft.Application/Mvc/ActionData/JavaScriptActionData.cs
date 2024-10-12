namespace KhumaloCraft.Application.Mvc;

public class JavaScriptActionData : LiteralActionData
{
    public JavaScriptActionData(string value)
        : base(value)
    {
    }

    public override ActionData Clone()
    {
        return new JavaScriptActionData(Value);
    }

    public override string ToString()
    {
        return "javascript:" + Value;
    }
}