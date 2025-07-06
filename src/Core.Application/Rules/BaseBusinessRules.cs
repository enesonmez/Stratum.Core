using Core.CrossCuttingConcerns.Exception.Types;
using Core.Localization.Abstraction;

namespace Core.Application.Rules;

public abstract class BaseBusinessRules
{
    private readonly ILocalizationService _localizationService;

    protected BaseBusinessRules(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }
    
    protected async Task ThrowBusinessException(string messageKey, string sectionName)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, sectionName);
        throw new BusinessException(message);
    }
}