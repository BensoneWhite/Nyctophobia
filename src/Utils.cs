namespace Nyctophobia;

public static class NTUtils
{
    public static void UnregisterEnums(Type type)
    {
        var extEnums = type.GetFields(BindingFlags.Static | BindingFlags.Public).Where(x => x.FieldType.IsSubclassOf(typeof(ExtEnumBase)));

        foreach (var extEnum in extEnums)
        {
            var obj = extEnum.GetValue(null);
            if (obj != null)
            {
                obj.GetType().GetMethod("Unregister")!.Invoke(obj, null);
                extEnum.SetValue(null, null);
            }
        }
    }

    public static void RevivePlayer(Player player)
    {
        player.dead = false;
        player.stun = 0;
        player.animation = Player.AnimationIndex.None;

        var state = player.playerState;
        state.permanentDamageTracking = 0;
        state.alive = true;
        state.permaDead = false;

        if (player.room?.game?.cameras?.FirstOrDefault()?.hud?.textPrompt is { } prompt)
        {
            prompt.gameOverMode = false;
        }
    }
    public static bool PlayerHasCustomTail(PlayerGraphics pg)
    {
        if (!ModManager.ActiveMods.Any(x => x.id == "dressmyslugcat"))
        {
            return true;
        }

        return PlayerHasCustomTailDMS(pg);
    }

    public static bool PlayerHasCustomTailDMS(PlayerGraphics pg)
    {
        return !(DressMySlugcat.Customization.For(pg)?.CustomTail?.EffectiveCustTailShape ?? false);
    }

    public static void MapTextureColor(Texture2D texture, int alpha, Color32 to, bool apply = true)
    {
        var colors = texture.GetPixels32();

        for (var i = 0; i < colors.Length; i++)
        {
            if (colors[i].a == alpha)
            {
                colors[i] = to;
            }
        }

        texture.SetPixels32(colors);

        if (apply)
        {
            texture.Apply(false);
        }
    }
}
