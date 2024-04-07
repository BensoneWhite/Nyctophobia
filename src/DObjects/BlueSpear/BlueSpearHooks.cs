using Noise;
using Smoke;
using static PhysicalObject;
using static Weapon;

namespace Nyctophobia;

public static class BlueSpearHooks
{
    public static void Apply()
    {
        On.ExplosiveSpear.MiniExplode += ExplosiveSpear_MiniExplode;
        On.ExplosiveSpear.Explode += ExplosiveSpear_Explode;
    }

    private static void ExplosiveSpear_Explode(On.ExplosiveSpear.orig_Explode orig, ExplosiveSpear self)
    {
        if (self is BlueSpear)
        {
            if (self.exploded)
            {
                return;
            }

            self.exploded = true;
            if (self.stuckInObject != null)
            {
                if (self.stuckInObject is Creature && self.stuckInObject is not Player)
                {
                    (self.stuckInObject as Creature).Violence(self.firstChunk, self.rotation * 12f, self.stuckInChunk, null, Creature.DamageType.Explosion, (self.stuckInAppendage != null) ? 1.8f : 4.2f, 120f);
                }
                else
                {
                    self.stuckInChunk.vel += self.rotation * 12f / self.stuckInChunk.mass;
                }
            }

            Vector2 vector = self.firstChunk.pos + self.rotation * (self.pivotAtTip ? 0f : 10f);
            self.room.AddObject(new SootMark(self.room, vector, 100f, bigSprite: false));
            self.room.AddObject(new Explosion(self.room, self, vector, 5, 270f, 5f, 2f, 60f, 0.3f, self.thrownBy, 0f, 0f, 0.7f));
            for (int i = 0; i < 14; i++)
            {
                self.room.AddObject(new Explosion.ExplosionSmoke(vector, Custom.RNV() * 5f * UnityEngine.Random.value, 2f));
            }

            self.room.AddObject(new Explosion.ExplosionLight(vector, 360f, 1f, 3, self.explodeColor));
            self.room.AddObject(new ExplosionSpikes(self.room, vector, 13, 20f, 5f, 7f, 110f, self.explodeColor));
            self.room.AddObject(new ShockWave(vector, 180f, 0.045f, 4));
            for (int j = 0; j < 20; j++)
            {
                Vector2 vector2 = Custom.RNV();
                self.room.AddObject(new Spark(vector + vector2 * UnityEngine.Random.value * 40f, vector2 * Mathf.Lerp(4f, 30f, UnityEngine.Random.value), self.explodeColor, null, 4, 18));
            }

            self.room.ScreenMovement(vector, default, 0.7f);
            for (int k = 0; k < 2; k++)
            {
                Smolder smolder = null;
                if (self.stuckInObject != null)
                {
                    smolder = new Smolder(self.room, self.stuckInChunk.pos, self.stuckInChunk, self.stuckInAppendage);
                }
                else
                {
                    Vector2? vector3 = SharedPhysics.ExactTerrainRayTracePos(self.room, self.firstChunk.pos, self.firstChunk.pos + ((k == 0) ? (self.rotation * 20f) : (Custom.RNV() * 20f)));
                    if (vector3.HasValue)
                    {
                        smolder = new Smolder(self.room, vector3.Value + Custom.DirVec(vector3.Value, self.firstChunk.pos) * 3f, null, null);
                    }
                }

                if (smolder != null)
                {
                    self.room.AddObject(smolder);
                }
            }

            self.abstractPhysicalObject.LoseAllStuckObjects();
            self.room.PlaySound(SoundID.Fire_Spear_Explode, vector);
            self.room.InGameNoise(new InGameNoise(vector, 8000f, self, 1f));
            self.Destroy();
        }
        else orig(self);
    }

    public static float ConchunkWeight(Vector2 pushDir, BodyChunkConnection con, ExplosiveSpear explosiveSpear)
    {
        if (con.chunk1 == explosiveSpear.stuckInChunk)
        {
            return Custom.LerpMap(Vector2.Dot(pushDir, Custom.DirVec(explosiveSpear.stuckInChunk.pos, con.chunk2.pos)), -0.5f, 1f, 0f, 7f, 1.5f);
        }

        if (con.chunk2 == explosiveSpear.stuckInChunk)
        {
            return Custom.LerpMap(Vector2.Dot(pushDir, Custom.DirVec(explosiveSpear.stuckInChunk.pos, con.chunk1.pos)), -0.5f, 1f, 0f, 7f, 1.5f);
        }

        return 0f;
    }

    public static void ExplosiveSpear_MiniExplode(On.ExplosiveSpear.orig_MiniExplode orig, ExplosiveSpear self)
    {
        if (self is BlueSpear)
        {
            if (self.stuckInObject != null)
            {
                float num = 7f;
                float num2 = 0f;
                Vector2 vector = self.rotation;
                if (self.room.readyForAI && self.room.aimap.getAItile(self.firstChunk.pos).floorAltitude < 3)
                {
                    vector = Vector3.Slerp(vector, new Vector2(0f, 1f), 0.2f);
                }

                for (int i = 0; i < self.stuckInObject.bodyChunkConnections.Length; i++)
                {
                    if (self.stuckInObject.bodyChunkConnections[i].type != BodyChunkConnection.Type.Pull && (self.stuckInObject.bodyChunkConnections[i].chunk1 == self.stuckInChunk || self.stuckInObject.bodyChunkConnections[i].chunk2 == self.stuckInChunk))
                    {
                        num2 += ConchunkWeight(vector, self.stuckInObject.bodyChunkConnections[i], self);
                    }
                }

                if (num2 > 0f)
                {
                    float num3 = Mathf.Clamp(num2 * 2f, 0f, 6f);
                    num -= num3;
                    for (int j = 0; j < self.stuckInObject.bodyChunkConnections.Length; j++)
                    {
                        if (self.stuckInObject.bodyChunkConnections[j].type != BodyChunkConnection.Type.Pull)
                        {
                            if (self.stuckInObject.bodyChunkConnections[j].chunk2 == self.stuckInChunk)
                            {
                                self.stuckInObject.bodyChunkConnections[j].chunk1.vel += vector * num3 * ConchunkWeight(vector, self.stuckInObject.bodyChunkConnections[j], self) / (num2 * self.stuckInObject.bodyChunkConnections[j].chunk1.mass);
                                self.stuckInObject.bodyChunkConnections[j].chunk1.pos += vector * num3 * ConchunkWeight(vector, self.stuckInObject.bodyChunkConnections[j], self) / (num2 * self.stuckInObject.bodyChunkConnections[j].chunk1.mass);
                            }
                            else if (self.stuckInObject.bodyChunkConnections[j].chunk1 == self.stuckInChunk)
                            {
                                self.stuckInObject.bodyChunkConnections[j].chunk2.vel += vector * num3 * ConchunkWeight(vector, self.stuckInObject.bodyChunkConnections[j], self) / (num2 * self.stuckInObject.bodyChunkConnections[j].chunk2.mass);
                                self.stuckInObject.bodyChunkConnections[j].chunk2.pos += vector * num3 * ConchunkWeight(vector, self.stuckInObject.bodyChunkConnections[j], self) / (num2 * self.stuckInObject.bodyChunkConnections[j].chunk2.mass);
                            }
                        }
                    }
                }

                if (self.stuckInObject is Creature && self.stuckInObject is not Player)
                {
                    (self.stuckInObject as Creature).Violence(self.firstChunk, vector * num, self.stuckInChunk, null, Creature.DamageType.Explosion, (self.stuckInAppendage != null) ? 0.4f : 1.2f, 0f);
                }
                else
                {
                    self.stuckInChunk.vel += vector * num / self.stuckInChunk.mass;
                }

                self.stuckInChunk.pos += vector * num / self.stuckInChunk.mass;
            }

            Vector2 vector2 = self.firstChunk.pos + self.rotation * (self.pivotAtTip ? 0f : 15f);
            self.room.AddObject(new Explosion.ExplosionLight(vector2, 160f, 1f, 2, self.explodeColor));
            for (int k = 0; k < 8; k++)
            {
                Vector2 vector3 = Custom.RNV();
                self.room.AddObject(new Spark(vector2 + vector3 * Random.value * 10f, vector3 * Mathf.Lerp(6f, 18f, Random.value), self.explodeColor, null, 4, 18));
            }

            self.room.AddObject(new ShockWave(vector2, 120f, 0.035f, 2));
            self.room.PlaySound(SoundID.Fire_Spear_Pop, vector2);
            for (int l = 0; l < self.rag.GetLength(0); l++)
            {
                self.rag[l, 2] += (Custom.DirVec(vector2, self.rag[l, 0]) + Custom.RNV() - self.rotation) * Random.value * 5f;
                self.rag[l, 0] += (Custom.DirVec(vector2, self.rag[l, 0]) + Custom.RNV() - self.rotation) * Random.value * 5f;
            }

            if (self.mode == Mode.Free && self.stuckInObject == null)
            {
                if (self.firstChunk.vel.y < 0f)
                {
                    self.firstChunk.vel.y *= 0.5f;
                }

                self.firstChunk.vel -= (Custom.RNV() + self.rotation) * Random.value * ((self.firstChunk.ContactPoint.y < 0) ? 5f : 15f);
                self.SetRandomSpin();
            }

            self.room.InGameNoise(new InGameNoise(vector2, 800f, self, 1f));
            self.vibrate = Math.Max(self.vibrate, 6);
        }
        else orig(self);
    }
}
