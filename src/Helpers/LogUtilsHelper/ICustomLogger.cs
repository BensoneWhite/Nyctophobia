namespace Nyctophobia;

public interface ICustomLogger
{
    public void Log(object ex);
    public void LogDebug(object ex);
    public void LogInfo(object ex);
    public void LogImportant(object ex);
    public void LogMessage(object ex);
    public void LogWarning(object ex);
    public void LogError(object ex);
    public void LogFatal(object ex);
}