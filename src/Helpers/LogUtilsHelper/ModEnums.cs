using LogUtils.Enums;

namespace Nyctophobia;

public static class ModEnums
{
    public class LogID(string filename, LogAccess access, bool register = false) : LogUtils.Enums.LogID(filename, access, register)
    {
        public static readonly LogID NycLog = new("NycLog", LogAccess.FullAccess, true);
    }

    public class LogCategory(string name, LogLevel? level, LogType? type) : LogUtils.Enums.LogCategory(name, level, type)
    {
        public static readonly LogCategory NycType = new("NycType", null, null);
    }
}