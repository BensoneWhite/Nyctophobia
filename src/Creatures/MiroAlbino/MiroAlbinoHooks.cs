namespace Nyctophobia;

public class MiroAlbinoHooks
{
    public static void Apply()
    {
        On.MirosBirdGraphics.DrawSprites += MirosBirdGraphics_DrawSprites;
        On.MirosBirdGraphics.ApplyPalette += MirosBirdGraphics_ApplyPalette;
    }

    private static void MirosBirdGraphics_ApplyPalette(On.MirosBirdGraphics.orig_ApplyPalette orig, MirosBirdGraphics self, SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        orig(self, sLeaser, rCam, palette);
        if (self.bird is MiroAlbino)
        {
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                if (i != self.EyeTrailSprite)
                {
                    sLeaser.sprites[i].color = new Color(0.98f, 0.93f, 0.90f);
                }
            }
        }
    }

    private static void MirosBirdGraphics_DrawSprites(On.MirosBirdGraphics.orig_DrawSprites orig, MirosBirdGraphics self, SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        orig(self, sLeaser, rCam, timeStacker, camPos);
        if (self.bird is MiroAlbino)
        {
            sLeaser.sprites[self.EyeSprite].color = new Color(0.59f, 0.08f, 0.06f);
        }
    }
}