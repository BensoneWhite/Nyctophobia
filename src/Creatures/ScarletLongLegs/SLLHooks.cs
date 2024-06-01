namespace Nyctophobia;

public class SLLHooks
{
    public static void Apply()
    {
        On.DaddyLongLegs.ctor += DaddyLongLegs_ctor;
    }

    private static void DaddyLongLegs_ctor(On.DaddyLongLegs.orig_ctor orig, DaddyLongLegs self, AbstractCreature abstractCreature, World world)
    {
        orig(self, abstractCreature, world);

        if (self.Template.type == NTEnums.CreatureType.ScarletLongLegs)
        {
            if (IsPrideDay)
            {
                self.eyeColor = new Color(Random.value, Random.value, Random.value);
                self.effectColor = new Color(Random.value, Random.value, Random.value);
                self.colorClass = true;
            }
            else
            {
                self.eyeColor = Color.red;
                self.effectColor = Color.red;
                self.colorClass = true;
            }
        }
    }
}