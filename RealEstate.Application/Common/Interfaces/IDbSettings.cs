namespace RealEstate.Application.Common.Interfaces;

public interface IDbSettings
{
    string ConnectionString { get; }
    int CommandTimeout { get; }
    bool EnableRetryOnFailure { get; }
    int MaxRetryCount { get; }
    TimeSpan MaxRetryDelay { get; }
}