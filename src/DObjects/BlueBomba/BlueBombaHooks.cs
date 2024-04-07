using Noise;
using Smoke;
using static ScavengerBomb;
using BombFragment = ScavengerBomb.BombFragment;

namespace Nyctophobia;

public static class BlueBombaHooks
{
    public static void Apply()
    {
        On.ScavengerBomb.Explode += ScavengerBomb_Explode;
    }

    private static void ScavengerBomb_Explode(On.ScavengerBomb.orig_Explode orig, ScavengerBomb self, BodyChunk hitChunk)
    {
        if (self is BlueBomba)
        {
            if (self.slatedForDeletetion)
            {
                return;
            }

            Vector2 vector = Vector2.Lerp(self.firstChunk.pos, self.firstChunk.lastPos, 0.35f);
            self.room.AddObject(new SootMark(self.room, vector, 104f, bigSprite: true));
            if (!self.explosionIsForShow)
            {
                for (var j = 0; j < self.room.physicalObjects.Length; j++)
                {
                    for (var k = 0; k < self.room.physicalObjects[j].Count; k++)
                    {
                        for (var l = 0; l < self.room.physicalObjects[j][k].bodyChunks.Length; l++)
                        {

                            if ((self.room.physicalObjects[j][k] as Creature) is Player)
                            {
                                self.room.AddObject(new Explosion(self.room, self, vector, 7, 300f, 6.2f, 2f, 280f, 0.25f, self.thrownBy, 0f, 208f, 1f));
                            }
                            else
                            {
                                self.room.AddObject(new Explosion(self.room, self, vector, 7, 350f, 6.2f, 2f, 280f, 0.25f, self.thrownBy, 0f, 208f, 1f));
                            }
                        }
                    }
                }
            }
            self.room.AddObject(new Explosion.ExplosionLight(vector, 464f, 1f, 9, self.explodeColor));
            self.room.AddObject(new Explosion.ExplosionLight(vector, 399f, 1f, 4, new Color(1f, 1f, 1f)));
            self.room.AddObject(new ExplosionSpikes(self.room, vector, 28, 49f, 21.7f, 9.1f, 221f, self.explodeColor));
            self.room.AddObject(new ShockWave(vector, 330f, 0.045f, 5));
            for (int i = 0; i < 25; i++)
            {
                Vector2 vector2 = Custom.RNV();
                if (self.room.GetTile(vector + vector2 * 20f).Solid)
                {
                    if (!self.room.GetTile(vector - vector2 * 20f).Solid)
                    {
                        vector2 *= -1f;
                    }
                    else
                    {
                        vector2 = Custom.RNV();
                    }
                }
                for (int j = 0; j < 3; j++)
                {
                    self.room.AddObject(new Spark(vector + vector2 * Mathf.Lerp(30f, 60f, Random.value), vector2 * Mathf.Lerp(7f, 38f, Random.value) + Custom.RNV() * 20f * Random.value, Color.Lerp(self.explodeColor, new Color(1f, 1f, 1f), Random.value), null, 11, 28));
                }
                self.room.AddObject(new Explosion.FlashingSmoke(vector + vector2 * 52f * Random.value, vector2 * Mathf.Lerp(4f, 20f, Mathf.Pow(Random.value, 2f)), 1f + 0.05f * Random.value, new Color(1f, 1f, 1f), self.explodeColor, Random.Range(3, 11)));
            }
            if (self.smoke != null)
            {
                for (int k = 0; k < 8; k++)
                {
                    self.smoke.EmitWithMyLifeTime(vector + Custom.RNV(), Custom.RNV() * Random.value * 17f);
                }
            }
            for (int l = 0; l < 6; l++)
            {
                self.room.AddObject(new BombFragment(vector, Custom.DegToVec(((float)l + Random.value) / 6f * 360f) * Mathf.Lerp(18f, 38f, Random.value)));
            }
            self.room.ScreenMovement(vector, default, 1.3f);
            for (int m = 0; m < self.abstractPhysicalObject.stuckObjects.Count; m++)
            {
                self.abstractPhysicalObject.stuckObjects[m].Deactivate();
            }
            self.room.PlaySound(SoundID.Bomb_Explode, vector);
            self.room.InGameNoise(new InGameNoise(vector, 9000f, self, 1f));
            bool flag = hitChunk != null;
            for (int n = 0; n < 5; n++)
            {
                if (self.room.GetTile(vector + Custom.fourDirectionsAndZero[n].ToVector2() * 20f).Solid)
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                if (self.smoke == null)
                {
                    self.smoke = new BombSmoke(self.room, vector, null, self.explodeColor);
                    self.room.AddObject(self.smoke);
                }
                if (hitChunk != null)
                {
                    self.smoke.chunk = hitChunk;
                }
                else
                {
                    self.smoke.chunk = null;
                    self.smoke.fadeIn = 1f;
                }
                self.smoke.pos = vector;
                self.smoke.stationary = true;
                self.smoke.DisconnectSmoke();
            }
            else
            {
                self.smoke?.Destroy();
            }
            self.Destroy();
        }

        else orig(self, hitChunk);
    }
}
