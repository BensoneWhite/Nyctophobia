﻿namespace Nyctophobia;

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
}