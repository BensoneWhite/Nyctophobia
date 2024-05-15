namespace Nyctophobia;

public static class MethodHelpers
{
    public static void InPlaceTryCatch<T>(ref T variableToSet, T defaultValue, string errorMessage, [CallerLineNumber] int lineNumber = 0)
    {
        try
        {
            variableToSet = defaultValue;
        }
        catch (Exception ex)
        {
            Plugin.DebugError(errorMessage.Replace("%ln", $"{lineNumber}"));
            Debug.LogException(ex);
        }
    }
}