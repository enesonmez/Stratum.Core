using System.Reflection;
using Core.Localization.Abstraction;
using Core.Localization.File;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Localization.DI;

public static class ServiceCollectionLocalizationManagerExtension
{
    /// <summary>
    /// Adds <see cref="ResourceLocalizationManager"/> as <see cref="ILocalizationService"/> to <see cref="IServiceCollection"/>.
    /// <list type="bullet">
    ///    <item>
    ///        <description>Reads all yaml files in the "<see cref="Assembly.GetExecutingAssembly()"/>/Features/{featureName}/Resources/Locales/". Yaml file names must be like {uniqueKeySectionName}.{culture}.yaml.</description>
    ///    </item>
    ///    <item>
    ///        <description>If you don't want separate locale files with sections, create "<see cref="Assembly.GetExecutingAssembly()"/>/Features/Index/Resources/Locales/index.{culture}.yaml".</description>
    ///    </item>
    /// </list>
    /// </summary>
    public static IServiceCollection AddFileLocalization(this IServiceCollection services, Assembly assembly)
    {
        services.AddSingleton<ILocalizationService, FileLocalizationManager>(_ =>
        {
            // <locale, <featureName, resourceDir>>
            Dictionary<string, Dictionary<string, string>> resources = [];
            
            string projectPath = Directory.GetParent(Path.GetDirectoryName(assembly.Location)!)!.Parent!.Parent!.Parent!.ToString();
            string[] featureDirs = Directory.GetDirectories(
                Path.Combine(projectPath, "Application", "Features")
            );
            foreach (string featureDir in featureDirs)
            {
                string featureName = Path.GetFileName(featureDir);
                string localeDir = Path.Combine(featureDir, "Resources", "Locales");
                if (Directory.Exists(localeDir))
                {
                    string[] localeFiles = Directory.GetFiles(localeDir);
                    foreach (string localeFile in localeFiles)
                    {
                        string localeName = Path.GetFileNameWithoutExtension(localeFile);
                        int separatorIndex = localeName.IndexOf('.');
                        string localeCulture = localeName[(separatorIndex + 1)..];

                        if (System.IO.File.Exists(localeFile))
                        {
                            if (!resources.ContainsKey(localeCulture))
                                resources.Add(localeCulture, []);
                            resources[localeCulture].Add(featureName, localeFile);
                        }
                    }
                }
            }

            return new FileLocalizationManager(resources);
        });

        return services;
    }
}