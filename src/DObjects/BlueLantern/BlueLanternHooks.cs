namespace Nyctophobia;

public static class BlueLanternHooks
{
    public static void Apply()
    {
        On.Lantern.ApplyPalette += Lantern_ApplyPalette;
    }

    private static void Lantern_ApplyPalette(On.Lantern.orig_ApplyPalette orig, Lantern self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        if (self is BlueLantern)
        {
            sLeaser.sprites[0].color = new Color(0.196f, 0.596f, 0.965f);
            sLeaser.sprites[1].color = new Color(1f, 1f, 1f);
            sLeaser.sprites[2].color = Color.Lerp(new Color(0.196f, 0.596f, 0.965f), new Color(1f, 1f, 1f), 0.3f);
            sLeaser.sprites[3].color = new Color(0.4f, 0.596f, 0.965f);
            if (self.stick != null)
            {
                sLeaser.sprites[4].color = palette.blackColor;
            }
        }
        else orig(self, sLeaser, rCam, palette);
    }
}
