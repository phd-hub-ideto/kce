using KhumaloCraft.Helpers;
using System.Text.Json;

namespace KhumaloCraft.Application.Mvc;

public class TitleCaseJsonNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) => FormattingHelper.TitleCase(name);
}