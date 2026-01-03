namespace Core.CrossCuttingConcerns.Logging.Abstraction;

public interface ILogger
{
    public void Trace(string message);
    public void Critical(string message);
    public void Information(string message);
    public void Information(string template, params object[] args);
    public void Warning(string message);
    public void Debug(string message);
    public void Error(string message);
    public void Error(string template, params object[] args);
    IDisposable? PushProperty(string key, object? value);
}