using static Nyctophobia.SelectMenuHooks;

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

    public static void UpdateModule(SelectMenuModule module)
    {
        if (IsPrideDay)
        {
            module.Hue += 0.01f;

            if (module.Hue > 1.0f)
                module.Hue = 0.0f;

            module.Color = (Custom.HSL2RGB(module.Hue, 1.0f, 0.5f));
        }
        else
        {
            if (module.Increasing)
            {
                module.Hue += 0.005f;
                if (module.Hue >= 1.0f)
                {
                    module.Hue = 1.0f;
                    module.Increasing = false;
                }
            }
            else
            {
                module.Hue -= 0.005f;
                if (module.Hue <= 0.0f)
                {
                    module.Hue = 0.0f;
                    module.Increasing = true;
                }
            }

            module.Color = Color.Lerp(new Color(0.592f, 0.22f, 0.22f), Color.red, module.Hue);
        }
    }

    public static bool IsNyctoCat(SlugcatSelectMenu.SlugcatPageContinue self)
    {
        return self.slugcatNumber == NTEnums.NightWalker || self.slugcatNumber == NTEnums.Witness || self.slugcatNumber == NTEnums.Exile;
    }

    public static bool IsNyctoCat(SlugcatSelectMenu self)
    {
        return self.slugcatPages[self.slugcatPageIndex].slugcatNumber == NTEnums.NightWalker || self.slugcatPages[self.slugcatPageIndex].slugcatNumber == NTEnums.Witness || self.slugcatPages[self.slugcatPageIndex].slugcatNumber == NTEnums.Exile;
    }
}