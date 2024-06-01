namespace Nyctophobia;

public static class NTUtils
{
    public static void UnregisterEnums(System.Type type)
    {
        IEnumerable<FieldInfo> extEnums = type.GetFields(BindingFlags.Static | BindingFlags.Public).Where(x => x.FieldType.IsSubclassOf(typeof(ExtEnumBase)));

        foreach (FieldInfo extEnum in extEnums)
        {
            object obj = extEnum.GetValue(null);
            if (obj != null)
            {
                _ = obj.GetType().GetMethod("Unregister")!.Invoke(obj, null);
                extEnum.SetValue(null, null);
            }
        }
    }

    public static void RevivePlayer(Player player)
    {
        player.dead = false;
        player.stun = 0;
        player.animation = AnimationIndex.None;

        PlayerState state = player.playerState;
        state.permanentDamageTracking = 0;
        state.alive = true;
        state.permaDead = false;

        if (player.room?.game?.cameras?.FirstOrDefault()?.hud?.textPrompt is { } prompt)
        {
            prompt.gameOverMode = false;
        }
    }

    public static void MapTextureColor(Texture2D texture, int alpha, Color32 to, bool apply = true)
    {
        Color32[] colors = texture.GetPixels32();

        for (int i = 0; i < colors.Length; i++)
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

    public static void KillCreaturesInRoom(Room room)
    {
        for (int i = 0; i < room.physicalObjects.Length; i++)
        {
            for (int num = room.physicalObjects[i].Count - 1; num >= 0; num--)
            {
                PhysicalObject physicalObject = room.physicalObjects[i][num];
                if (physicalObject is Creature and not Player)
                {
                    (physicalObject as Creature).Die();
                    (physicalObject as Creature).slatedForDeletetion = true;
                }
            }
        }
    }

    public static BodyChunk HeadChunk(this Creature creature) => creature switch
    {
        Hazer or VultureGrub => creature.bodyChunks[1],
        Scavenger => creature.bodyChunks[2],
        MirosBird or Vulture or BigJellyFish => creature.bodyChunks[4],
        _ => creature.mainBodyChunk
    };

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

    public static void CenterSprite(FSprite sprite, ButtonTemplate button)
    {
        sprite.SetAnchor(0.5f, 0.5f);
        sprite.SetPosition(new Vector2(button.pos.x + 2 + (button.size.x - 4) / 2, button.pos.y + 2 + (button.size.y - 4) / 2));
        ScaleSprite(sprite, button.size - new Vector2(4, 4));
    }

    public static void ScaleSprite(FSprite sprite, Vector2 size) => ScaleSprite(sprite, size.x, size.y);

    public static void ScaleSprite(FSprite sprite, float x, float y) => sprite.scale = sprite.element.sourceSize.x > sprite.element.sourceSize.y ? x / sprite.element.sourceSize.x : y / sprite.element.sourceSize.y;

    public static AssetBundle LoadFromEmbeddedResource(string fullyQualifiedPath)
    {
        return AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(fullyQualifiedPath));
    }
}