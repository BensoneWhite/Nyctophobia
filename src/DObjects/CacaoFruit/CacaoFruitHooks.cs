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
            if (edible is CacaoFruit)
            {
                self.AddFood(5);
                self.room.PlaySound(SoundID.Death_Lightning_Spark_Object, self.mainBodyChunk, false, 1f, 1f);
            }
        }

        if (edible is CacaoFruit && !self.IsWitness())
        {
            self.AddFood(self.MaxFoodInStomach);
            self.dynamicRunSpeed[1] += Custom.LerpAndTick(5f, 0f, 0.1f, 0.01f);
        }

        if (edible is CacaoFruit && self.IsWitness())
        {
            self.AddFood(self.MaxFoodInStomach);
            self.dynamicRunSpeed[1] += Custom.LerpAndTick(7f, 0f, 0.1f, 0.01f);
        }
    }
}
