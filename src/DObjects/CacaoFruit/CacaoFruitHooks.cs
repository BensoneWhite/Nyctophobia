namespace Nyctophobia;

public class CacaoFruitHooks
{
    public static void Apply()
    {
        On.Player.ObjectEaten += Player_ObjectEaten;
    }

    private static void Player_ObjectEaten(On.Player.orig_ObjectEaten orig, Player self, IPlayerEdible edible)
    {
        if (ModManager.ActiveMods.Any(mod => mod.id == "willowwisp.bellyplus"))
        {
            if (edible is CacaoFruit && !self.room.game.IsArenaSession)
            {
                self.AddFood(5);
                self.room.PlaySound(SoundID.Death_Lightning_Spark_Object, self.mainBodyChunk, false, 1f, 1f);
            }
        }

        if(self.room.game.IsArenaSession)
        {
            self.AddFood(10);
        }

        if (!self.room.game.IsArenaSession && edible is CacaoFruit && !self.IsWitness())
        {
            self.AddFood(self.MaxFoodInStomach);
            self.dynamicRunSpeed[1] += Custom.LerpAndTick(5f, 0f, 0.1f, 0.01f);
        }

        if (!self.room.game.IsArenaSession && edible is CacaoFruit && self.IsWitness())
        {
            self.AddFood(self.MaxFoodInStomach);
            self.dynamicRunSpeed[1] += Custom.LerpAndTick(7f, 0f, 0.1f, 0.01f);
        }
    }
}
