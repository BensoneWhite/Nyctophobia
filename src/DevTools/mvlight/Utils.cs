namespace Nyctophobia;

public static class Utils
{
    public static bool IsMiraActive => ModManager.ActiveMods.Any(x => x.id == "mira");
    public static bool MiraVersionWarning => IsMiraActive;

    public static RainWorld RainWorld => Custom.rainWorld;
    public static Dictionary<string, FShader> Shaders => RainWorld.Shaders;
    public static InGameTranslator Translator => RainWorld.inGameTranslator;

    public static bool WarpEnabled(this RainWorldGame game) => game.IsStorySession && (!ModManager.MSC || !game.rainWorld.safariMode);

    public static void AddTextPrompt(this RainWorldGame game, string text, int wait, int time, bool darken = false, bool? hideHud = null)
    {
        hideHud ??= ModManager.MMF;
        game.cameras.First().hud.textPrompt.AddMessage(Translator.Translate(text), wait, time, darken, (bool)hideHud);
    }

    public static void LockAndHideShortcuts(this Room room)
    {
        room.LockShortcuts();
        room.HideShortcuts();
    }

    public static void UnlockAndShowShortcuts(this Room room)
    {
        room.UnlockShortcuts();
        room.ShowShortcuts();

        room.game.cameras.First().hud.PlaySound(SoundID.SS_AI_Give_The_Mark_Boom);
    }

    public static void LockShortcuts(this Room room)
    {
        foreach (var shortcut in room.shortcutsIndex)
            if (!room.lockedShortcuts.Contains(shortcut))
                room.lockedShortcuts.Add(shortcut);
    }

    public static void UnlockShortcuts(this Room room)
    {
        room.lockedShortcuts.Clear();
    }

    public static void HideShortcuts(this Room room)
    {
        var rCam = room.game.cameras.First();

        if (rCam.room != room) return;

        var shortcutGraphics = rCam.shortcutGraphics;

        for (int i = 0; i < room.shortcuts.Length; i++)
            if (shortcutGraphics.entranceSprites.Length > i && shortcutGraphics.entranceSprites[i, 0] != null)
                shortcutGraphics.entranceSprites[i, 0].isVisible = false;
    }

    public static void ShowShortcuts(this Room room)
    {
        var rCam = room.game.cameras.First();

        if (rCam.room != room) return;

        var shortcutGraphics = rCam.shortcutGraphics;

        for (int i = 0; i < room.shortcuts.Length; i++)
            if (shortcutGraphics.entranceSprites[i, 0] != null)
                shortcutGraphics.entranceSprites[i, 0].isVisible = true;
    }

    public static Color RWColorSafety(this Color color)
    {
        var hsl = Custom.RGB2HSL(color);

        var safeColor = Custom.HSL2RGB(hsl.x, hsl.y, Mathf.Clamp(hsl.z, 0.01f, 1.0f), color.a);

        return safeColor;
    }

    public static int TexUpdateInterval(this Player player)
    {
        var texUpdateInterval = 5;
        var quality = player.abstractCreature.world.game.rainWorld.options.quality;

        if (quality == Options.Quality.LOW)
        {
            texUpdateInterval = 20;
        }
        else if (quality == Options.Quality.MEDIUM)
        {
            texUpdateInterval = 10;
        }

        return texUpdateInterval;
    }

    public static void SetIfSame(this ref Color toSet, Color toCompare, Color newColor)
    {
        if (toSet == toCompare)
        {
            toSet = newColor;
        }
    }

    public static void MapAlphaToColor(this Texture2D texture, Dictionary<byte, Color> map)
    {
        var data = texture.GetPixelData<Color32>(0);

        for (int i = 0; i < data.Length; i++)
        {
            if (map.TryGetValue(data[i].a, out var targetColor))
            {
                data[i] = targetColor;
            }
        }

        texture.SetPixelData(data, 0);
        texture.Apply(false);
    }

    public static Color HSLToRGB(this Vector3 hsl)
    {
        return Custom.HSL2RGB(hsl.x, hsl.y, hsl.z);
    }
}