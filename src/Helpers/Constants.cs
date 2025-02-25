namespace Nyctophobia;

public class Constants
{
    public static bool IsFestive;
    public static bool IsChristmas;
    public static bool IsNewYear;
    public static bool IsPrideDay;
    public static bool IsAnniversary;
    public static bool IsApril;

    // Festive Dates
    public static readonly DateTime christmas = new(DateTime.Now.Year, 12, 25);

    public static readonly DateTime newYear = new(DateTime.Now.Year, 1, 1);
    public static readonly DateTime prideDay = new(DateTime.Now.Year, 6, 1);
    public static readonly DateTime anniversaryDay = new(DateTime.Now.Year, 6, 14);
    public static readonly DateTime AprilDay = new(DateTime.Now.Year, 2, 1);

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

    public const int BODY_SPRITE = 0;
    public const int HIPS_SPRITE = 1;
    public const int TAIL_SPRITE = 2;
    public const int HEAD_SPRITE = 3;
    public const int LEGS_SPRITE = 4;
    public const int ARM_L_SPRITE = 5;
    public const int ARM_R_SPRITE = 6;
    public const int HAND_L_SPRITE = 7;
    public const int HAND_R_SPRITE = 8;
    public const int FACE_SPRITE = 9;
    public const int GLOW_SPRITE = 10;
    public const int MARK_SPRITE = 11;
}