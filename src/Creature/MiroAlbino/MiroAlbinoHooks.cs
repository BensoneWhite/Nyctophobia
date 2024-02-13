namespace Nyctophobia;

public class MiroAlbinoHooks
{
    public static void Apply()
    {
        On.MirosBirdGraphics.ctor += MirosBirdGraphics_ctor;
        On.MirosBird.Update += MirosBird_Update;
    }

    private static void MirosBird_Update(On.MirosBird.orig_Update orig, MirosBird self, bool eu)
    {
        orig(self, eu);
    }

    private static void MirosBirdGraphics_ctor(On.MirosBirdGraphics.orig_ctor orig, MirosBirdGraphics self, PhysicalObject ow)
    {
        orig(self, ow);
    }
}
