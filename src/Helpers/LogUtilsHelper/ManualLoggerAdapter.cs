namespace Nyctophobia;

public class ManualLoggerAdapter : ICustomLogger
{
    private readonly ManualLogSource _logger;

    public ManualLoggerAdapter(ManualLogSource logger)
    {
        _logger = logger;
    }

    public void LogInfo(object log) => _logger.LogInfo(log);
    public void LogWarning(object log) => _logger.LogWarning(log);
    public void LogError(object log) => _logger.LogError(log);
    public void LogFatal(object log) => _logger.LogFatal(log);
}