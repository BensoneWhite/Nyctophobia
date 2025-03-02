namespace Nyctophobia;

public class LogUtilsLoggerAdapter : ICustomLogger
{
    private readonly LogUtils.Logger _logger;

    public LogUtilsLoggerAdapter()
    {
        _logger = new LogUtils.Logger(ModEnums.LogID.NycLog)
        {
            ManagedLogSource = Plugin.Logger
        };
    }

    public void Log(object ex) => _logger.Log(ex);
    public void LogDebug(object ex) => _logger.LogDebug(ex);
    public void LogInfo(object ex) => _logger.LogInfo(ex);
    public void LogImportant(object ex) => _logger.LogImportant(ex);
    public void LogMessage(object ex) => _logger.LogMessage(ex);
    public void LogWarning(object ex) => _logger.LogWarning(ex);
    public void LogError(object ex) => _logger.LogError(ex);
    public void LogFatal(object ex) => _logger.LogFatal(ex);
}