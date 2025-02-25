namespace Nyctophobia;

public interface ICustomLogger
{
    void LogInfo(object log);
    void LogWarning(object log);
    void LogError(object log);
    void LogFatal(object log);
}