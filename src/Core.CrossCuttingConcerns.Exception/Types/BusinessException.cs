namespace Core.CrossCuttingConcerns.Exception.Types;

public class BusinessException : System.Exception
{
    public string? KeySection { get; }
    
    public BusinessException()
    {
            
    }

    public BusinessException(string message, string? keySection = null) : base(message)
    {
        KeySection = keySection;
    }

    public BusinessException(string message, System.Exception innerException, string? keySection = null) 
        : base(message, innerException)
    {
        KeySection = keySection;
    }
}