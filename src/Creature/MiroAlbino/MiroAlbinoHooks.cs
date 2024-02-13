namespace Nyctophobia;

public class MiroAlbinoHooks
{
    public static void Apply()
    {
        On.MirosBirdGraphics.Update += MirosBirdGraphics_Update;
        On.MirosBird.Update += MirosBird_Update;
    }

    private static void MirosBirdGraphics_Update(On.MirosBirdGraphics.orig_Update orig, MirosBirdGraphics self)
    {
        orig(self);
        self.eyeCol = Color.white;
    }

    private static void MirosBird_Update(On.MirosBird.orig_Update orig, MirosBird self, bool eu)
    {
        orig(self, eu);
    }
}
