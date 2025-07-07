namespace Core.CrossCuttingConcerns.Logging;

public class LogDetail
{
    public string MethodName { get; set; }
    public string User { get; set; }
    public List<LogParameter> Parameters { get; set; }

    public LogDetail()
    {
        MethodName = string.Empty;
        User = string.Empty;
        Parameters = [];
    }

    public LogDetail(string fullName, string methodName, string user, List<LogParameter> parameters)
    {
        MethodName = methodName;
        User = user;
        Parameters = parameters;
    }
}