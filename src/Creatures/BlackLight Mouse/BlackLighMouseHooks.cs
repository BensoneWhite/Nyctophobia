namespace Nyctophobia;

public class BlackLighMouseHooks
{
    public static void Apply()
    {
        On.MouseGraphics.Update += MouseGraphics_Update;
        On.MouseGraphics.DrawSprites += MouseGraphics_DrawSprites;
        On.MouseSpark.DrawSprites += MouseSpark_DrawSprites;
    }

    private static void MouseSpark_DrawSprites(On.MouseSpark.orig_DrawSprites orig, MouseSpark self, SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        orig(self, sLeaser, rCam, timeStacker, camPos);
        if (self.room is not null && self.room.physicalObjects is not null)
        {
            for (int j = 0; j < self.room.physicalObjects.Length; j++)
            {
                for (int k = 0; k < self.room.physicalObjects[j].Count; k++)
                {
                    if (self.room.physicalObjects[j][k] != null && self.room.physicalObjects[j][k] is Creature creature && creature is LanternMouse mouse && mouse.Template.type == NTEnums.CreatureType.BlackLightMouse)
                    {
                        sLeaser.sprites[0].color = IsPrideDay ? new Color(Random.value, Random.value, Random.value) : Color.black;
                    }
                }
            }
        }
    }

    private static void MouseGraphics_DrawSprites(On.MouseGraphics.orig_DrawSprites orig, MouseGraphics self, SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        orig(self, sLeaser, rCam, timeStacker, camPos);
        if (self.mouse.Template.type == NTEnums.CreatureType.BlackLightMouse)
        {
            sLeaser.sprites[self.HeadSprite].color = Color.Lerp(self.blackColor, self.blackColor, Mathf.InverseLerp(0f, 0.25f, self.charging));
            for (int i = 0; i < 2; i++)
            {
                new Color(Random.value, Random.value, Random.value);
                if (IsPrideDay)
                {
                    sLeaser.sprites[self.BodySprite(i)].color = Color.Lerp(new Color(Random.value, Random.value, Random.value), new Color(Random.value, Random.value, Random.value), Mathf.InverseLerp(0f, 0.25f, self.charging));
                    sLeaser.sprites[self.EyeASprite(i)].color = new Color(Random.value, Random.value, Random.value);
                    sLeaser.sprites[self.EyeBSprite(i)].color = new Color(Random.value, Random.value, Random.value);
                    for (int j = 0; j < 2; j++)
                    {
                        sLeaser.sprites[self.BackSpotSprite(i, j)].color = Color.Lerp(new Color(Random.value, Random.value, Random.value), new Color(Random.value, Random.value, Random.value), Mathf.InverseLerp(0f, 0.25f, self.charging));
                        sLeaser.sprites[self.LimbSprite(i, j)].color = Color.Lerp(new Color(Random.value, Random.value, Random.value), new Color(Random.value, Random.value, Random.value), Mathf.InverseLerp(0f, 0.25f, self.charging));
                    }
                }
                else
                {
                    sLeaser.sprites[self.BodySprite(i)].color = Color.Lerp(self.blackColor, self.blackColor, Mathf.InverseLerp(0f, 0.25f, self.charging));
                    sLeaser.sprites[self.EyeASprite(i)].color = self.DecalColor;
                    sLeaser.sprites[self.EyeBSprite(i)].color = self.EyesColor;
                    for (int j = 0; j < 2; j++)
                    {
                        sLeaser.sprites[self.BackSpotSprite(i, j)].color = Color.Lerp(self.blackColor, self.blackColor, Mathf.InverseLerp(0f, 0.25f, self.charging));
                        sLeaser.sprites[self.LimbSprite(i, j)].color = Color.Lerp(self.blackColor, self.blackColor, Mathf.InverseLerp(0f, 0.25f, self.charging));
                    }
                }
            }
        }
    }

    private static void MouseGraphics_Update(On.MouseGraphics.orig_Update orig, MouseGraphics self)
    {
        orig(self);

        if (self.mouse.Template.type == NTEnums.CreatureType.BlackLightMouse && self.lightSource != null)
        {
            self.lightSource.alpha = 0f;
            self.lightSource.rad = 0f;
            self.lightSource.color = IsPrideDay ? new Color(Random.value, Random.value, Random.value) : Color.black;
        }
    }
}