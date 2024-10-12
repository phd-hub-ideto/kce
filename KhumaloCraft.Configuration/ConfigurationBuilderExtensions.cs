using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace KhumaloCraft.Configuration.Providers;

public static class ConfigurationBuilderExtensions
{
    public static void AddJsonFilesRecursive(this IConfigurationBuilder configurationBuilder)
    {
        var fileProvider = (PhysicalFileProvider)FileConfigurationExtensions.GetFileProvider(configurationBuilder);

        string searchPath = fileProvider.Root;

        configurationBuilder.AddJsonFilesRecursive(searchPath, "appsettings.json");
    }

    public static IConfigurationBuilder AddJsonFilesRecursive(this IConfigurationBuilder configurationBuilder, string path, string jsonFileName)
    {
        var appSettingsFiles = EnumerateJsonFiles(path, jsonFileName).Reverse().ToList();

        if (!appSettingsFiles.Any())
        {
            throw new InvalidOperationException("No appsettings files found.");
        }

        foreach (var appSettingFile in appSettingsFiles)
        {
            configurationBuilder.AddJsonFile(appSettingFile, optional: false, reloadOnChange: true);
        }

        return configurationBuilder;
    }

    private static IEnumerable<string> EnumerateJsonFiles(string path, string fileName)
    {
        var current = new DirectoryInfo(path);

        do
        {
            string fileFullPath = Path.Combine(current.FullName, fileName);

            if (File.Exists(fileFullPath))
            {
                yield return fileFullPath;
            }

            current = current.Parent;
        }
        while (current != null);
    }
}
