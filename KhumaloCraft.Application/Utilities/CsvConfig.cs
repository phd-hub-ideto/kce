using CsvHelper.Configuration;
using System.Globalization;

namespace KhumaloCraft.Application.Utilities;

public static class CsvConfig
{
    public static readonly CsvConfiguration DefaultConfiguration = new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = "," };
}