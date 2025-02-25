namespace Nyctophobia;

public class LogUtilsLoggerAdapter : ICustomLogger
{
    private readonly LogUtils.Logger _logger;

    public LogUtilsLoggerAdapter(LogUtils.Logger logger)
    {
        _logger = logger;
    }

    public void LogInfo(object log) => _logger.LogInfo(log);
    public void LogWarning(object log) => _logger.LogWarning(log);
    public void LogError(object log) => _logger.LogError(log);
    public void LogFatal(object log) => _logger.LogFatal(log);
}