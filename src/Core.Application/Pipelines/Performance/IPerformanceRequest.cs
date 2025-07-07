namespace Core.Application.Pipelines.Performance;

public interface IPerformanceRequest
{
    public int Interval { get; } // ms
}