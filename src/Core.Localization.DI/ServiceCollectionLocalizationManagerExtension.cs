using System.Reflection;
using Core.Localization.Abstraction;
using Core.Localization.DB.Services;
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
    public static IServiceCollection AddFileLocalization(this IServiceCollection services)
    {
        services.AddSingleton<ILocalizationService, FileLocalizationManager>(_ =>
        {
            // <locale, <featureName, resourceDir>>
            Dictionary<string, Dictionary<string, string>> resources = [];
            
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            
            var yamlFiles = Directory.GetFiles(baseDir, "*.yaml", 
                    SearchOption.AllDirectories).Where(f => f.Contains("Locales")); // Sadece Locales klasöründekiler
            
            foreach (string filePath in yamlFiles)
            {
                var fileInfo = new FileInfo(filePath);
                var parts = fileInfo.Name.Split('.');
            
                if (parts.Length < 3) continue;

                string featureName = parts[0]; // users
                string localeCulture = parts[1]; // tr

                if (!resources.ContainsKey(localeCulture))
                {
                    resources.Add(localeCulture, []);
                }
                resources[localeCulture].Add(featureName, filePath);
            }

            return new FileLocalizationManager(resources);
        });

        return services;
    }

    public static IServiceCollection AddDbLocalization(this IServiceCollection services)
    {
        services.AddScoped<ILocalizationService, DbLocalizationManager>();
        
        return services;
    }
}