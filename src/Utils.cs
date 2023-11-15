using System.Reflection;

namespace Witness;

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

    public static Texture2D MergeElements(string elementAName, string elementBName, Color? colorA = null, Color? colorB = null, bool apply = true)
    {
        var elementA = Futile.atlasManager.GetElementWithName(elementAName);
        var elementB = Futile.atlasManager.GetElementWithName(elementBName);

        var textureA = (Texture2D)elementA.atlas.texture;
        var textureB = (Texture2D)elementB.atlas.texture;

        var rectA = new Rect(elementA.uvRect.x * elementA.atlas.textureSize.x, elementA.atlas.textureSize.y - elementA.sourceSize.y - elementA.uvRect.y * elementA.atlas.textureSize.y, elementA.sourceRect.width, elementA.sourceRect.height);
        var rectB = new Rect(elementB.uvRect.x * elementB.atlas.textureSize.x, elementB.atlas.textureSize.y - elementB.sourceSize.y - elementB.uvRect.y * elementB.atlas.textureSize.y, elementB.sourceRect.width, elementB.sourceRect.height);

        colorA ??= Color.white;
        colorB ??= Color.white;

        var result = new Texture2D((int)rectA.width, (int)rectA.height, TextureFormat.ARGB32, false);

        for (var x = 0; x < rectA.width; x++)
        {
            for (var y = 0; y < rectA.height; y++)
            {
                result.SetPixel(x, y, colorA.Value * textureA.GetPixel((int)(rectA.x + x), (int)(textureA.height - rectA.y - rectA.height + y)));
            }
        }

        for (var x = 0; x < rectB.width; x++)
        {
            for (var y = 0; y < rectB.height; y++)
            {
                var color = textureB.GetPixel((int)(rectB.x + x), (int)(textureB.height - rectB.y - rectB.height + y));
                if (color.a > 0)
                {
                    result.SetPixel(x, y, colorB.Value * color);
                }
            }
        }

        if (apply)
        {
            result.Apply();
        }

        return result;
    }
}
