using System.Data;
using Core.Localization.Abstraction;
using Core.Localization.DB.Entities;
using Core.Localization.DB.Repositories.Resources;
using Microsoft.EntityFrameworkCore;

namespace Core.Localization.DB.Services;

public class DbLocalizationManager : ILocalizationService
{
    private readonly IResourceReadRepository _resourceReadRepository;

    public DbLocalizationManager(IResourceReadRepository resourceReadRepository)
    {
        _resourceReadRepository = resourceReadRepository;
    }

    public ICollection<string>? AcceptLocales { get; set; }
    public Task<string> GetLocalizedAsync(string key, string? keySection = null)
    {
        return GetLocalizedAsync(key, AcceptLocales ?? throw new NoNullAllowedException(nameof(AcceptLocales)), keySection);
    }

    public async Task<string> GetLocalizedAsync(string key, ICollection<string> acceptLocales, string? keySection = null)
    {
       Resource? resource = await _resourceReadRepository.GetResourceWithTranslationAsync(x => x.Key == key, enableTracking: false);

       if (resource != null)
       {
           foreach (var locale in acceptLocales)
           {
               ResourceTranslation? translation = resource.ResourceTranslations.FirstOrDefault(x => x.CultureCode == locale);
               if (translation != null)
                   return translation.Value;
           }
       }

       return key;
    }
}