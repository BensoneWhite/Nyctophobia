namespace Nyctophobia;

public class Constants
{
    public static bool IsFestive;
    public static bool IsChristmas;
    public static bool IsNewYear;

    // Festive Dates
    public static readonly DateTime christmas = new(DateTime.Now.Year, 12, 25);

    public static readonly DateTime newYear = new(DateTime.Now.Year, 1, 1);

    //This are used for sprite containers
    public static List<string> nodeList =
    [
        "Shadows",            // 0
        "BackgroundShortcuts",// 1
        "Background",         // 2
        "Midground",          // 3
        "Items",              // 4
        "Foreground",         // 5
        "ForegroundLights",   // 6
        "Shortcuts",          // 7
        "Water",              // 8
        "GrabShaders",        // 9
        "Bloom",              // 10
        "HUD",                // 11
        "HUD2"                // 12
    ];
}