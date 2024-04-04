namespace Nyctophobia;

public class GeneralHooks
{
    public static ConditionalWeakTable<Player, ItemData> ItemData = new();

    public static void Apply()
    {
        On.Player.ctor += Player_ctor;
        On.Player.Update += Player_Update;
        On.Player.UpdateBodyMode += Player_UpdateBodyMode;
        On.Player.ObjectEaten += Player_ObjectEaten;
    }

    private static void Player_UpdateBodyMode(On.Player.orig_UpdateBodyMode orig, Player self)
    {
        orig(self);

        if (!ItemData.TryGetValue(self, out var player)) return;

        if (self is null || self.room is null) return;

        player.cacaoSpeed = Custom.LerpAndTick(player.cacaoSpeed, 0f, 0.001f, 0.0001f);

        self.dynamicRunSpeed[0] += player.cacaoSpeed;
    }

    private static void Player_ObjectEaten(On.Player.orig_ObjectEaten orig, Player self, IPlayerEdible edible)
    {
        orig(self, edible);

        if (!ItemData.TryGetValue(self, out var player)) return;

        if (ModManager.ActiveMods.Any(mod => mod.id == "willowwisp.bellyplus") && edible is CacaoFruit)
        {
            self.AddFood(5);
            self.room.PlaySound(SoundID.Death_Lightning_Spark_Object, self.mainBodyChunk, false, 1f, 1f);
            player.cacaoSpeed = 10;
        }

        if (edible is CacaoFruit && self.IsWitness())
        {
            player.cacaoSpeed = 7;
        }

        if (edible is CacaoFruit && !self.IsWitness() && !self.room.game.IsArenaSession)
        {
            player.cacaoSpeed = 5;
        }
        else
        {
            player.cacaoSpeed = 5;
        }
    }

    private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);

        if (!ItemData.TryGetValue(self, out var player)) return;

        if (self is null || self.room is null) return;
    }

    private static void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
    {
        orig(self, abstractCreature, world);

        ItemData.Add(self, new ItemData(self));
    }
}
