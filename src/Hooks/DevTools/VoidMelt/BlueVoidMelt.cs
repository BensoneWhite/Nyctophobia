﻿namespace Nyctophobia;

//This DevTool is half made, POM makes it better
public class BlueVoidMelt
{
    public static void Apply()
    {
        On.MeltLights.ApplyPalette += MeltLights_ApplyPalette;
        On.MeltLights.Update += MeltLights_Update;
        On.MeltLights.MeltLight.Update += MeltLight_Update;
    }

    private static void MeltLight_Update(On.MeltLights.MeltLight.orig_Update orig, MeltLights.MeltLight self, bool eu)
    {
        if (ModManager.MSC && self.room.world.region != null && self.room.world.region.name == "MO")
        {
            self.lastPos = self.pos;
            self.pos.y -= self.speed;
            self.lightSource.setRad = Mathf.Clamp(self.lightSource.Rad + Mathf.Lerp(-10f, 10f, Random.value), self.rad * 0.5f, self.rad * 1.5f);
            self.lightSource.setPos = self.pos;
            self.lightSource.setAlpha = Mathf.InverseLerp(0f - self.lightSource.Rad, 0f, self.pos.y);
            for (int i = 0; i < self.room.game.cameras.Length; i++)
            {
                if (self.room.game.cameras[i].voidSeaMode)
                {
                    self.lightSource.setAlpha = 0f;
                }
            }
            self.lightSource.stayAlive = true;
            if (self.pos.y < 0f - self.lightSource.Rad)
            {
                self.Destroy();
            }
        }
        else
        {
            orig(self, eu);
        }
    }

    private static void MeltLights_Update(On.MeltLights.orig_Update orig, MeltLights self, bool eu)
    {
        var room = self.room;

        if (ModManager.MSC && room.world.region != null && room.world.region.name == "MO")
        {
            if (!self.hasLookedForVoidSea)
            {
                for (int i = 0; i < self.room.updateList.Count; i++)
                {
                    if (self.room.updateList[i] is VoidSeaScene)
                    {
                        self.voidSeaEffect = self.room.updateList[i] as VoidSeaScene;
                        break;
                    }
                }
                self.hasLookedForVoidSea = true;
            }
            if (self.wait > 0f)
            {
                self.wait -= 1f;
            }
            else
            {
                self.wait += Mathf.Lerp(40f, 10f, self.Amount) / ((float)self.room.TileWidth / 55f);
                if (Random.value < self.SpawnChance)
                {
                    self.room.AddObject(new MeltLights.MeltLight(self.Amount, new Vector2(Mathf.Lerp(-150f, room.PixelWidth + 150f, Random.value), room.PixelHeight + 600f), room, new Color(0.102f, 0.329f, 0.839f)));
                }
            }
            for (int j = 0; j < room.physicalObjects.Length; j++)
            {
                for (int p = 0; p < room.physicalObjects[j].Count; p++)
                {
                    if (room.physicalObjects[j][p].room == room)
                    {
                        for (int c = 0; c < room.physicalObjects[j][p].bodyChunks.Length; c++)
                        {
                            room.physicalObjects[j][p].bodyChunks[c].vel.y += room.physicalObjects[j][p].gravity * 0.5f * self.Amount * (1f - room.physicalObjects[j][p].bodyChunks[c].submersion);
                        }
                    }
                }
            }
        }
        else
        {
            orig(self, eu);
        }
    }

    private static void MeltLights_ApplyPalette(On.MeltLights.orig_ApplyPalette orig, MeltLights self, SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        if (ModManager.MSC && self.room.world.region != null && self.room.world.region.name == "MO")
            self.color = new Color(0f, 0f, 1f);
        else
            orig(self, sLeaser, rCam, palette);
    }
}