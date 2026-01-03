using Serilog.Core;
using Serilog.Events;
using System.Text.RegularExpressions;

namespace Core.CrossCuttingConcerns.Logging.SeriLog.Enricher;

public class SensitiveDataDestructuringPolicy : IDestructuringPolicy
{
    private static readonly HashSet<string> SensitiveKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "Password", "Sifre", "Token", "CreditCard", "Cvv", "Tckn"
    };

    private static readonly Regex JsonMaskingRegex = new Regex(
        @"(?i)""(?<key>password|sifre|token|tckn|accessToken|refreshToken)""\s*:\s*""(.*?)""", 
        RegexOptions.Compiled | RegexOptions.ExplicitCapture);

    public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue result)
    {
        if (value is LogParameter parameter)
        {
            if (SensitiveKeys.Contains(parameter.Type))
            {
                result = CreateMaskedLogParameter(parameter, "***MASKED***", propertyValueFactory);
                return true; 
            }
            if (parameter.Value is string stringValue)
            {
                var readableJson = Regex.Unescape(stringValue);
                if (IsJson(stringValue))
                {
                    var maskedValue = JsonMaskingRegex.Replace(readableJson, 
                        m => $"\"{m.Groups["key"].Value}\": \"***MASKED***\"");
                    
                    if (maskedValue != readableJson)
                    {
                        result = CreateMaskedLogParameter(parameter, maskedValue, propertyValueFactory);
                        return true;
                    }
                }
            }
        }

        result = null!;
        return false;
    }

    private LogEventPropertyValue CreateMaskedLogParameter(LogParameter original, object newValue, ILogEventPropertyValueFactory factory)
    {
        return factory.CreatePropertyValue(new 
        {
            original.Type,   
            Value = newValue
        }, destructureObjects: true);
    }

    private static bool IsJson(string input) 
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        input = input.Trim();
        return (input.StartsWith("{") && input.EndsWith("}")) || 
               (input.StartsWith("[") && input.EndsWith("]"));
    }
}